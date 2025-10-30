using DepartmentalSystemAPI.DTOs;
using DepartmentalSystemAPI.Models;

namespace DepartmentalSystemAPI.Services
{
    public interface IProjectService
    {
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
    }
}