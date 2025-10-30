using Microsoft.AspNetCore.Mvc;
using DepartmentalSystemAPI.Services;
using DepartmentalSystemAPI.DTOs;
using DepartmentalSystemAPI.Models;

namespace DepartmentalSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectTaskDto>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<ProjectTaskDto>>> GetProjectTasks(int projectId)
        {
            var tasks = await _taskService.GetProjectTasksAsync(projectId);
            return Ok(tasks);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<List<ProjectTaskDto>>> GetEmployeeTasks(int employeeId)
        {
            var tasks = await _taskService.GetEmployeeTasksAsync(employeeId);
            return Ok(tasks);
        }

        [HttpGet("employee/{employeeId}/status/{status}")]
        public async Task<ActionResult<List<ProjectTaskDto>>> GetEmployeeTasksByStatus(int employeeId, string status)
        {
            if (Enum.TryParse<ProjectTaskStatus>(status, true, out var taskStatus))
            {
                var tasks = await _taskService.GetEmployeeTasksByStatusAsync(employeeId, taskStatus);
                return Ok(tasks);
            }
            return BadRequest("Invalid status value.");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTaskDto>> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTask>> CreateTask(CreateTaskDto taskDto)
        {
            var createdById = 1;
            var task = await _taskService.CreateTaskAsync(taskDto, createdById);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectTaskDto>> UpdateTask(int id, UpdateTaskDto taskDto)
        {
            var updatedById = 1;
            var task = await _taskService.UpdateTaskAsync(id, taskDto, updatedById);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPut("{id}/progress")]
        public async Task<ActionResult<ProjectTaskDto>> UpdateTaskProgress(int id, [FromBody] UpdateTaskProgressDto progressDto)
        {
            var employeeId = 1;
            var task = await _taskService.UpdateTaskProgressAsync(id, employeeId, progressDto.Progress, progressDto.Notes);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPut("{id}/status/{status}")]
        public async Task<ActionResult<ProjectTaskDto>> UpdateTaskStatus(int id, string status)
        {
            if (Enum.TryParse<ProjectTaskStatus>(status, true, out var taskStatus))
            {
                var employeeId = 1;
                var task = await _taskService.UpdateTaskStatusAsync(id, employeeId, taskStatus);
                if (task == null) return NotFound();
                return Ok(task);
            }
            return BadRequest("Invalid status value.");
        }

        [HttpPut("{id}/hours")]
        public async Task<ActionResult<ProjectTaskDto>> LogTaskHours(int id, [FromQuery] int hours, [FromQuery] string notes = null)
        {
            var employeeId = 1;
            var task = await _taskService.LogTaskHoursAsync(id, employeeId, hours, notes);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<List<ProjectTaskDto>>> GetOverdueTasks()
        {
            var tasks = await _taskService.GetOverdueTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<List<ProjectTaskDto>>> GetUpcomingTasks()
        {
            var tasks = await _taskService.GetUpcomingTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("upcoming/employee/{employeeId}")]
        public async Task<ActionResult<List<ProjectTaskDto>>> GetUpcomingEmployeeTasks(int employeeId)
        {
            var tasks = await _taskService.GetUpcomingEmployeeTasksAsync(employeeId);
            return Ok(tasks);
        }

        [HttpPut("{id}/reassign/{newEmployeeId}")]
        public async Task<IActionResult> ReassignTask(int id, int newEmployeeId)
        {
            var reassignedById = 1;
            await _taskService.ReassignTaskAsync(id, newEmployeeId, reassignedById);
            return NoContent();
        }
    }
}