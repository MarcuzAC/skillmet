using Microsoft.EntityFrameworkCore;
using DepartmentalSystemAPI.Data;
using DepartmentalSystemAPI.Models;
using DepartmentalSystemAPI.DTOs;

namespace DepartmentalSystemAPI.Services
{
    public class SkillService : ISkillService
    {
        private readonly DepartmentContext _context;

        public SkillService(DepartmentContext context)
        {
            _context = context;
        }

        public async Task<List<SkillDto>> GetAllSkillsAsync()
        {
            return await _context.Skills
                .Select(s => new SkillDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Category = s.Category.ToString(),
                    EmployeeCount = s.EmployeeSkills.Count
                })
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<List<SkillDto>> GetSkillsByCategoryAsync(string category)
        {
            if (Enum.TryParse<SkillCategory>(category, true, out var skillCategory))
            {
                return await _context.Skills
                    .Where(s => s.Category == skillCategory)
                    .Select(s => new SkillDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        Category = s.Category.ToString(),
                        EmployeeCount = s.EmployeeSkills.Count
                    })
                    .ToListAsync();
            }
            return new List<SkillDto>();
        }
        public async Task<List<EmployeeSkillDto>> GetEmployeeSkillsAsync(int employeeId)
        {
            var employeeSkills = await _context.EmployeeSkills
                .Include(es => es.Skill)
                .Include(es => es.Employee)
                .Where(es => es.EmployeeId == employeeId)
                .Select(es => new EmployeeSkillDto
                {
                    Id = es.Id,
                    EmployeeId = es.EmployeeId,
                    SkillId = es.SkillId,
                    SkillName = es.Skill.Name,
                    Category = es.Skill.Category.ToString(),
                    ProficiencyLevel = es.ProficiencyLevel,
                    YearsOfExperience = es.YearsOfExperience,
                    LastUsed = es.LastUsed,
                    IsCertified = es.IsCertified
                })
                .ToListAsync();

            return employeeSkills;
        }

        public async Task<Skill> CreateSkillAsync(Skill skill)
        {
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return skill;
        }

        public async Task<bool> DeleteSkillAsync(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return false;

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EmployeeSkill> AddEmployeeSkillAsync(int employeeId, AddSkillDto skillDto)
        {
            var existingSkill = await _context.EmployeeSkills
                .FirstOrDefaultAsync(es => es.EmployeeId == employeeId && es.SkillId == skillDto.SkillId);

            if (existingSkill != null)
            {
                existingSkill.ProficiencyLevel = skillDto.ProficiencyLevel;
                existingSkill.YearsOfExperience = skillDto.YearsOfExperience;
                existingSkill.IsCertified = skillDto.IsCertified;
                existingSkill.LastUsed = DateTime.UtcNow;
            }
            else
            {
                existingSkill = new EmployeeSkill
                {
                    EmployeeId = employeeId,
                    SkillId = skillDto.SkillId,
                    ProficiencyLevel = skillDto.ProficiencyLevel,
                    YearsOfExperience = skillDto.YearsOfExperience,
                    IsCertified = skillDto.IsCertified,
                    LastUsed = DateTime.UtcNow
                };
                _context.EmployeeSkills.Add(existingSkill);
            }

            await _context.SaveChangesAsync();
            return existingSkill;
        }

        public async Task<bool> RemoveEmployeeSkillAsync(int employeeId, int skillId)
        {
            var employeeSkill = await _context.EmployeeSkills
                .FirstOrDefaultAsync(es => es.EmployeeId == employeeId && es.SkillId == skillId);

            if (employeeSkill == null) return false;

            _context.EmployeeSkills.Remove(employeeSkill);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<EmployeeDto>> GetEmployeesBySkillAsync(int skillId, int minProficiency = 1)
        {
            return await _context.Employees
                .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                .Where(e => e.EmployeeSkills.Any(es =>
                    es.SkillId == skillId && es.ProficiencyLevel >= minProficiency))
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Position = e.Position,
                    Department = e.Department,
                    HireDate = e.HireDate,
                    Status = e.Status.ToString(),
                    CurrentWorkload = e.CurrentWorkload,
                    Skills = e.EmployeeSkills.Select(es => new EmployeeSkillDto
                    {
                        Id = es.Id,
                        SkillId = es.SkillId,
                        SkillName = es.Skill.Name,
                        Category = es.Skill.Category.ToString(),
                        ProficiencyLevel = es.ProficiencyLevel,
                        YearsOfExperience = es.YearsOfExperience,
                        LastUsed = es.LastUsed,
                        IsCertified = es.IsCertified
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<SkillDto>> GetProjectRequiredSkillsAsync(int projectId)
        {
            return await _context.ProjectSkills
                .Include(ps => ps.Skill)
                .Where(ps => ps.ProjectId == projectId)
                .Select(ps => new SkillDto
                {
                    Id = ps.SkillId,
                    Name = ps.Skill.Name,
                    Description = ps.Skill.Description,
                    Category = ps.Skill.Category.ToString(),
                    EmployeeCount = ps.Skill.EmployeeSkills.Count
                })
                .ToListAsync();
        }
    }
}