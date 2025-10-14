using System.Net;
using System.Net.Http.Json;
using Course.Application.Interfaces;
using Course.Domain.Exceptions;

namespace Course.Application.Implements
{
    public class AccountsClient : IAccountsClient
    {
        private readonly HttpClient _httpClient;
        public AccountsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AccountResponse> GetLecturerByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync($"api/accounts/lecturers/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new CustomExceptions.NoDataFoundException("Lecturer not found");
            }
            response.EnsureSuccessStatusCode();
            var account = await response.Content.ReadFromJsonAsync<AccountResponse>(cancellationToken: cancellationToken);
            if (account == null)
            {
                throw new CustomExceptions.NoDataFoundException("Lecturer not found");
            }
            if (!string.Equals(account.Role, "Lecturer", StringComparison.OrdinalIgnoreCase))
            {
                throw new CustomExceptions.InvalidRoleException("Account is not a lecturer");
            }
            return account;
        }

        public async Task<AccountResponse> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync($"api/accounts/students/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new CustomExceptions.NoDataFoundException("Student not found");
            }
            response.EnsureSuccessStatusCode();
            var account = await response.Content.ReadFromJsonAsync<AccountResponse>(cancellationToken: cancellationToken);
            if (account == null)
            {
                throw new CustomExceptions.NoDataFoundException("Student not found");
            }
            if (!string.Equals(account.Role, "Student", StringComparison.OrdinalIgnoreCase))
            {
                throw new CustomExceptions.InvalidRoleException("Account is not a student");
            }
            return account;
        }
    }
}