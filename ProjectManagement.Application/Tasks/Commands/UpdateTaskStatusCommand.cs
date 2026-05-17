using MediatR;
using ProjectManagement.Application.Common.Caching;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Enums;
using ProjectManagement.Domain.Interfaces;

namespace ProjectManagement.Application.Tasks.Commands
{
    public record UpdateTaskStatusCommand(Guid TaskId, TaskItemStatus NewStatus) : ICommand<bool>;

    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, bool>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDashboardCache _dashboardCache;

        public UpdateTaskStatusCommandHandler(
            ITaskRepository taskRepository,
            IUnitOfWork unitOfWork,
            IDashboardCache dashboardCache)
        {
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
            _dashboardCache = dashboardCache;
        }

        public async Task<bool> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken)
                ?? throw new InvalidOperationException($"Task with ID {request.TaskId} not found.");

            task.UpdateStatus(request.NewStatus);

            _taskRepository.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _dashboardCache.Remove();

            return true;
        }
    }
}


