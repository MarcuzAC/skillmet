using Microsoft.AspNetCore.Mvc;
using DepartmentalSystemAPI.Services;
using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.DTOs;

namespace DepartmentalSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SkillDto>>> GetAllSkills()
        {
            var skills = await _skillService.GetAllSkillsAsync();
            return Ok(skills);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<SkillDto>>> GetSkillsByCategory(string category)
        {
            var skills = await _skillService.GetSkillsByCategoryAsync(category);
            return Ok(skills);
        }

        [HttpPost]
        public async Task<ActionResult<Skill>> CreateSkill(Skill skill)
        {
            var createdSkill = await _skillService.CreateSkillAsync(skill);
            return CreatedAtAction(nameof(GetAllSkills), createdSkill);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var result = await _skillService.DeleteSkillAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("employee/{employeeId}")]
        public async Task<ActionResult<EmployeeSkill>> AddEmployeeSkill(int employeeId, AddSkillDto skillDto)
        {
            var employeeSkill = await _skillService.AddEmployeeSkillAsync(employeeId, skillDto);
            return Ok(employeeSkill);
        }

        [HttpDelete("employee/{employeeId}/{skillId}")]
        public async Task<IActionResult> RemoveEmployeeSkill(int employeeId, int skillId)
        {
            var result = await _skillService.RemoveEmployeeSkillAsync(employeeId, skillId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{skillId}/employees")]
        public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesBySkill(int skillId, [FromQuery] int minProficiency = 1)
        {
            var employees = await _skillService.GetEmployeesBySkillAsync(skillId, minProficiency);
            return Ok(employees);
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<SkillDto>>> GetProjectRequiredSkills(int projectId)
        {
            var skills = await _skillService.GetProjectRequiredSkillsAsync(projectId);
            return Ok(skills);
        }
    }
}