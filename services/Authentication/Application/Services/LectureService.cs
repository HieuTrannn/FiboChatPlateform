
using Authentication.Application.Interfaces;
using Authentication.Domain.Abstraction;
using Authentication.Domain.Entities;
using Contracts.Common;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using static Authentication.Application.DTOs.AuthenDTO;
using static Authentication.Application.DTOs.LectureDTO;

namespace Authentication.Application.Services
{
    public class LectureService : ILectureService
    {
        private readonly ILogger<ILectureService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRsaService _rsaService;
        private readonly IEmailService _emailService;
        private readonly IGenericRepository<Account> _accountRepository;

        public LectureService(ILogger<ILectureService> logger, IUnitOfWork unitOfWork, IEmailService service, IRsaService rsaService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _rsaService = rsaService;
            _emailService = service;
            _accountRepository = _unitOfWork.GetRepository<Account>();
        }

        public async Task<ApiResponse<RegisterResponse>> CreateLecturer(LectureRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new lecture with email: {Email}", request.Email);


                var existingAccount = await _accountRepository.FindByConditionAsync(a => a.Email == request.Email);
                if (existingAccount is not null)
                {
                    _logger.LogWarning("Lecture creation failed. Email {Email} already exists.", request.Email);
                    return ApiResponse<RegisterResponse>.BadRequest("Email already exists.");
                }

                var rawPassword = GenerateRandomPassword();
                var lecture = request.Adapt<Account>();
                lecture.Firstname = "null";
                lecture.Lastname = "null";
                lecture.Password = _rsaService.Encrypt(rawPassword);
                lecture.CreatedAt = DateTime.UtcNow;
                lecture.IsVerified = true; // Lectures are verified by default

                var roleRepository = _unitOfWork.GetRepository<Role>();
                var lectureRole = await roleRepository.FindByConditionAsync(
                    r => r.RoleName.ToLower() == StaticEnum.RoleEnum.Lecturer.ToString().ToLower());

                if (lectureRole is null)
                {
                    _logger.LogError("Lecture creation failed. Lecturer role not found.");
                    return ApiResponse<RegisterResponse>.Error("Lecturer role not found.");

                }
                lecture.RoleId = lectureRole.Id;

                await _accountRepository.InsertAsync(lecture);
                await _unitOfWork.SaveChangeAsync();

                await SendUserRegistrationEmail(lecture, rawPassword);

                var lecturer = new Lecture
                {
                    FullName = request.FullName,
                    Status = StaticEnum.StatusEnum.Active.ToString(),
                    LecturerId = lecture.Id
                };
                var lectureRepository = _unitOfWork.GetRepository<Lecture>();
                await lectureRepository.InsertAsync(lecturer);
                await _unitOfWork.SaveChangeAsync();

                _logger.LogInformation("Lecture created successfully with email: {Email}", request.Email);
                return ApiResponse<RegisterResponse>.Ok(new RegisterResponse
                {

                    Success = true,
                    Message = "Lecture created successfully."
                }, "Registration successful",
                 nameof(HttpStatusCode.OK)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating lecture with email: {Email}", request.Email);
                return ApiResponse<RegisterResponse>.Error("An error occurred while creating the lecture: " + ex);
            }
        }

        public async Task<List<LectureResponse>> GetAllLecturers()
        {
            try
            {
                _logger.LogInformation("Retrieving all lecturers.");
                var lectureRepository = _unitOfWork.GetRepository<Lecture>();
                var lectures = await lectureRepository.GetAllAsync();

                if (lectures == null || !lectures.Any())
                {
                    _logger.LogWarning("No lecturers found.");
                    return new List<LectureResponse>();
                }


                return lectures.Adapt<List<LectureResponse>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving lecturers.");
                throw; // Rethrow or handle as needed

            }
        }

        public async Task<LectureResponse> GetLecturerById(string id)
        {
            try
            {
                _logger.LogInformation("Retrieving lecturer with ID: {Id}", id);
                var lectureRepository = _unitOfWork.GetRepository<Lecture>();
                var lecture = await lectureRepository.Entities
                    .Include(l => l.Account)
                    .FirstOrDefaultAsync(l => l.LecturerId.ToString() == id);
                if (lecture == null)
                {
                    _logger.LogWarning("Lecturer with ID: {Id} not found.", id);
                    return null;
                }

                return lecture.Adapt<LectureResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving lecturer with ID: {Id}", id);
                throw; // Rethrow or handle as needed
            }
        }

        public async Task<RegisterResponse> DeleteLecturerById(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting lecturer with ID: {Id}", id);
                _unitOfWork.BeginTransaction();

                var lectureRepository = _unitOfWork.GetRepository<Lecture>();
                var lecture = await lectureRepository.GetByIdAsync(id);

                if (lecture == null)
                {
                    _logger.LogWarning("Lecturer with ID: {Id} not found.", id);
                    return new RegisterResponse { Success = false, Message = "Lecturer not found." };
                }

                lecture.Status = StaticEnum.StatusEnum.Inactive.ToString();
                await lectureRepository.UpdateAsync(lecture);
                await _unitOfWork.SaveChangeAsync();
                _unitOfWork.CommitTransaction();
                return new RegisterResponse { Success = true, Message = "Lecturer deleted successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting lecturer with ID: {Id}", id);
                throw; // Rethrow or handle as needed
            }
        }

        public async Task<RegisterResponse> UpdateLecturerById(Guid id, LectureRequest request)
        {
            try
            {
                _logger.LogInformation("Updating lecturer with ID: {Id}", id);
                var lectureRepository = _unitOfWork.GetRepository<Lecture>();
                var lecture = await lectureRepository.GetByIdAsync(id);
                var accountRepository = _unitOfWork.GetRepository<Account>();
                var account = await accountRepository.GetByIdAsync(id.ToString());
                if (lecture == null)
                {
                    _logger.LogWarning("Lecturer with ID: {Id} not found.", id);
                    return new RegisterResponse { Success = false, Message = "Lecturer not found." };
                }

                if (account == null)
                {
                    _logger.LogWarning("Account with ID: {Id} not found.", id);
                    return new RegisterResponse { Success = false, Message = "Account not found." };
                }
                account.Email = request.Email;
                account.UpdatedAt = DateTime.UtcNow;
                account.UpdatedAt = DateTime.UtcNow;

                lecture.FullName = request.FullName;

                await accountRepository.UpdateAsync(account);
                await lectureRepository.UpdateAsync(lecture);
                await _unitOfWork.SaveChangeAsync();

                return new RegisterResponse { Success = true, Message = "Lecturer updated successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating lecturer with ID: {Id}", id);
                throw; // Rethrow or handle as needed
            }
        }

        private async Task SendUserRegistrationEmail(Account account, string rawPassword)
        {
            var templateData = new Dictionary<string, string>
            {
                { "USERNAME", account.Email },
                { "PASSWORD", rawPassword },
                { "subject", "Thông tin đăng nhập FiboAiChat"}
            };

            await _emailService.SendEmailAsync(
                recipientEmail: account.Email,
                recipientName: account.Email,
                templateFileName: "LectureRegistration.html",
                templateData: templateData
            );
        }

        private static string GenerateRandomPassword(int length = 10)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            var random = new Random();
            var chars = Enumerable.Range(0, length)
                .Select(x => validChars[random.Next(validChars.Length)]);
            return new string(chars.ToArray());
        }
    }
}
