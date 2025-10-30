namespace DepartmentalSystemAPI.Models
{
    public class AssignmentHistory
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int TaskId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UnassignedDate { get; set; }
        public int HoursWorked { get; set; } = 0;
        public string Notes { get; set; }

        public Employee Employee { get; set; }
        public ProjectTask Task { get; set; }
    }
}