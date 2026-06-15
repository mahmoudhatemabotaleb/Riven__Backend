using Microsoft.AspNetCore.Mvc;
using RivenBackend.Data;
using RivenBackend.Security;

namespace RivenBackend.Controllers.Base
{
    public abstract class CaseResourceControllerBase : ControllerBase
    {
        protected readonly AppDbContext Context;
        protected readonly ICaseAccessService CaseAccess;

        protected CaseResourceControllerBase(AppDbContext context, ICaseAccessService caseAccess)
        {
            Context = context;
            CaseAccess = caseAccess;
        }

        protected async Task<ActionResult?> AuthorizeCaseAsync(int caseId)
        {
            try
            {
                await CaseAccess.EnsureCanAccessCaseAsync(caseId);
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
