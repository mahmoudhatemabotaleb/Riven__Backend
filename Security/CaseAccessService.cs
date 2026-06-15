using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.Models;
using RivenBackend.Repositories;

namespace RivenBackend.Security
{
    public class CaseAccessService : ICaseAccessService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CaseAccessService(
            ICaseRepository caseRepository,
            AppDbContext context,
            ICurrentUserService currentUser)
        {
            _caseRepository = caseRepository;
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Case> GetAuthorizedCaseAsync(int caseId)
        {
            var case_ = await _caseRepository.GetByIdAsync(caseId)
                ?? throw new KeyNotFoundException("Case not found.");

            if (!CanAccessCase(case_))
                throw new UnauthorizedAccessException("You do not have access to this case.");

            return case_;
        }

        public async Task EnsureCanAccessCaseAsync(int caseId) =>
            _ = await GetAuthorizedCaseAsync(caseId);

        public bool CanAccessCase(Case case_)
        {
            if (_currentUser.IsAdmin) return true;
            if (_currentUser.IsDoctor && case_.HospitalId == _currentUser.HospitalId) return true;
            if (_currentUser.IsParamedic && case_.UserId == _currentUser.UserId) return true;
            return false;
        }

        public IQueryable<Case> FilterAccessibleCases(IQueryable<Case> query)
        {
            if (_currentUser.IsAdmin) return query;
            if (_currentUser.IsDoctor)
                return query.Where(c => c.HospitalId == _currentUser.HospitalId);
            if (_currentUser.IsParamedic)
                return query.Where(c => c.UserId == _currentUser.UserId);
            return query.Where(_ => false);
        }

        public async Task<List<int>> GetAccessibleCaseIdsAsync() =>
            await FilterAccessibleCases(_context.Cases)
                .Select(c => c.CaseId)
                .ToListAsync();

        public async Task EnsureCanAccessAmbulanceAsync(int ambulanceId)
        {
            var ambulance = await _context.Ambulances.FindAsync(ambulanceId)
                ?? throw new KeyNotFoundException("Ambulance not found.");

            if (_currentUser.IsAdmin) return;

            if (_currentUser.IsDoctor && ambulance.HospitalId == _currentUser.HospitalId)
                return;

            if (_currentUser.IsParamedic)
            {
                var ownsCase = await _context.Cases.AnyAsync(c =>
                    c.AmbulanceId == ambulanceId && c.UserId == _currentUser.UserId);
                if (ownsCase) return;
            }

            throw new UnauthorizedAccessException("You do not have access to this ambulance.");
        }

        public IQueryable<Ambulance> FilterAccessibleAmbulances(IQueryable<Ambulance> query)
        {
            if (_currentUser.IsAdmin) return query;
            if (_currentUser.IsDoctor)
                return query.Where(a => a.HospitalId == _currentUser.HospitalId);

            if (_currentUser.IsParamedic)
            {
                var ambulanceIds = _context.Cases
                    .Where(c => c.UserId == _currentUser.UserId)
                    .Select(c => c.AmbulanceId)
                    .Distinct();
                return query.Where(a => ambulanceIds.Contains(a.AmbulanceId));
            }

            return query.Where(_ => false);
        }
    }
}
