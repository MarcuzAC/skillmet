namespace DepartmentalSystemAPI.DTOs
{
    public class WorkloadDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public decimal CurrentWorkload { get; set; }
        public int AssignedTasks { get; set; }
        public int CompletedTasksThisWeek { get; set; }
        public int TotalHoursThisWeek { get; set; }
        public string AvailabilityStatus { get; set; }
        public List<EmployeeTaskDto> CurrentTasks { get; set; } = new();
    }

    public class EmployeeTaskDto
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string ProjectName { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public double Progress { get; set; }
        public int EstimatedHours { get; set; }
        public int LoggedHours { get; set; }
        public string Status { get; set; }
    }
}