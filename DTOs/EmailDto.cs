namespace DepartmentalSystemAPI.DTOs.Email
{
    public class EmailMessageDto
    {
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = false;
        public List<EmailAttachmentDto> Attachments { get; set; } = new();
        public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    }

    public class EmailAttachmentDto
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }

    public class ProjectAssignmentEmailDto
    {
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public string ProjectManager { get; set; }
        public string ClientName { get; set; }
        public decimal Budget { get; set; }
        public int EstimatedHours { get; set; }
    }

    public class TaskAssignmentEmailDto
    {
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string ProjectName { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public string AssignedBy { get; set; }
    }

    public class ProjectUpdateEmailDto
    {
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string ProjectName { get; set; }
        public string UpdateType { get; set; } // StatusChange, DeadlineUpdate, ProgressUpdate, etc.
        public string PreviousValue { get; set; }
        public string NewValue { get; set; }
        public string UpdateMessage { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; }
    }

    public class SystemNotificationEmailDto
    {
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string NotificationType { get; set; } // Info, Warning, Error, Success
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; } = DateTime.Now;
        public string ActionRequired { get; set; }
        public DateTime? ActionDueDate { get; set; }
    }

    public class BulkEmailDto
    {
        public List<EmailRecipientDto> Recipients { get; set; } = new();
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = false;
        public List<EmailAttachmentDto> Attachments { get; set; } = new();
    }

    public class EmailRecipientDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class EmailResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string EmailId { get; set; }
        public DateTime SentAt { get; set; }
        public List<EmailErrorDto> Errors { get; set; } = new();
    }

    public class EmailErrorDto
    {
        public string RecipientEmail { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.Now;
    }

    public class EmailTemplateDto
    {
        public string TemplateName { get; set; }
        public string SubjectTemplate { get; set; }
        public string BodyTemplate { get; set; }
        public bool IsHtml { get; set; }
        public Dictionary<string, string> Placeholders { get; set; } = new();
    }

    // Enums
    public enum EmailPriority
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    public enum EmailStatus
    {
        Pending = 0,
        Sent = 1,
        Failed = 2,
        Delivered = 3
    }
}