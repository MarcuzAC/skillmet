namespace DepartmentalSystemAPI.DTOs
{
    public class GanttChartDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<GanttTaskDto> Tasks { get; set; } = new();
        public List<GanttDependencyDto> Dependencies { get; set; } = new();
    }

    public class GanttTaskDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public double Progress { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public int? Parent { get; set; }
        public string Type { get; set; } = "task";
    }

    public class GanttDependencyDto
    {
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Type { get; set; } = "finish_to_start";
    }
}