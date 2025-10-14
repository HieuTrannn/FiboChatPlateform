namespace Course.Application.Interfaces
{
    public record AccountResponse(Guid Id, string Firstname, string Lastname, string Email, string PhoneNumber, string Address, string Role, string Status, string Cohort, DateTime CreatedAt);
    public interface IAccountsClient
    {
        Task<AccountResponse> GetLecturerByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AccountResponse> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}