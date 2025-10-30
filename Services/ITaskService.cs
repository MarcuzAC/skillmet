using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.DTOs;

namespace DepartmentalSystemAPI.Services
{
    public interface ITaskService
    {
        Task<List<ProjectTaskDto>> GetAllTasksAsync();
        Task<List<ProjectTaskDto>> GetProjectTasksAsync(int projectId);
        Task<List<ProjectTaskDto>> GetEmployeeTasksAsync(int employeeId);
        Task<List<ProjectTaskDto>> GetEmployeeTasksByStatusAsync(int employeeId, ProjectTaskStatus status);
        Task<ProjectTaskDto> GetTaskByIdAsync(int id);
        Task<ProjectTask> CreateTaskAsync(CreateTaskDto taskDto, int createdById);
        Task<ProjectTaskDto> UpdateTaskAsync(int id, UpdateTaskDto taskDto, int updatedById);
        Task<ProjectTaskDto> UpdateTaskProgressAsync(int taskId, int employeeId, double progress, string notes = null);
        Task<ProjectTaskDto> UpdateTaskStatusAsync(int taskId, int employeeId, ProjectTaskStatus status);
        Task<ProjectTaskDto> LogTaskHoursAsync(int taskId, int employeeId, int hours, string notes = null);
        Task<bool> DeleteTaskAsync(int id);
        Task<List<ProjectTaskDto>> GetOverdueTasksAsync();
        Task<List<ProjectTaskDto>> GetUpcomingTasksAsync();
        Task<List<ProjectTaskDto>> GetUpcomingEmployeeTasksAsync(int employeeId);
        Task ReassignTaskAsync(int taskId, int newEmployeeId, int reassignedById);
    }
}