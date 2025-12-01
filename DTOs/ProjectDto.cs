namespace DepartmentalSystemAPI.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTasks { get; set; }
        public string ClientName { get; set; }
        public string AssignedToUserName { get; set; }

        // Add these properties for employee-specific project views
        public int MyTaskCount { get; set; }
        public int MyCompletedTasks { get; set; }
    }

    public class ProjectDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }
        public decimal Budget { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public string ClientName { get; set; }
        public string AssignedToUserName { get; set; }
        public List<ProjectTaskDto> Tasks { get; set; } = new();
        public List<ProjectSkillDto> RequiredSkills { get; set; } = new();
        public List<StageDto> Stages { get; set; } = new();
    }

    public class CreateProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; }
        public decimal Budget { get; set; }
        public int EstimatedHours { get; set; }
        public string ClientName { get; set; }
        public string AssignedToUserName { get; set; }
    }
}