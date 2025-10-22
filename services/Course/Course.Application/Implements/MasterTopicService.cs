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
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetByIdAsync called with empty id");
                throw new ArgumentException("Id must not be empty.", nameof(id));
            }

            // Eager-load LecturerMasterTopics using expression overload (correct signature)
            var repo = _unitOfWork.GetRepository<MasterTopic>();
            var masterTopic = await repo.GetByIdWithIncludeAsync(
                id,
                mt => mt.LecturerMasterTopics
            );

            // Fallback: try no-tracking fetch if needed
            if (masterTopic == null)
            {
                _logger.LogInformation("MasterTopic {Id} not found with include. Trying no-tracking fetch.", id);
                masterTopic = await repo.GetByIdAsync(id);
            }

            if (masterTopic == null)
            {
                _logger.LogError("Master topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Master topic not found");
            }

            // Build rich response (includes external API lookups inside)
            return await ToMasterTopicResponse(masterTopic);
        }

        public async Task<BasePaginatedList<MasterTopicResponse>> GetAllAsync(int page, int pageSize)
        {
            var masterTopics = await _unitOfWork.GetRepository<MasterTopic>()
                .GetAllAsync(includeProperties: "LecturerMasterTopics");

            var items = new List<MasterTopicResponse>(masterTopics.Count);
            foreach (var masterTopic in masterTopics)
            {
                items.Add(await ToMasterTopicResponse(masterTopic));
            }
            return new BasePaginatedList<MasterTopicResponse>(items, masterTopics.Count, page, pageSize);
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

            // Optional semester validation via external API (non-blocking on failure)
            if (request.SemesterId != Guid.Empty)
            {
                try
                {
                    var semester = await _externalApiService.GetSemesterByIdAsync(request.SemesterId.ToString());
                    if (semester == null)
                    {
                        _logger.LogWarning("Semester {SemesterId} not found, continuing creation.", request.SemesterId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Semester validation failed for {SemesterId}, continuing creation.", request.SemesterId);
                }
            }

            // Normalize lecturer list: distinct, remove empty Guids
            var lecturerIds = request.LecturerIds?
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var masterTopic = new MasterTopic
            {
                DomainId = request.DomainId,
                SemesterId = request.SemesterId,
                Name = request.Name,
                Description = request.Description,
            };

            if (lecturerIds != null && lecturerIds.Count > 0)
            {
                foreach (var lecturerId in lecturerIds)
                {
                    masterTopic.LecturerMasterTopics.Add(new LecturerMasterTopic
                    {
                        LecturerId = lecturerId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            // repository.InsertAsync already saves; avoid double SaveChanges
            await _unitOfWork.GetRepository<MasterTopic>().InsertAsync(masterTopic);

            // Reload with includes to ensure navigation data is populated
            var reloaded = await _unitOfWork
                .GetRepository<MasterTopic>()
                .GetByIdWithIncludeAsync(masterTopic.Id, mt => mt.LecturerMasterTopics);

            return await ToMasterTopicResponse(reloaded ?? masterTopic);
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
            if (domain == null)
            {
                _logger.LogError("Domain not found with id: {DomainId}", masterTopic.DomainId);
                throw new CustomExceptions.NoDataFoundException("Domain not found");
            }

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

            // Get ALL lecturers from N:N relationship
            var lecturerResponses = new List<LecturerResponse>();

            if (masterTopic.LecturerMasterTopics != null && masterTopic.LecturerMasterTopics.Any())
            {
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
                Semester = semesterResponse != null ? new SemesterResponse
                {
                    Id = masterTopic.SemesterId,
                    Code = semesterResponse.Code,
                    Term = semesterResponse.Term,
                    Year = semesterResponse.Year,
                    Status = semesterResponse.Status,
                    StartDate = semesterResponse.StartDate,
                    EndDate = semesterResponse.EndDate,
                } : null,
                Lecturers = lecturerResponses.Select(l => new LecturerResponse
                {
                    LecturerId = l.LecturerId,
                    FullName = l.FullName,
                    Gender = l.Gender,
                    Status = l.Status,
                }).ToList(),
                Description = masterTopic.Description,
                Status = masterTopic.Status,
                CreatedAt = masterTopic.CreatedAt,
            };
            return await Task.FromResult(response);
        }
    }
}