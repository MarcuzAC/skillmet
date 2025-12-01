using DepartmentalSystemAPI.DTOs;
using DepartmentalSystemAPI.Models;

namespace DepartmentalSystemAPI.Services
{
    public interface IProjectService
    {
        // Existing methods
        Task<List<ProjectDto>> GetAllProjectsAsync();
        Task<List<ProjectDto>> GetProjectsByPriorityAsync();
        Task<List<ProjectDto>> GetProjectsByStatusAsync(string status);
        Task<ProjectDetailDto> GetProjectByIdAsync(int id);
        Task<GanttChartDto> GetProjectGanttChartAsync(int projectId);
        Task<Project> CreateProjectAsync(CreateProjectDto projectDto);
        Task<Project> UpdateProjectAsync(int id, CreateProjectDto projectDto);
        Task UpdateProjectProgressAsync(int projectId);
        Task<bool> DeleteProjectAsync(int id);
        Task<List<ProjectDto>> GetOverdueProjectsAsync();

        // NEW: Role-based Gantt chart methods
        Task<GanttChartDto> GetProjectGanttChartAsync(int projectId, int userId, string userRole);
        Task<GanttChartDto> GetPortfolioGanttChartAsync();
        Task<GanttChartDto> GetPersonalGanttChartAsync(int employeeId);

        // NEW: Project filtering methods
        Task<List<ProjectDto>> GetEmployeeProjectsAsync(int employeeId);
        Task<List<ProjectDto>> GetManagedProjectsAsync(string managerUserName);

        // NEW: Employee analytics methods
        Task<WorkloadDto> GetEmployeeWorkloadSummaryAsync(int employeeId);
        Task<List<EmployeeTaskDto>> GetUpcomingEmployeeDeadlinesAsync(int employeeId, int daysAhead = 7);
    }
}