using DepartmentalSystemAPI.DTOs;
using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DepartmentalSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // Existing endpoints
        [HttpGet]
        public async Task<ActionResult<List<ProjectDto>>> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("priority")]
        public async Task<ActionResult<List<ProjectDto>>> GetProjectsByPriority()
        {
            var projects = await _projectService.GetProjectsByPriorityAsync();
            return Ok(projects);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<ProjectDto>>> GetProjectsByStatus(string status)
        {
            var projects = await _projectService.GetProjectsByStatusAsync(status);
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDetailDto>> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpGet("{id}/gantt")]
        public async Task<ActionResult<GanttChartDto>> GetGanttChart(int id)
        {
            var ganttData = await _projectService.GetProjectGanttChartAsync(id);
            if (ganttData == null) return NotFound();
            return Ok(ganttData);
        }

        // NEW: Role-based Gantt chart
        [HttpGet("{id}/gantt/role")]
        public async Task<ActionResult<GanttChartDto>> GetProjectGanttWithRole(int id, [FromQuery] int userId, [FromQuery] string userRole)
        {
            var ganttData = await _projectService.GetProjectGanttChartAsync(id, userId, userRole);
            if (ganttData == null) return NotFound();
            return Ok(ganttData);
        }

        // NEW: Portfolio Gantt for Admins
        [HttpGet("gantt/portfolio")]
        public async Task<ActionResult<GanttChartDto>> GetPortfolioGantt()
        {
            var ganttData = await _projectService.GetPortfolioGanttChartAsync();
            return Ok(ganttData);
        }

        // NEW: Personal Gantt for Employees
        [HttpGet("gantt/personal/{employeeId}")]
        public async Task<ActionResult<GanttChartDto>> GetPersonalGantt(int employeeId)
        {
            var ganttData = await _projectService.GetPersonalGanttChartAsync(employeeId);
            return Ok(ganttData);
        }

        // NEW: Employee projects
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<List<ProjectDto>>> GetEmployeeProjects(int employeeId)
        {
            var projects = await _projectService.GetEmployeeProjectsAsync(employeeId);
            return Ok(projects);
        }

        // NEW: Managed projects
        [HttpGet("manager/{managerUserName}")]
        public async Task<ActionResult<List<ProjectDto>>> GetManagedProjects(string managerUserName)
        {
            var projects = await _projectService.GetManagedProjectsAsync(managerUserName);
            return Ok(projects);
        }

        // NEW: Employee workload summary
        [HttpGet("employee/{employeeId}/workload-summary")]
        public async Task<ActionResult<WorkloadDto>> GetEmployeeWorkloadSummary(int employeeId)
        {
            var workload = await _projectService.GetEmployeeWorkloadSummaryAsync(employeeId);
            if (workload == null) return NotFound();
            return Ok(workload);
        }

        // NEW: Employee upcoming deadlines
        [HttpGet("employee/{employeeId}/deadlines")]
        public async Task<ActionResult<List<EmployeeTaskDto>>> GetUpcomingEmployeeDeadlines(int employeeId, [FromQuery] int daysAhead = 7)
        {
            var deadlines = await _projectService.GetUpcomingEmployeeDeadlinesAsync(employeeId, daysAhead);
            return Ok(deadlines);
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(CreateProjectDto projectDto)
        {
            var project = await _projectService.CreateProjectAsync(projectDto);
            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Project>> UpdateProject(int id, CreateProjectDto projectDto)
        {
            var project = await _projectService.UpdateProjectAsync(id, projectDto);
            if (project == null) return NotFound();
            return Ok(project);
        }

        [HttpPut("{id}/progress")]
        public async Task<IActionResult> UpdateProjectProgress(int id)
        {
            await _projectService.UpdateProjectProgressAsync(id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var result = await _projectService.DeleteProjectAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<List<ProjectDto>>> GetOverdueProjects()
        {
            var projects = await _projectService.GetOverdueProjectsAsync();
            return Ok(projects);
        }
    }
}