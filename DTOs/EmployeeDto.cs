namespace DepartmentalSystemAPI.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; }
        public decimal CurrentWorkload { get; set; }
        public List<EmployeeSkillDto> Skills { get; set; } = new();
    }

    public class EmployeeDetailDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; }
        public decimal CurrentWorkload { get; set; }
        public List<EmployeeSkillDto> Skills { get; set; } = new();
        public List<EmployeeTaskDto> CurrentTasks { get; set; } = new();
    }

    public class CreateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
    }
}