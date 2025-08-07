namespace BlazorControlPanel.Models;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;
    public ProjectType Type { get; set; } = ProjectType.Development;
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public Guid? ProjectManagerId { get; set; }
    public Staff? ProjectManager { get; set; }
    public List<Guid> TeamMemberIds { get; set; } = new();
    public List<Staff> TeamMembers { get; set; } = new();
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    public decimal ProgressPercentage { get; set; }
    public List<ProjectTask> Tasks { get; set; } = new();
    public List<ProjectMilestone> Milestones { get; set; } = new();
    public List<ProjectDocument> Documents { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    
    public bool IsOverdue => EndDate.HasValue && EndDate.Value < DateTime.UtcNow && Status != ProjectStatus.Completed && Status != ProjectStatus.Cancelled;
    public bool IsActive => Status == ProjectStatus.InProgress || Status == ProjectStatus.Planning;
    public int DaysRemaining => EndDate.HasValue ? Math.Max(0, (EndDate.Value - DateTime.UtcNow).Days) : 0;
    public int DurationDays => EndDate.HasValue ? (EndDate.Value - StartDate).Days : 0;
    public decimal BudgetVariance => ActualCost - Budget;
    public decimal BudgetVariancePercentage => Budget > 0 ? (BudgetVariance / Budget) * 100 : 0;
    public int HoursVariance => ActualHours - EstimatedHours;
    public decimal HoursVariancePercentage => EstimatedHours > 0 ? ((decimal)HoursVariance / EstimatedHours) * 100 : 0;
    public int CompletedTasksCount => Tasks.Count(t => t.Status == Models.TaskStatus.Completed);
    public int TotalTasksCount => Tasks.Count;
    public decimal TaskCompletionPercentage => TotalTasksCount > 0 ? ((decimal)CompletedTasksCount / TotalTasksCount) * 100 : 0;
}

public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Models.TaskStatus Status { get; set; } = Models.TaskStatus.NotStarted;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public Guid? AssignedToId { get; set; }
    public Staff? AssignedTo { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    public decimal ProgressPercentage { get; set; }
    public List<Guid> DependentTaskIds { get; set; } = new();
    public List<ProjectTask> DependentTasks { get; set; } = new();
    public List<TaskComment> Comments { get; set; } = new();
    public List<TaskAttachment> Attachments { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    
    public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != Models.TaskStatus.Completed;
    public bool IsActive => Status == Models.TaskStatus.InProgress;
    public int DaysRemaining => DueDate.HasValue ? Math.Max(0, (DueDate.Value - DateTime.UtcNow).Days) : 0;
    public int HoursVariance => ActualHours - EstimatedHours;
    public decimal HoursVariancePercentage => EstimatedHours > 0 ? ((decimal)HoursVariance / EstimatedHours) * 100 : 0;
}

public class ProjectMilestone
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool IsCompleted { get; set; }
    public List<Guid> RequiredTaskIds { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsOverdue => !IsCompleted && DueDate < DateTime.UtcNow;
    public int DaysRemaining => Math.Max(0, (DueDate - DateTime.UtcNow).Days);
}

public class ProjectDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DocumentCategory Category { get; set; } = DocumentCategory.General;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string UploadedBy { get; set; } = string.Empty;
}

public class TaskComment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TaskId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Staff? Author { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class TaskAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TaskId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string UploadedBy { get; set; } = string.Empty;
}

public class TimeEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public Guid? TaskId { get; set; }
    public ProjectTask? Task { get; set; }
    public Guid StaffId { get; set; }
    public Staff? Staff { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    public decimal Hours => (decimal)Duration.TotalHours;
    public string Description { get; set; } = string.Empty;
    public bool IsBillable { get; set; } = true;
    public decimal HourlyRate { get; set; }
    public decimal Amount => Hours * HourlyRate;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
}

public enum ProjectStatus
{
    Planning,
    InProgress,
    OnHold,
    Completed,
    Cancelled,
    Archived
}

public enum ProjectPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum ProjectType
{
    Development,
    Design,
    Consulting,
    Maintenance,
    Research,
    Marketing,
    Other
}

public enum TaskStatus
{
    NotStarted,
    InProgress,
    OnHold,
    Completed,
    Cancelled
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum DocumentCategory
{
    General,
    Requirements,
    Design,
    Technical,
    Testing,
    Deployment,
    Legal,
    Financial
}

public static class ProjectDefaults
{
    public static readonly Dictionary<ProjectStatus, string> StatusDescriptions = new()
    {
        { ProjectStatus.Planning, "Project is in planning phase" },
        { ProjectStatus.InProgress, "Project is actively being worked on" },
        { ProjectStatus.OnHold, "Project is temporarily paused" },
        { ProjectStatus.Completed, "Project has been completed successfully" },
        { ProjectStatus.Cancelled, "Project has been cancelled" },
        { ProjectStatus.Archived, "Project has been archived" }
    };

    public static readonly List<string> CommonTags = new()
    {
        "Web Development",
        "Mobile App",
        "E-commerce",
        "API Integration",
        "Database",
        "UI/UX Design",
        "Testing",
        "Deployment",
        "Maintenance",
        "Bug Fix",
        "Feature Enhancement",
        "Performance Optimization"
    };

    public static readonly List<string> DefaultMilestones = new()
    {
        "Project Kickoff",
        "Requirements Gathering",
        "Design Approval",
        "Development Phase 1",
        "Testing Phase",
        "User Acceptance Testing",
        "Deployment",
        "Project Completion"
    };
}
