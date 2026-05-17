using MediatR;
using ProjectManagement.Application.Common.Caching;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Tasks.Commands
{
    public record CreateTaskCommand(
        Guid ProjectId,
        string Title,
        string Description,
        Guid AssigneeId
    ) : ICommand<Guid>;

    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDashboardCache _dashboardCache;

        public CreateTaskCommandHandler(
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IDashboardCache dashboardCache)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _dashboardCache = dashboardCache;
        }

        public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdWithTasksAsync(request.ProjectId, cancellationToken)
                ?? throw new KeyNotFoundException($"Project with id {request.ProjectId} not found.");

            var assignee = await _userRepository.GetByIdAsync(request.AssigneeId, cancellationToken)
                ?? throw new KeyNotFoundException($"User with id {request.AssigneeId} not found.");

            if (!assignee.IsEmployee)
            {
                throw new InvalidOperationException($"User with id {request.AssigneeId} is not an employee and cannot be assigned tasks.");
            }

            var newTask = project.AddTask(request.Title.Trim(), request.Description?.Trim() ?? string.Empty, request.AssigneeId);
            await _unitOfWork.SaveChangesAsync();
            _dashboardCache.Remove();

            return newTask.Id;
        }
    }
}


