using Microsoft.AspNetCore.Mvc;
using DepartmentalSystemAPI.Services;
using DepartmentalSystemAPI.DTOs;
using DepartmentalSystemAPI.Models;

namespace DepartmentalSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDetailDto>> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpGet("{id}/skills")]
        public async Task<ActionResult<EmployeeDetailDto>> GetEmployeeWithSkills(int id)
        {
            var employee = await _employeeService.GetEmployeeWithSkillsAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpGet("workload")]
        public async Task<ActionResult<List<WorkloadDto>>> GetAllWorkloads()
        {
            var workloads = await _employeeService.GetAllEmployeesWorkloadAsync();
            return Ok(workloads);
        }

        [HttpGet("{id}/workload")]
        public async Task<ActionResult<WorkloadDto>> GetEmployeeWorkload(int id)
        {
            var workload = await _employeeService.GetEmployeeWorkloadAsync(id);
            if (workload == null) return NotFound();
            return Ok(workload);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(CreateEmployeeDto employeeDto)
        {
            var employee = await _employeeService.CreateEmployeeAsync(employeeDto);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        [HttpGet("available")]
        public async Task<ActionResult<List<EmployeeDto>>> GetAvailableEmployees()
        {
            var employees = await _employeeService.GetAvailableEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("skills")]
        public async Task<ActionResult<List<EmployeeDto>>> FindEmployeesBySkills([FromQuery] List<int> skillIds, [FromQuery] int minProficiency = 3)
        {
            var employees = await _employeeService.FindEmployeesBySkillsAsync(skillIds, minProficiency);
            return Ok(employees);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateEmployeeStatus(int id, [FromBody] string status)
        {
            if (Enum.TryParse<AvailabilityStatus>(status, true, out var availabilityStatus))
            {
                await _employeeService.UpdateEmployeeStatusAsync(id, availabilityStatus);
                return NoContent();
            }
            return BadRequest("Invalid status value.");
        }
    }
}