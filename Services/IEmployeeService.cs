using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.DTOs;

namespace DepartmentalSystemAPI.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAllEmployeesAsync();
        Task<List<EmployeeDto>> GetAllAdEmployeesAsync();
        Task<List<EmployeeDto>> GetEmployeesByDepartmentAsync(string department);
        Task SyncAdEmployeesAsync();
        Task<EmployeeDetailDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeDetailDto> GetEmployeeWithSkillsAsync(int id);
        Task<WorkloadDto> GetEmployeeWorkloadAsync(int employeeId);
        Task<List<WorkloadDto>> GetAllEmployeesWorkloadAsync();
        Task<Employee> CreateEmployeeAsync(CreateEmployeeDto employeeDto);
        Task UpdateEmployeeWorkloadAsync(int employeeId);
        Task<List<EmployeeDto>> FindEmployeesBySkillsAsync(List<int> skillIds, int minProficiency = 3);
        Task<List<EmployeeDto>> GetAvailableEmployeesAsync();
        Task UpdateEmployeeStatusAsync(int employeeId, AvailabilityStatus status);

        // ADD THIS METHOD
        string GenerateEmailFromUserName(string userName);
    }
}