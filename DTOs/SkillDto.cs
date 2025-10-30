namespace DepartmentalSystemAPI.DTOs
{
    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int EmployeeCount { get; set; }
    }

    public class EmployeeSkillDto
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Category { get; set; }
        public int ProficiencyLevel { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime LastUsed { get; set; }
        public bool IsCertified { get; set; }
    }

    public class AddSkillDto
    {
        public int SkillId { get; set; }
        public int ProficiencyLevel { get; set; } = 1;
        public int YearsOfExperience { get; set; } = 0;
        public bool IsCertified { get; set; } = false;
    }
}