namespace RivenBackend.Security
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        int HospitalId { get; }
        string Role { get; }
        bool IsAdmin { get; }
        bool IsDoctor { get; }
        bool IsParamedic { get; }
    }
}
