using Microsoft.Extensions.Logging;
using Course.Domain.Abstraction;
using Course.Application.DTOs.MasterTopicDTOs;
using Course.Domain.Exceptions;
using Course.Domain.Entities;
using Contracts.Common;
using Course.Application.Interfaces;

namespace Course.Application.Implements
{
    public class MasterTopicService : IMasterTopicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MasterTopicService> _logger;
        private readonly IExternalApiService _externalApiService;

        public MasterTopicService(IUnitOfWork unitOfWork, ILogger<MasterTopicService> logger, IExternalApiService externalApiService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _externalApiService = externalApiService;
        }

        public async Task<MasterTopicResponse> GetByIdAsync(Guid id)
        {
            // Include LecturerMasterTopics relationship
            var masterTopic = await _unitOfWork.GetRepository<MasterTopic>()
                .GetByIdAsync(id);

            if (masterTopic == null)
            {
                _logger.LogError("Master topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Master topic not found");
            }
            return await ToMasterTopicResponse(masterTopic);
        }

        public async Task<BasePaginatedList<MasterTopicResponse>> GetAllAsync(int page, int pageSize)
        {
            var masterTopics = await _unitOfWork.GetRepository<MasterTopic>().GetAllAsync();
            var response = await Task.WhenAll(masterTopics.Select(ToMasterTopicResponse));
            return new BasePaginatedList<MasterTopicResponse>(response, masterTopics.Count, page, pageSize);
        }

        public async Task<MasterTopicResponse> CreateAsync(MasterTopicCreateRequest request)
        {
            // Validate domain exists
            var domain = await _unitOfWork.GetRepository<Domain.Entities.Domain>().GetByIdAsync(request.DomainId);
            if (domain == null)
            {
                _logger.LogError("Domain not found with id: {DomainId}", request.DomainId);
                throw new CustomExceptions.NoDataFoundException("Domain not found");
            }

            // Validate semester exists via external API (N:1 relationship) - Make it optional
            SemesterResponse? semesterResponse = null;
            if (request.SemesterId != Guid.Empty)
            {
                try
                {
                    semesterResponse = await _externalApiService.GetSemesterByIdAsync(request.SemesterId.ToString());
                    if (semesterResponse == null)
                    {
                        _logger.LogWarning("Semester not found with id: {SemesterId}, but continuing with creation", request.SemesterId);
                        // Don't throw exception, just log warning and continue
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to validate semester {SemesterId}, but continuing with creation", request.SemesterId);
                    // Don't throw exception, just log warning and continue
                }
            }

            // Validate lecturers exist via external API (N:N relationship) - Make it optional
            if (request.LecturerIds != null && request.LecturerIds.Any())
            {
                var lecturerValidationTasks = request.LecturerIds.Select(async lecturerId =>
                {
                    try
                    {
                        var lecturer = await _externalApiService.GetLecturerByIdAsync(lecturerId.ToString());
                        if (lecturer == null)
                        {
                            _logger.LogWarning("Lecturer not found with id: {LecturerId}, but continuing with creation", lecturerId);
                            // Don't throw exception, just log warning and continue
                        }
                        return lecturer;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to validate lecturer {LecturerId}, but continuing with creation", lecturerId);
                        // Don't throw exception, just log warning and continue
                        return null;
                    }
                });

                await Task.WhenAll(lecturerValidationTasks);
            }

            var masterTopic = new MasterTopic
            {
                DomainId = request.DomainId,
                SemesterId = request.SemesterId,
                Name = request.Name,
                Description = request.Description,
            };

            // Add lecturer relationships (N:N)
            if (request.LecturerIds != null)
            {
                foreach (var lecturerId in request.LecturerIds)
                {
                    masterTopic.LecturerMasterTopics.Add(new LecturerMasterTopic
                    {
                        LecturerId = lecturerId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _unitOfWork.GetRepository<MasterTopic>().InsertAsync(masterTopic);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Master topic created successfully with id: {Id}, Semester: {SemesterId}, Lecturers: {LecturerCount}",
                masterTopic.Id, masterTopic.SemesterId, request.LecturerIds?.Count ?? 0);
            return await ToMasterTopicResponse(masterTopic);
        }

        public async Task<MasterTopicResponse> UpdateAsync(Guid id, MasterTopicUpdateRequest request)
        {
            var masterTopic = await _unitOfWork.GetRepository<MasterTopic>().GetByIdAsync(id);
            if (masterTopic == null)
            {
                _logger.LogError("Master topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Master topic not found");
            }
            masterTopic.Name = request.Name ?? masterTopic.Name;
            masterTopic.Description = request.Description ?? masterTopic.Description;
            masterTopic.DomainId = request.DomainId;
            masterTopic.SemesterId = request.SemesterId;
            if (request.LecturerIds != null)
            {
                foreach (var lecturerId in request.LecturerIds)
                {
                    masterTopic.LecturerMasterTopics.Add(new LecturerMasterTopic { LecturerId = lecturerId });
                }
            }
            await _unitOfWork.GetRepository<MasterTopic>().UpdateAsync(masterTopic);
            await _unitOfWork.SaveChangesAsync();
            return await ToMasterTopicResponse(masterTopic);
        }

        public async Task<MasterTopicResponse> DeleteAsync(Guid id)
        {
            var masterTopic = await _unitOfWork.GetRepository<MasterTopic>().GetByIdAsync(id);
            if (masterTopic == null)
            {
                _logger.LogError("Master topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Master topic not found");
            }
            await _unitOfWork.GetRepository<MasterTopic>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return await ToMasterTopicResponse(masterTopic);
        }

        private async Task<MasterTopicResponse> ToMasterTopicResponse(MasterTopic masterTopic)
        {
            // Get Domain information (N:1 relationship)
            var domain = await _unitOfWork.GetRepository<Domain.Entities.Domain>().GetByIdAsync(masterTopic.DomainId);
            var domainResponse = new DomainResponse
            {
                Id = domain.Id,
                Name = domain.Name,
                Description = domain.Description,
                Status = domain.Status,
                CreatedAt = domain.CreatedAt,
            };

            // Get Semester information from external API (N:1 relationship)
            SemesterResponse? semesterResponse = null;
            if (masterTopic.SemesterId != Guid.Empty)
            {
                try
                {
                    semesterResponse = await _externalApiService.GetSemesterByIdAsync(masterTopic.SemesterId.ToString());
                    if (semesterResponse == null)
                    {
                        _logger.LogWarning("Semester not found via external API for id: {SemesterId}", masterTopic.SemesterId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calling external API for Semester with id: {SemesterId}", masterTopic.SemesterId);
                }
            }

            // Get ALL lecturers from N:N relationship - Sử dụng navigation property
            var lecturerResponses = new List<LecturerResponse>();

            if (masterTopic.LecturerMasterTopics != null && masterTopic.LecturerMasterTopics.Any())
            {
                // Fetch lecturer details for each lecturer ID
                var lecturerTasks = masterTopic.LecturerMasterTopics.Select(async lmt =>
                {
                    try
                    {
                        var lecturer = await _externalApiService.GetLecturerByIdAsync(lmt.LecturerId.ToString());
                        if (lecturer == null)
                        {
                            _logger.LogWarning("Lecturer not found via external API for id: {LecturerId}", lmt.LecturerId);
                        }
                        return lecturer;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error calling external API for Lecturer with id: {LecturerId}", lmt.LecturerId);
                        return null;
                    }
                });

                var lecturers = await Task.WhenAll(lecturerTasks);
                lecturerResponses.AddRange(lecturers.Where(l => l != null)!);
            }
            else
            {
                _logger.LogInformation("No LecturerMasterTopics found for MasterTopic with id: {MasterTopicId}", masterTopic.Id);
            }

            var response = new MasterTopicResponse
            {
                Id = masterTopic.Id,
                Domain = domainResponse,
                Name = masterTopic.Name,
                Semester = new SemesterResponse
                {
                    Id = masterTopic.SemesterId,
                    Code = semesterResponse.Code,
                    Term = semesterResponse.Term,
                    Year = semesterResponse.Year,
                    Status = semesterResponse.Status,
                    StartDate = semesterResponse.StartDate,
                    EndDate = semesterResponse.EndDate,
                },
                Lecturers = new List<LecturerResponse> {
                    new LecturerResponse {
                        Id = lecturerResponses.First().Id,
                        FullName = lecturerResponses.First().FullName,
                        Gender = lecturerResponses.First().Gender,
                        Status = lecturerResponses.First().Status,
                    },
                }, // N:N relationship
                Description = masterTopic.Description,
                Status = masterTopic.Status,
                CreatedAt = masterTopic.CreatedAt,
            };
            return await Task.FromResult(response);
        }
    }
}