using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.DTOs;

namespace DepartmentalSystemAPI.Services
{
    public interface ISkillService
    {
        Task<List<SkillDto>> GetAllSkillsAsync();
        Task<List<SkillDto>> GetSkillsByCategoryAsync(string category);
        Task<Skill> CreateSkillAsync(Skill skill);
        Task<bool> DeleteSkillAsync(int id);
        Task<EmployeeSkill> AddEmployeeSkillAsync(int employeeId, AddSkillDto skillDto);
        Task<bool> RemoveEmployeeSkillAsync(int employeeId, int skillId);
        Task<List<EmployeeDto>> GetEmployeesBySkillAsync(int skillId, int minProficiency = 1);
        Task<List<SkillDto>> GetProjectRequiredSkillsAsync(int projectId);
        Task<List<EmployeeSkillDto>> GetEmployeeSkillsAsync(int employeeId);
    }
}