using Microsoft.EntityFrameworkCore;
using DepartmentalSystemAPI.Data;
using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.DTOs;
using Microsoft.Extensions.Logging;

namespace DepartmentalSystemAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly DepartmentContext _context;
        private readonly IEmployeeService _employeeService;
        private readonly IProjectService _projectService;
        private readonly ILogger<TaskService> _logger;

        public TaskService(DepartmentContext context, IEmployeeService employeeService, IProjectService projectService, ILogger<TaskService> logger)
        {
            _context = context;
            _employeeService = employeeService;
            _projectService = projectService;
            _logger = logger;
        }

        public async Task<List<ProjectTaskDto>> GetAllTasksAsync()
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Stage)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    ActualStartDate = t.ActualStartDate,
                    ActualEndDate = t.ActualEndDate,
                    DurationDays = t.DurationDays,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Progress = t.Progress,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    RemainingHours = t.RemainingHours,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    StageId = t.StageId,
                    StageName = t.Stage.Name,
                    PredecessorTaskId = t.PredecessorTaskId
                })
                .ToListAsync();
        }

        public async Task<List<ProjectTaskDto>> GetProjectTasksAsync(int projectId)
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Stage)
                .Where(t => t.ProjectId == projectId)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    ActualStartDate = t.ActualStartDate,
                    ActualEndDate = t.ActualEndDate,
                    DurationDays = t.DurationDays,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Progress = t.Progress,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    RemainingHours = t.RemainingHours,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    StageId = t.StageId,
                    StageName = t.Stage.Name,
                    PredecessorTaskId = t.PredecessorTaskId
                })
                .OrderBy(t => t.StartDate)
                .ToListAsync();
        }

        public async Task<List<ProjectTaskDto>> GetEmployeeTasksAsync(int employeeId)
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Stage)
                .Where(t => t.AssignedToId == employeeId)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    ActualStartDate = t.ActualStartDate,
                    ActualEndDate = t.ActualEndDate,
                    DurationDays = t.DurationDays,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Progress = t.Progress,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    RemainingHours = t.RemainingHours,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    StageId = t.StageId,
                    StageName = t.Stage.Name,
                    PredecessorTaskId = t.PredecessorTaskId
                })
                .OrderBy(t => t.EndDate)
                .ToListAsync();
        }

        public async Task<List<ProjectTaskDto>> GetEmployeeTasksByStatusAsync(int employeeId, ProjectTaskStatus status)
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Stage)
                .Where(t => t.AssignedToId == employeeId && t.Status == status)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    ActualStartDate = t.ActualStartDate,
                    ActualEndDate = t.ActualEndDate,
                    DurationDays = t.DurationDays,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Progress = t.Progress,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    RemainingHours = t.RemainingHours,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    StageId = t.StageId,
                    StageName = t.Stage.Name,
                    PredecessorTaskId = t.PredecessorTaskId
                })
                .ToListAsync();
        }

        public async Task<ProjectTaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Stage)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return null;

            return new ProjectTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                ActualStartDate = task.ActualStartDate,
                ActualEndDate = task.ActualEndDate,
                DurationDays = task.DurationDays,
                Priority = task.Priority.ToString(),
                Status = task.Status.ToString(),
                Progress = task.Progress,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                RemainingHours = task.RemainingHours,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedToId = task.AssignedToId,
                AssignedToName = task.AssignedTo != null ? $"{task.AssignedTo.FirstName} {task.AssignedTo.LastName}" : null,
                StageId = task.StageId,
                StageName = task.Stage.Name,
                PredecessorTaskId = task.PredecessorTaskId
            };
        }

        public async Task<ProjectTask> CreateTaskAsync(CreateTaskDto taskDto, int createdById)
        {
            var task = new ProjectTask
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                StartDate = taskDto.StartDate,
                EndDate = taskDto.EndDate,
                DurationDays = (int)(taskDto.EndDate - taskDto.StartDate).TotalDays,
                Priority = Enum.Parse<TaskPriority>(taskDto.Priority),
                EstimatedHours = taskDto.EstimatedHours,
                RemainingHours = taskDto.EstimatedHours,
                ProjectId = taskDto.ProjectId,
                AssignedToId = taskDto.AssignedToId,
                StageId = taskDto.StageId,
                PredecessorTaskId = taskDto.PredecessorTaskId,
                Status = ProjectTaskStatus.NotStarted,
                Progress = 0
            };

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();

            if (taskDto.AssignedToId.HasValue)
            {
                var assignment = new AssignmentHistory
                {
                    EmployeeId = taskDto.AssignedToId.Value,
                    TaskId = task.Id,
                    AssignedDate = DateTime.UtcNow,
                    Notes = "Initial assignment"
                };
                _context.AssignmentHistories.Add(assignment);

                await _employeeService.UpdateEmployeeWorkloadAsync(taskDto.AssignedToId.Value);
            }

            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<ProjectTaskDto> UpdateTaskAsync(int id, UpdateTaskDto taskDto, int updatedById)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return null;

            var oldAssignee = task.AssignedToId;

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.StartDate = taskDto.StartDate;
            task.EndDate = taskDto.EndDate;
            task.DurationDays = (int)(taskDto.EndDate - taskDto.StartDate).TotalDays;
            task.Priority = Enum.Parse<TaskPriority>(taskDto.Priority);
            task.EstimatedHours = taskDto.EstimatedHours;
            task.AssignedToId = taskDto.AssignedToId;
            task.StageId = taskDto.StageId;
            task.PredecessorTaskId = taskDto.PredecessorTaskId;

            if (oldAssignee != taskDto.AssignedToId && taskDto.AssignedToId.HasValue)
            {
                var oldAssignment = await _context.AssignmentHistories
                    .FirstOrDefaultAsync(ah => ah.EmployeeId == oldAssignee && ah.TaskId == id && ah.UnassignedDate == null);

                if (oldAssignment != null)
                {
                    oldAssignment.UnassignedDate = DateTime.UtcNow;
                }

                var newAssignment = new AssignmentHistory
                {
                    EmployeeId = taskDto.AssignedToId.Value,
                    TaskId = id,
                    AssignedDate = DateTime.UtcNow,
                    Notes = "Reassigned"
                };
                _context.AssignmentHistories.Add(newAssignment);

                if (oldAssignee.HasValue)
                    await _employeeService.UpdateEmployeeWorkloadAsync(oldAssignee.Value);
                await _employeeService.UpdateEmployeeWorkloadAsync(taskDto.AssignedToId.Value);
            }

            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(id);
        }

        public async Task<ProjectTaskDto> UpdateTaskProgressAsync(int taskId, int employeeId, double progress, string notes = null)
        {
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null) return null;

            task.Progress = Math.Max(0, Math.Min(100, progress));

            if (progress >= 100)
            {
                task.Status = ProjectTaskStatus.Completed;
                task.ActualEndDate = DateTime.UtcNow;
            }
            else if (progress > 0 && task.Status == ProjectTaskStatus.NotStarted)
            {
                task.Status = ProjectTaskStatus.InProgress;
                task.ActualStartDate ??= DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(notes))
            {
                var assignment = await _context.AssignmentHistories
                    .FirstOrDefaultAsync(ah => ah.EmployeeId == employeeId && ah.TaskId == taskId && ah.UnassignedDate == null);

                if (assignment != null)
                {
                    assignment.HoursWorked += 1;
                    assignment.Notes = notes;
                }

                task.ActualHours += 1;
                task.RemainingHours = Math.Max(0, task.EstimatedHours - task.ActualHours);
            }

            await _context.SaveChangesAsync();

            // FIXED: Use injected _projectService
            await _projectService.UpdateProjectProgressAsync(task.ProjectId);

            await _employeeService.UpdateEmployeeWorkloadAsync(employeeId);

            return await GetTaskByIdAsync(taskId);
        }

        public async Task<ProjectTaskDto> UpdateTaskStatusAsync(int taskId, int employeeId, ProjectTaskStatus status)
        {
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null) return null;

            task.Status = status;

            if (status == ProjectTaskStatus.Completed)
            {
                task.Progress = 100;
                task.ActualEndDate = DateTime.UtcNow;
            }
            else if (status == ProjectTaskStatus.InProgress && task.ActualStartDate == null)
            {
                task.ActualStartDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // FIXED: Use injected _projectService
            await _projectService.UpdateProjectProgressAsync(task.ProjectId);

            return await GetTaskByIdAsync(taskId);
        }

        public async Task<ProjectTaskDto> LogTaskHoursAsync(int taskId, int employeeId, int hours, string notes = null)
        {
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null) return null;

            var assignment = await _context.AssignmentHistories
                .FirstOrDefaultAsync(ah => ah.EmployeeId == employeeId && ah.TaskId == taskId && ah.UnassignedDate == null);

            if (assignment != null)
            {
                assignment.HoursWorked += hours;
                if (!string.IsNullOrEmpty(notes))
                    assignment.Notes = notes;
            }

            task.ActualHours += hours;
            task.RemainingHours = Math.Max(0, task.EstimatedHours - task.ActualHours);

            if (task.EstimatedHours > 0)
            {
                task.Progress = Math.Min(100, (double)task.ActualHours / task.EstimatedHours * 100);
            }

            if (task.Progress >= 100)
            {
                task.Status = ProjectTaskStatus.Completed;
                task.ActualEndDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // FIXED: Use injected _projectService
            await _projectService.UpdateProjectProgressAsync(task.ProjectId);

            await _employeeService.UpdateEmployeeWorkloadAsync(employeeId);

            return await GetTaskByIdAsync(taskId);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return false;

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProjectTaskDto>> GetOverdueTasksAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Stage)
                .Where(t => t.EndDate < today && t.Status != ProjectTaskStatus.Completed)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    ActualStartDate = t.ActualStartDate,
                    ActualEndDate = t.ActualEndDate,
                    DurationDays = t.DurationDays,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Progress = t.Progress,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    RemainingHours = t.RemainingHours,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    StageId = t.StageId,
                    StageName = t.Stage.Name,
                    PredecessorTaskId = t.PredecessorTaskId
                })
                .ToListAsync();
        }

        public async Task<List<ProjectTaskDto>> GetUpcomingTasksAsync()
        {
            var nextWeek = DateTime.UtcNow.AddDays(7).Date;
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Stage)
                .Where(t => t.EndDate <= nextWeek && t.Status != ProjectTaskStatus.Completed)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    ActualStartDate = t.ActualStartDate,
                    ActualEndDate = t.ActualEndDate,
                    DurationDays = t.DurationDays,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Progress = t.Progress,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    RemainingHours = t.RemainingHours,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    StageId = t.StageId,
                    StageName = t.Stage.Name,
                    PredecessorTaskId = t.PredecessorTaskId
                })
                .OrderBy(t => t.EndDate)
                .ToListAsync();
        }

        public async Task<List<ProjectTaskDto>> GetUpcomingEmployeeTasksAsync(int employeeId)
        {
            var nextWeek = DateTime.UtcNow.AddDays(7).Date;
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.Stage)
                .Where(t => t.AssignedToId == employeeId && t.EndDate <= nextWeek && t.Status != ProjectTaskStatus.Completed)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    ActualStartDate = t.ActualStartDate,
                    ActualEndDate = t.ActualEndDate,
                    DurationDays = t.DurationDays,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Progress = t.Progress,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    RemainingHours = t.RemainingHours,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    StageId = t.StageId,
                    StageName = t.Stage.Name,
                    PredecessorTaskId = t.PredecessorTaskId
                })
                .OrderBy(t => t.EndDate)
                .ToListAsync();
        }

        public async Task ReassignTaskAsync(int taskId, int newEmployeeId, int reassignedById)
        {
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null) return;

            var oldAssignee = task.AssignedToId;

            if (oldAssignee.HasValue)
            {
                var oldAssignment = await _context.AssignmentHistories
                    .FirstOrDefaultAsync(ah => ah.EmployeeId == oldAssignee && ah.TaskId == taskId && ah.UnassignedDate == null);

                if (oldAssignment != null)
                {
                    oldAssignment.UnassignedDate = DateTime.UtcNow;
                }
            }

            task.AssignedToId = newEmployeeId;

            var newAssignment = new AssignmentHistory
            {
                EmployeeId = newEmployeeId,
                TaskId = taskId,
                AssignedDate = DateTime.UtcNow,
                Notes = $"Reassigned by user {reassignedById}"
            };
            _context.AssignmentHistories.Add(newAssignment);

            await _context.SaveChangesAsync();

            if (oldAssignee.HasValue)
                await _employeeService.UpdateEmployeeWorkloadAsync(oldAssignee.Value);
            await _employeeService.UpdateEmployeeWorkloadAsync(newEmployeeId);
        }
    }
}