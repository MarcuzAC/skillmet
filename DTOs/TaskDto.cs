namespace DepartmentalSystemAPI.DTOs
{
    public class ProjectTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int DurationDays { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double Progress { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public int RemainingHours { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public int StageId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public int? PredecessorTaskId { get; set; }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public int EstimatedHours { get; set; }
        public int ProjectId { get; set; }
        public int? AssignedToId { get; set; }
        public int StageId { get; set; }
        public int? PredecessorTaskId { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public int EstimatedHours { get; set; }
        public int? AssignedToId { get; set; }
        public int StageId { get; set; }
        public int? PredecessorTaskId { get; set; }
    }

    public class UpdateTaskProgressDto
    {
        public double Progress { get; set; }
        public int HoursWorked { get; set; }
        public string? Notes { get; set; }
    }
}