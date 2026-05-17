using MediatR;
using ProjectManagement.Application.Common.Caching;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Projects.Commands
{
    public record CreateProjectCommand(
        string Name,
        string Description,
        Guid OwnerId
    ) : ICommand<Guid>;

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDashboardCache _dashboardCache;

        public CreateProjectCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IDashboardCache dashboardCache)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _dashboardCache = dashboardCache;
        }

        public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Project name cannot be empty.", nameof(request.Name));
            }

            if (request.OwnerId == Guid.Empty)
            {
                throw new ArgumentException("OwnerId cannot be empty.", nameof(request.OwnerId));
            }

            var newProject = new Project(
                Guid.NewGuid(),
                request.Name.Trim(),
                request.Description?.Trim() ?? string.Empty,
                request.OwnerId
            );

            _projectRepository.Add(newProject);
            await _unitOfWork.SaveChangesAsync();
            _dashboardCache.Remove();

            return newProject.Id;
        }
    }
}


