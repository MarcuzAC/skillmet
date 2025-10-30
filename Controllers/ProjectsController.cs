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