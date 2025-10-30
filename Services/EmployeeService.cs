using Microsoft.EntityFrameworkCore;
using DepartmentalSystemAPI.Data;
using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.DTOs;

namespace DepartmentalSystemAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DepartmentContext _context;
        private readonly IAdService _adService;

        public EmployeeService(DepartmentContext context, IAdService adService)
        {
            _context = context;
            _adService = adService;
        }

        // ADD THIS METHOD FOR EMAIL GENERATION
        public string GenerateEmailFromUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            // Convert "milton chithenga" to "mchithenga@mra.mw"
            var nameParts = userName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length < 2)
                return null;

            var firstName = nameParts[0].ToLower();
            var lastName = nameParts[^1].ToLower();

            var emailPrefix = $"{firstName[0]}{lastName}";
            return $"{emailPrefix}@mra.mw";
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Position = e.Position,
                    Department = e.Department,
                    HireDate = e.HireDate,
                    Status = e.Status.ToString(),
                    CurrentWorkload = e.CurrentWorkload,
                    Skills = e.EmployeeSkills.Select(es => new EmployeeSkillDto
                    {
                        Id = es.Id,
                        SkillId = es.SkillId,
                        SkillName = es.Skill.Name,
                        Category = es.Skill.Category.ToString(),
                        ProficiencyLevel = es.ProficiencyLevel,
                        YearsOfExperience = es.YearsOfExperience,
                        LastUsed = es.LastUsed,
                        IsCertified = es.IsCertified
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<EmployeeDto>> GetAllAdEmployeesAsync()
        {
            // Get all users from Active Directory
            var adUsers = await _adService.GetAllUsersAsync();

            var employees = new List<EmployeeDto>();

            foreach (var adUser in adUsers)
            {
                // Find or create employee in local database
                var employee = await _context.Employees
                    .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill)
                    .FirstOrDefaultAsync(e => e.Email == adUser.Email ||
                                            (e.FirstName == adUser.FirstName && e.LastName == adUser.LastName));

                if (employee == null)
                {
                    // Create new employee from AD data
                    employee = new Employee
                    {
                        FirstName = adUser.FirstName,
                        LastName = adUser.LastName,
                        Email = adUser.Email,
                        Position = adUser.Title,
                        Department = adUser.Department,
                        HireDate = DateTime.UtcNow.Date,
                        Status = AvailabilityStatus.Available,
                        CurrentWorkload = 0
                    };

                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Update existing employee with latest AD data
                    employee.FirstName = adUser.FirstName;
                    employee.LastName = adUser.LastName;
                    employee.Email = adUser.Email;
                    employee.Position = adUser.Title;
                    employee.Department = adUser.Department;

                    await _context.SaveChangesAsync();
                }

                // Convert to DTO
                employees.Add(new EmployeeDto
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    Position = employee.Position,
                    Department = employee.Department,
                    HireDate = employee.HireDate,
                    Status = employee.Status.ToString(),
                    CurrentWorkload = employee.CurrentWorkload,
                    Skills = employee.EmployeeSkills.Select(es => new EmployeeSkillDto
                    {
                        Id = es.Id,
                        SkillId = es.SkillId,
                        SkillName = es.Skill.Name,
                        Category = es.Skill.Category.ToString(),
                        ProficiencyLevel = es.ProficiencyLevel,
                        YearsOfExperience = es.YearsOfExperience,
                        LastUsed = es.LastUsed,
                        IsCertified = es.IsCertified
                    }).ToList()
                });
            }

            return employees;
        }

        public async Task<EmployeeDetailDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                .Include(e => e.AssignedTasks)
                .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return null;

            return new EmployeeDetailDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Position = employee.Position,
                Department = employee.Department,
                HireDate = employee.HireDate,
                Status = employee.Status.ToString(),
                CurrentWorkload = employee.CurrentWorkload,
                Skills = employee.EmployeeSkills.Select(es => new EmployeeSkillDto
                {
                    Id = es.Id,
                    SkillId = es.SkillId,
                    SkillName = es.Skill.Name,
                    Category = es.Skill.Category.ToString(),
                    ProficiencyLevel = es.ProficiencyLevel,
                    YearsOfExperience = es.YearsOfExperience,
                    LastUsed = es.LastUsed,
                    IsCertified = es.IsCertified
                }).ToList(),
                CurrentTasks = employee.AssignedTasks.Where(t => t.Status != ProjectTaskStatus.Completed)
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
                    }).ToList()
            };
        }

        public async Task<EmployeeDetailDto> GetEmployeeWithSkillsAsync(int id)
        {
            return await GetEmployeeByIdAsync(id);
        }

        public async Task<WorkloadDto> GetEmployeeWorkloadAsync(int employeeId)
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

            return new WorkloadDto
            {
                EmployeeId = employee.Id,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                Position = employee.Position,
                CurrentWorkload = employee.CurrentWorkload,
                AssignedTasks = employee.AssignedTasks.Count(t => t.Status != ProjectTaskStatus.Completed),
                CompletedTasksThisWeek = completedTasksThisWeek,
                TotalHoursThisWeek = totalHoursThisWeek,
                AvailabilityStatus = employee.Status.ToString(),
                CurrentTasks = employee.AssignedTasks.Where(t => t.Status != ProjectTaskStatus.Completed)
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
                    }).ToList()
            };
        }

        public async Task<List<WorkloadDto>> GetAllEmployeesWorkloadAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.AssignedTasks)
                .ToListAsync();

            var workloads = new List<WorkloadDto>();
            var weekStart = DateTime.UtcNow.AddDays(-7);

            foreach (var employee in employees)
            {
                var completedTasksThisWeek = await _context.ProjectTasks
                    .CountAsync(t => t.AssignedToId == employee.Id &&
                                   t.Status == ProjectTaskStatus.Completed &&
                                   t.ActualEndDate >= weekStart);

                var totalHoursThisWeek = await _context.AssignmentHistories
                    .Where(ah => ah.EmployeeId == employee.Id && ah.AssignedDate >= weekStart)
                    .SumAsync(ah => ah.HoursWorked);

                workloads.Add(new WorkloadDto
                {
                    EmployeeId = employee.Id,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    Email = employee.Email,
                    Position = employee.Position,
                    CurrentWorkload = employee.CurrentWorkload,
                    AssignedTasks = employee.AssignedTasks.Count(t => t.Status != ProjectTaskStatus.Completed),
                    CompletedTasksThisWeek = completedTasksThisWeek,
                    TotalHoursThisWeek = totalHoursThisWeek,
                    AvailabilityStatus = employee.Status.ToString(),
                    CurrentTasks = employee.AssignedTasks.Where(t => t.Status != ProjectTaskStatus.Completed)
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
                        }).ToList()
                });
            }

            return workloads;
        }

        public async Task<Employee> CreateEmployeeAsync(CreateEmployeeDto employeeDto)
        {
            var employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Position = employeeDto.Position,
                Department = employeeDto.Department,
                HireDate = DateTime.UtcNow,
                Status = AvailabilityStatus.Available
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateEmployeeWorkloadAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.AssignedTasks)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee != null)
            {
                var totalEstimatedHours = employee.AssignedTasks
                    .Where(t => t.Status != ProjectTaskStatus.Completed)
                    .Sum(t => t.EstimatedHours);

                employee.CurrentWorkload = Math.Min(100, (decimal)totalEstimatedHours / employee.MaxWeeklyHours * 100);

                employee.Status = employee.CurrentWorkload switch
                {
                    >= 80 => AvailabilityStatus.Busy,
                    >= 60 => AvailabilityStatus.InMeeting,
                    _ => AvailabilityStatus.Available
                };

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<EmployeeDto>> FindEmployeesBySkillsAsync(List<int> skillIds, int minProficiency = 3)
        {
            return await _context.Employees
                .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                .Where(e => e.EmployeeSkills.Any(es =>
                    skillIds.Contains(es.SkillId) && es.ProficiencyLevel >= minProficiency))
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Position = e.Position,
                    Department = e.Department,
                    HireDate = e.HireDate,
                    Status = e.Status.ToString(),
                    CurrentWorkload = e.CurrentWorkload,
                    Skills = e.EmployeeSkills.Select(es => new EmployeeSkillDto
                    {
                        Id = es.Id,
                        SkillId = es.SkillId,
                        SkillName = es.Skill.Name,
                        Category = es.Skill.Category.ToString(),
                        ProficiencyLevel = es.ProficiencyLevel,
                        YearsOfExperience = es.YearsOfExperience,
                        LastUsed = es.LastUsed,
                        IsCertified = es.IsCertified
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<EmployeeDto>> GetAvailableEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                .Where(e => e.Status == AvailabilityStatus.Available && e.CurrentWorkload < 80)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Position = e.Position,
                    Department = e.Department,
                    HireDate = e.HireDate,
                    Status = e.Status.ToString(),
                    CurrentWorkload = e.CurrentWorkload,
                    Skills = e.EmployeeSkills.Select(es => new EmployeeSkillDto
                    {
                        Id = es.Id,
                        SkillId = es.SkillId,
                        SkillName = es.Skill.Name,
                        Category = es.Skill.Category.ToString(),
                        ProficiencyLevel = es.ProficiencyLevel,
                        YearsOfExperience = es.YearsOfExperience,
                        LastUsed = es.LastUsed,
                        IsCertified = es.IsCertified
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task UpdateEmployeeStatusAsync(int employeeId, AvailabilityStatus status)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                employee.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<EmployeeDto>> GetEmployeesByDepartmentAsync(string department)
        {
            return await _context.Employees
                .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                .Where(e => e.Department == department)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Position = e.Position,
                    Department = e.Department,
                    HireDate = e.HireDate,
                    Status = e.Status.ToString(),
                    CurrentWorkload = e.CurrentWorkload,
                    Skills = e.EmployeeSkills.Select(es => new EmployeeSkillDto
                    {
                        Id = es.Id,
                        SkillId = es.SkillId,
                        SkillName = es.Skill.Name,
                        Category = es.Skill.Category.ToString(),
                        ProficiencyLevel = es.ProficiencyLevel,
                        YearsOfExperience = es.YearsOfExperience,
                        LastUsed = es.LastUsed,
                        IsCertified = es.IsCertified
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task SyncAdEmployeesAsync()
        {
            var adUsers = await _adService.GetAllUsersAsync();

            foreach (var adUser in adUsers)
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == adUser.Email ||
                                            (e.FirstName == adUser.FirstName && e.LastName == adUser.LastName));

                if (employee == null)
                {
                    // Create new employee from AD data
                    employee = new Employee
                    {
                        FirstName = adUser.FirstName,
                        LastName = adUser.LastName,
                        Email = adUser.Email,
                        Position = adUser.Title,
                        Department = adUser.Department,
                        HireDate = DateTime.UtcNow.Date,
                        Status = AvailabilityStatus.Available,
                        CurrentWorkload = 0
                    };

                    _context.Employees.Add(employee);
                }
                else
                {
                    // Update existing employee with latest AD data
                    employee.FirstName = adUser.FirstName;
                    employee.LastName = adUser.LastName;
                    employee.Email = adUser.Email;
                    employee.Position = adUser.Title;
                    employee.Department = adUser.Department;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}