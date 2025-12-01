using Microsoft.EntityFrameworkCore;
using DepartmentalSystemAPI.Data;
using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.DTOs;
using DepartmentalSystemAPI.Services;

namespace DepartmentalSystemAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DepartmentContext _context;
        private readonly IEmailService _emailService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(DepartmentContext context, IEmailService emailService, IEmployeeService employeeService, ILogger<ProjectService> logger)
        {
            _context = context;
            _emailService = emailService;
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.Tasks)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Deadline = p.Deadline,
                    Priority = p.Priority.ToString(),
                    Status = p.Status.ToString(),
                    Progress = p.Progress,
                    EstimatedHours = p.EstimatedHours,
                    ActualHours = p.ActualHours,
                    TaskCount = p.Tasks.Count,
                    CompletedTasks = p.Tasks.Count(t => t.Status == ProjectTaskStatus.Completed),
                    ClientName = p.ClientName,
                    AssignedToUserName = p.AssignedToUserName
                })
                .OrderByDescending(p => p.Priority)
                .ThenByDescending(p => p.Deadline)
                .ToListAsync();
        }

        public async Task<List<ProjectDto>> GetProjectsByPriorityAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Tasks)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Deadline = p.Deadline,
                    Priority = p.Priority.ToString(),
                    Status = p.Status.ToString(),
                    Progress = p.Progress,
                    EstimatedHours = p.EstimatedHours,
                    ActualHours = p.ActualHours,
                    TaskCount = p.Tasks.Count,
                    CompletedTasks = p.Tasks.Count(t => t.Status == ProjectTaskStatus.Completed),
                    ClientName = p.ClientName,
                    AssignedToUserName = p.AssignedToUserName
                })
                .ToListAsync();

            return projects.OrderBy(p => GetPriorityOrder(p.Priority))
                          .ThenByDescending(p => p.Deadline)
                          .ToList();
        }

        private int GetPriorityOrder(string priority)
        {
            return priority?.ToLower() switch
            {
                "critical" => 1,
                "high" => 2,
                "medium" => 3,
                "low" => 4,
                _ => 5
            };
        }

        public async Task<List<ProjectDto>> GetProjectsByStatusAsync(string status)
        {
            if (Enum.TryParse<ProjectStatus>(status, true, out var projectStatus))
            {
                return await _context.Projects
                    .Include(p => p.Tasks)
                    .Where(p => p.Status == projectStatus)
                    .Select(p => new ProjectDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        Deadline = p.Deadline,
                        Priority = p.Priority.ToString(),
                        Status = p.Status.ToString(),
                        Progress = p.Progress,
                        EstimatedHours = p.EstimatedHours,
                        ActualHours = p.ActualHours,
                        TaskCount = p.Tasks.Count,
                        CompletedTasks = p.Tasks.Count(t => t.Status == ProjectTaskStatus.Completed),
                        ClientName = p.ClientName,
                        AssignedToUserName = p.AssignedToUserName
                    })
                    .ToListAsync();
            }
            return new List<ProjectDto>();
        }

        public async Task<ProjectDetailDto> GetProjectByIdAsync(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Stage)
                .Include(p => p.RequiredSkills)
                    .ThenInclude(ps => ps.Skill)
                .Include(p => p.Stages)
                    .ThenInclude(s => s.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return null;

            return new ProjectDetailDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Deadline = project.Deadline,
                Priority = project.Priority.ToString(),
                Status = project.Status.ToString(),
                Progress = project.Progress,
                Budget = project.Budget,
                EstimatedHours = project.EstimatedHours,
                ActualHours = project.ActualHours,
                ClientName = project.ClientName,
                AssignedToUserName = project.AssignedToUserName,
                Tasks = project.Tasks.Select(t => new ProjectTaskDto
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
                    ProjectName = project.Name,
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    StageId = t.StageId,
                    StageName = t.Stage.Name,
                    PredecessorTaskId = t.PredecessorTaskId
                }).ToList(),
                RequiredSkills = project.RequiredSkills.Select(ps => new ProjectSkillDto
                {
                    Id = ps.Id,
                    SkillId = ps.SkillId,
                    SkillName = ps.Skill.Name,
                    Category = ps.Skill.Category.ToString(),
                    RequiredProficiency = ps.RequiredProficiency,
                    Importance = ps.Importance
                }).ToList(),
                Stages = project.Stages.OrderBy(s => s.Order).Select(s => new StageDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Order = s.Order,
                    PlannedStartDate = s.PlannedStartDate,
                    PlannedEndDate = s.PlannedEndDate,
                    ActualStartDate = s.ActualStartDate,
                    ActualEndDate = s.ActualEndDate,
                    Progress = s.Progress,
                    IsMilestone = s.IsMilestone,
                    Tasks = s.Tasks.Select(t => new ProjectTaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        StartDate = t.StartDate,
                        EndDate = t.EndDate,
                        Priority = t.Priority.ToString(),
                        Status = t.Status.ToString(),
                        Progress = t.Progress,
                        EstimatedHours = t.EstimatedHours,
                        ActualHours = t.ActualHours
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<GanttChartDto> GetProjectGanttChartAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.PredecessorTask)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return null;

            var tasks = project.Tasks.Select(t => new GanttTaskDto
            {
                Id = t.Id,
                Text = t.Title,
                StartDate = t.StartDate,
                Duration = t.DurationDays,
                Progress = t.Progress / 100.0,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                AssignedTo = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : "Unassigned",
                Type = "task"
            }).ToList();

            var dependencies = project.Tasks
                .Where(t => t.PredecessorTaskId.HasValue)
                .Select(t => new GanttDependencyDto
                {
                    Id = t.Id,
                    From = t.PredecessorTaskId.Value,
                    To = t.Id,
                    Type = "finish_to_start"
                }).ToList();

            return new GanttChartDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                Tasks = tasks,
                Dependencies = dependencies
            };
        }

        // NEW: Role-based Gantt chart
        public async Task<GanttChartDto> GetProjectGanttChartAsync(int projectId, int userId, string userRole)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.PredecessorTask)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return null;

            // Filter tasks based on user role
            var tasksQuery = project.Tasks.AsQueryable();

            if (userRole == "Employee")
            {
                tasksQuery = tasksQuery.Where(t => t.AssignedToId == userId);
            }
            // Project Managers and Admins see all tasks

            var tasks = await tasksQuery
                .Select(t => new GanttTaskDto
                {
                    Id = t.Id,
                    Text = t.Title,
                    StartDate = t.StartDate,
                    Duration = t.DurationDays,
                    Progress = t.Progress / 100.0,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    AssignedTo = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : "Unassigned",
                    Type = "task"
                })
                .ToListAsync();

            // Only include dependencies for managers and admins
            var dependencies = userRole == "Employee" ?
                new List<GanttDependencyDto>() :
                project.Tasks
                    .Where(t => t.PredecessorTaskId.HasValue)
                    .Select(t => new GanttDependencyDto
                    {
                        Id = t.Id,
                        From = t.PredecessorTaskId.Value,
                        To = t.Id,
                        Type = "finish_to_start"
                    }).ToList();

            return new GanttChartDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                Tasks = tasks,
                Dependencies = dependencies
            };
        }

        // NEW: Portfolio Gantt Chart for Admins
        public async Task<GanttChartDto> GetPortfolioGanttChartAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.PredecessorTask)
                .ToListAsync();

            var portfolioTasks = projects.SelectMany(p => p.Tasks.Select(t => new GanttTaskDto
            {
                Id = t.Id,
                Text = $"{p.Name} - {t.Title}",
                StartDate = t.StartDate,
                Duration = t.DurationDays,
                Progress = t.Progress / 100.0,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                AssignedTo = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : "Unassigned",
                Type = "task"
            })).ToList();

            var dependencies = projects.SelectMany(p => p.Tasks
                .Where(t => t.PredecessorTaskId.HasValue)
                .Select(t => new GanttDependencyDto
                {
                    Id = t.Id,
                    From = t.PredecessorTaskId.Value,
                    To = t.Id,
                    Type = "finish_to_start"
                })).ToList();

            return new GanttChartDto
            {
                ProjectId = 0, // Portfolio view has no single project
                ProjectName = "Portfolio Overview",
                Tasks = portfolioTasks,
                Dependencies = dependencies
            };
        }

        // NEW: Personal Gantt Chart for Employees
        public async Task<GanttChartDto> GetPersonalGanttChartAsync(int employeeId)
        {
            var employeeTasks = await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.PredecessorTask)
                .Where(t => t.AssignedToId == employeeId)
                .OrderBy(t => t.StartDate)
                .ToListAsync();

            var tasks = employeeTasks.Select(t => new GanttTaskDto
            {
                Id = t.Id,
                Text = $"{t.Project.Name} - {t.Title}",
                StartDate = t.StartDate,
                Duration = t.DurationDays,
                Progress = t.Progress / 100.0,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                AssignedTo = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : "Unassigned",
                Type = "task"
            }).ToList();

            // Only include dependencies that are relevant to the employee
            var dependencies = employeeTasks
                .Where(t => t.PredecessorTaskId.HasValue &&
                           employeeTasks.Any(et => et.Id == t.PredecessorTaskId.Value))
                .Select(t => new GanttDependencyDto
                {
                    Id = t.Id,
                    From = t.PredecessorTaskId.Value,
                    To = t.Id,
                    Type = "finish_to_start"
                }).ToList();

            return new GanttChartDto
            {
                ProjectId = 0, // Personal view has no single project
                ProjectName = "My Tasks Overview",
                Tasks = tasks,
                Dependencies = dependencies
            };
        }

        // NEW: Get projects assigned to specific employee
        public async Task<List<ProjectDto>> GetEmployeeProjectsAsync(int employeeId)
        {
            return await _context.Projects
                .Include(p => p.Tasks)
                .Where(p => p.Tasks.Any(t => t.AssignedToId == employeeId))
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Deadline = p.Deadline,
                    Priority = p.Priority.ToString(),
                    Status = p.Status.ToString(),
                    Progress = p.Progress,
                    EstimatedHours = p.EstimatedHours,
                    ActualHours = p.ActualHours,
                    TaskCount = p.Tasks.Count,
                    CompletedTasks = p.Tasks.Count(t => t.Status == ProjectTaskStatus.Completed),
                    ClientName = p.ClientName,
                    AssignedToUserName = p.AssignedToUserName,
                    MyTaskCount = p.Tasks.Count(t => t.AssignedToId == employeeId),
                    MyCompletedTasks = p.Tasks.Count(t => t.AssignedToId == employeeId && t.Status == ProjectTaskStatus.Completed)
                })
                .ToListAsync();
        }

        // NEW: Get projects managed by specific user
        public async Task<List<ProjectDto>> GetManagedProjectsAsync(string managerUserName)
        {
            return await _context.Projects
                .Include(p => p.Tasks)
                .Where(p => p.AssignedToUserName == managerUserName)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Deadline = p.Deadline,
                    Priority = p.Priority.ToString(),
                    Status = p.Status.ToString(),
                    Progress = p.Progress,
                    EstimatedHours = p.EstimatedHours,
                    ActualHours = p.ActualHours,
                    TaskCount = p.Tasks.Count,
                    CompletedTasks = p.Tasks.Count(t => t.Status == ProjectTaskStatus.Completed),
                    ClientName = p.ClientName,
                    AssignedToUserName = p.AssignedToUserName
                })
                .ToListAsync();
        }

        // NEW: Get employee workload summary for personal dashboard
        public async Task<WorkloadDto> GetEmployeeWorkloadSummaryAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.AssignedTasks)
                .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) return null;

            var weekStart = DateTime.UtcNow.AddDays(-7);
            var completedTasksThisWeek = await _context.ProjectTasks
                .CountAsync(t => t.AssignedToId == employeeId &&
                               t.Status == ProjectTaskStatus.Completed &&
                               t.ActualEndDate >= weekStart);

            var totalHoursThisWeek = await _context.AssignmentHistories
                .Where(ah => ah.EmployeeId == employeeId && ah.AssignedDate >= weekStart)
                .SumAsync(ah => ah.HoursWorked);

            var currentTasks = employee.AssignedTasks
                .Where(t => t.Status != ProjectTaskStatus.Completed)
                .Select(t => new EmployeeTaskDto
                {
                    TaskId = t.Id,
                    TaskTitle = t.Title,
                    ProjectName = t.Project?.Name ?? "No Project",
                    DueDate = t.EndDate,
                    Priority = t.Priority.ToString(),
                    Progress = t.Progress,
                    EstimatedHours = t.EstimatedHours,
                    LoggedHours = t.ActualHours,
                    Status = t.Status.ToString()
                }).ToList();

            return new WorkloadDto
            {
                EmployeeId = employee.Id,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                Position = employee.Position,
                CurrentWorkload = employee.CurrentWorkload,
                AssignedTasks = employee.AssignedTasks.Count(t => t.Status != ProjectTaskStatus.Completed),
                CompletedTasksThisWeek = completedTasksThisWeek,
                TotalHoursThisWeek = (int)totalHoursThisWeek,
                AvailabilityStatus = employee.Status.ToString(),
                CurrentTasks = currentTasks
            };
        }

        // NEW: Get upcoming deadlines for employee
        public async Task<List<EmployeeTaskDto>> GetUpcomingEmployeeDeadlinesAsync(int employeeId, int daysAhead = 7)
        {
            var deadlineDate = DateTime.UtcNow.AddDays(daysAhead);

            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Where(t => t.AssignedToId == employeeId &&
                           t.Status != ProjectTaskStatus.Completed &&
                           t.EndDate <= deadlineDate)
                .OrderBy(t => t.EndDate)
                .Select(t => new EmployeeTaskDto
                {
                    TaskId = t.Id,
                    TaskTitle = t.Title,
                    ProjectName = t.Project.Name,
                    DueDate = t.EndDate,
                    Priority = t.Priority.ToString(),
                    Progress = t.Progress,
                    EstimatedHours = t.EstimatedHours,
                    LoggedHours = t.ActualHours,
                    Status = t.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task<Project> CreateProjectAsync(CreateProjectDto projectDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var project = new Project
                {
                    Name = projectDto.Name,
                    Description = projectDto.Description,
                    StartDate = projectDto.StartDate,
                    Deadline = projectDto.Deadline,
                    Priority = Enum.Parse<ProjectPriority>(projectDto.Priority),
                    Budget = projectDto.Budget,
                    EstimatedHours = projectDto.EstimatedHours,
                    ClientName = projectDto.ClientName,
                    Status = ProjectStatus.Planning,
                    Progress = 0,
                    ActualHours = 0,
                    AssignedToUserName = projectDto.AssignedToUserName
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                // Send email notification if project is assigned to someone
                if (!string.IsNullOrEmpty(projectDto.AssignedToUserName))
                {
                    await SendProjectAssignmentEmailAsync(project);
                }

                await transaction.CommitAsync();
                return project;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating project {ProjectName}", projectDto.Name);
                throw;
            }
        }

        public async Task<Project> UpdateProjectAsync(int id, CreateProjectDto projectDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null) return null;

                // Update project properties
                project.Name = projectDto.Name;
                project.Description = projectDto.Description;
                project.StartDate = projectDto.StartDate;
                project.Deadline = projectDto.Deadline;
                project.Priority = Enum.Parse<ProjectPriority>(projectDto.Priority);
                project.Budget = projectDto.Budget;
                project.EstimatedHours = projectDto.EstimatedHours;
                project.ClientName = projectDto.ClientName;
                project.AssignedToUserName = projectDto.AssignedToUserName;

                await _context.SaveChangesAsync();

                // Send email notification if project is assigned to someone
                if (!string.IsNullOrEmpty(projectDto.AssignedToUserName))
                {
                    await SendProjectAssignmentEmailAsync(project);
                }

                await transaction.CommitAsync();
                return project;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating project {ProjectId}", id);
                throw;
            }
        }

        public async Task UpdateProjectProgressAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project != null && project.Tasks.Any())
            {
                var totalProgress = project.Tasks.Average(t => t.Progress);
                var totalTasks = project.Tasks.Count;
                var completedTasks = project.Tasks.Count(t => t.Status == ProjectTaskStatus.Completed);

                project.Progress = totalProgress;

                if (completedTasks == totalTasks)
                {
                    project.Status = ProjectStatus.Completed;
                    project.EndDate = DateTime.UtcNow;
                }
                else if (project.Progress > 0)
                {
                    project.Status = ProjectStatus.InProgress;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProjectDto>> GetOverdueProjectsAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Projects
                .Include(p => p.Tasks)
                .Where(p => p.Deadline < today && p.Status != ProjectStatus.Completed)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Deadline = p.Deadline,
                    Priority = p.Priority.ToString(),
                    Status = p.Status.ToString(),
                    Progress = p.Progress,
                    EstimatedHours = p.EstimatedHours,
                    ActualHours = p.ActualHours,
                    TaskCount = p.Tasks.Count,
                    CompletedTasks = p.Tasks.Count(t => t.Status == ProjectTaskStatus.Completed),
                    ClientName = p.ClientName,
                    AssignedToUserName = p.AssignedToUserName
                })
                .ToListAsync();
        }

        private async Task SendProjectAssignmentEmailAsync(Project project)
        {
            try
            {
                // Generate email from username using EmployeeService
                var recipientEmail = _employeeService.GenerateEmailFromUserName(project.AssignedToUserName);

                if (string.IsNullOrEmpty(recipientEmail))
                {
                    _logger.LogWarning("Could not generate email for user: {UserName}", project.AssignedToUserName);
                    return;
                }

                var success = await _emailService.SendProjectAssignmentEmailAsync(
                    recipientName: project.AssignedToUserName,
                    recipientEmail: recipientEmail,
                    projectName: project.Name,
                    projectDescription: project.Description,
                    deadline: project.Deadline,
                    priority: project.Priority.ToString()
                );

                if (success)
                {
                    _logger.LogInformation("Project assignment email sent to {Email} for project {ProjectName}",
                        recipientEmail, project.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending project assignment email for project {ProjectName}", project.Name);
            }
        }
    }
}