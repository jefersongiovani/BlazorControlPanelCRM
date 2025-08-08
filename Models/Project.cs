/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

namespace BlazorControlPanel.Models;

/// <summary>
/// Project entity representing a work engagement or initiative for a customer.
/// Contains comprehensive project information including timeline, budget, resources,
/// and progress tracking for effective project management and delivery.
/// </summary>
/// <remarks>
/// Central entity for project management, used for planning, execution tracking,
/// resource allocation, and performance monitoring. Supports task management,
/// milestone tracking, and team collaboration features.
/// </remarks>
public class Project
{
    /// <summary>
    /// Unique identifier for the project record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Project name or title for identification and display
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the project scope and objectives
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Unique project code or identifier for reference and tracking
    /// </summary>
    public string ProjectCode { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the project in its lifecycle
    /// </summary>
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

    /// <summary>
    /// Priority level assigned to this project for resource allocation
    /// </summary>
    public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

    /// <summary>
    /// Type or category of the project (Development, Design, Consulting, etc.)
    /// </summary>
    public ProjectType Type { get; set; } = ProjectType.Development;

    /// <summary>
    /// Identifier of the customer this project is being delivered for
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Customer this project is being delivered for (navigation property)
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// Identifier of the staff member managing this project
    /// </summary>
    public Guid? ProjectManagerId { get; set; }

    /// <summary>
    /// Staff member managing this project (navigation property)
    /// </summary>
    public Staff? ProjectManager { get; set; }

    /// <summary>
    /// Collection of staff member identifiers assigned to work on this project
    /// </summary>
    public List<Guid> TeamMemberIds { get; set; } = new();

    /// <summary>
    /// Collection of staff members assigned to work on this project (navigation property)
    /// </summary>
    public List<Staff> TeamMembers { get; set; } = new();

    /// <summary>
    /// Planned start date for the project
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Planned end date for the project completion
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Actual date when work on the project began
    /// </summary>
    public DateTime? ActualStartDate { get; set; }

    /// <summary>
    /// Actual date when the project was completed
    /// </summary>
    public DateTime? ActualEndDate { get; set; }

    /// <summary>
    /// Allocated budget for the project
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// Actual costs incurred on the project to date
    /// </summary>
    public decimal ActualCost { get; set; }

    /// <summary>
    /// Estimated total hours required to complete the project
    /// </summary>
    public int EstimatedHours { get; set; }

    /// <summary>
    /// Actual hours spent on the project to date
    /// </summary>
    public int ActualHours { get; set; }

    /// <summary>
    /// Current completion percentage of the project (0-100)
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Collection of tasks that make up this project
    /// </summary>
    public List<ProjectTask> Tasks { get; set; } = new();

    /// <summary>
    /// Collection of milestones for tracking project progress
    /// </summary>
    public List<ProjectMilestone> Milestones { get; set; } = new();

    /// <summary>
    /// Collection of documents associated with this project
    /// </summary>
    public List<ProjectDocument> Documents { get; set; } = new();

    /// <summary>
    /// Collection of tags for categorizing and organizing projects
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Additional notes and observations about the project
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the project record was initially created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the project record was last modified
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifier of the user who created this project record
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the user who last updated this project record
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Computed property indicating whether the project is past its planned end date
    /// </summary>
    public bool IsOverdue => EndDate.HasValue && EndDate.Value < DateTime.UtcNow && Status != ProjectStatus.Completed && Status != ProjectStatus.Cancelled;

    /// <summary>
    /// Computed property indicating whether the project is currently active
    /// </summary>
    public bool IsActive => Status == ProjectStatus.InProgress || Status == ProjectStatus.Planning;

    /// <summary>
    /// Computed property returning the number of days remaining until the planned end date
    /// </summary>
    public int DaysRemaining => EndDate.HasValue ? Math.Max(0, (EndDate.Value - DateTime.UtcNow).Days) : 0;

    /// <summary>
    /// Computed property returning the total planned duration of the project in days
    /// </summary>
    public int DurationDays => EndDate.HasValue ? (EndDate.Value - StartDate).Days : 0;

    /// <summary>
    /// Computed property returning the budget variance (positive = over budget, negative = under budget)
    /// </summary>
    public decimal BudgetVariance => ActualCost - Budget;

    /// <summary>
    /// Computed property returning the budget variance as a percentage
    /// </summary>
    public decimal BudgetVariancePercentage => Budget > 0 ? (BudgetVariance / Budget) * 100 : 0;

    /// <summary>
    /// Computed property returning the hours variance (positive = over estimate, negative = under estimate)
    /// </summary>
    public int HoursVariance => ActualHours - EstimatedHours;

    /// <summary>
    /// Computed property returning the hours variance as a percentage
    /// </summary>
    public decimal HoursVariancePercentage => EstimatedHours > 0 ? ((decimal)HoursVariance / EstimatedHours) * 100 : 0;

    /// <summary>
    /// Computed property returning the number of completed tasks in this project
    /// </summary>
    public int CompletedTasksCount => Tasks.Count(t => t.Status == Models.TaskStatus.Completed);

    /// <summary>
    /// Computed property returning the total number of tasks in this project
    /// </summary>
    public int TotalTasksCount => Tasks.Count;

    /// <summary>
    /// Computed property returning the task completion percentage for this project
    /// </summary>
    public decimal TaskCompletionPercentage => TotalTasksCount > 0 ? ((decimal)CompletedTasksCount / TotalTasksCount) * 100 : 0;
}

/// <summary>
/// Project task entity representing individual work items within a project.
/// Contains task details, assignment information, progress tracking, and dependencies
/// for granular project management and execution control.
/// </summary>
/// <remarks>
/// Essential for breaking down projects into manageable work units, tracking progress,
/// managing dependencies, and ensuring accountability through task assignment and monitoring.
/// </remarks>
public class ProjectTask
{
    /// <summary>
    /// Unique identifier for the task record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the project this task belongs to
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Project this task belongs to (navigation property)
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// Task title or name for identification and display
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the task requirements and objectives
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the task in its lifecycle
    /// </summary>
    public Models.TaskStatus Status { get; set; } = Models.TaskStatus.NotStarted;

    /// <summary>
    /// Priority level assigned to this task for work prioritization
    /// </summary>
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    /// <summary>
    /// Identifier of the staff member assigned to complete this task
    /// </summary>
    public Guid? AssignedToId { get; set; }

    /// <summary>
    /// Staff member assigned to complete this task (navigation property)
    /// </summary>
    public Staff? AssignedTo { get; set; }

    /// <summary>
    /// Planned start date for the task
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Due date for task completion
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Actual date when the task was completed
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Estimated hours required to complete the task
    /// </summary>
    public int EstimatedHours { get; set; }

    /// <summary>
    /// Actual hours spent on the task to date
    /// </summary>
    public int ActualHours { get; set; }

    /// <summary>
    /// Current completion percentage of the task (0-100)
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Collection of task identifiers that this task depends on
    /// </summary>
    public List<Guid> DependentTaskIds { get; set; } = new();

    /// <summary>
    /// Collection of tasks that this task depends on (navigation property)
    /// </summary>
    public List<ProjectTask> DependentTasks { get; set; } = new();

    /// <summary>
    /// Collection of comments and discussions related to this task
    /// </summary>
    public List<TaskComment> Comments { get; set; } = new();

    /// <summary>
    /// Collection of files and documents attached to this task
    /// </summary>
    public List<TaskAttachment> Attachments { get; set; } = new();

    /// <summary>
    /// Collection of tags for categorizing and organizing tasks
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Timestamp when the task record was initially created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the task record was last modified
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifier of the user who created this task record
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the user who last updated this task record
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Computed property indicating whether the task is past its due date
    /// </summary>
    public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != Models.TaskStatus.Completed;

    /// <summary>
    /// Computed property indicating whether the task is currently being worked on
    /// </summary>
    public bool IsActive => Status == Models.TaskStatus.InProgress;

    /// <summary>
    /// Computed property returning the number of days remaining until the due date
    /// </summary>
    public int DaysRemaining => DueDate.HasValue ? Math.Max(0, (DueDate.Value - DateTime.UtcNow).Days) : 0;

    /// <summary>
    /// Computed property returning the hours variance (positive = over estimate, negative = under estimate)
    /// </summary>
    public int HoursVariance => ActualHours - EstimatedHours;

    /// <summary>
    /// Computed property returning the hours variance as a percentage
    /// </summary>
    public decimal HoursVariancePercentage => EstimatedHours > 0 ? ((decimal)HoursVariance / EstimatedHours) * 100 : 0;
}

/// <summary>
/// Project milestone entity representing significant checkpoints or deliverables within a project.
/// Used for tracking major project phases and ensuring timely completion of key objectives.
/// </summary>
/// <remarks>
/// Essential for project planning and monitoring, providing clear targets for project teams
/// and stakeholders to track progress against major deliverables and deadlines.
/// </remarks>
public class ProjectMilestone
{
    /// <summary>
    /// Unique identifier for the milestone record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the project this milestone belongs to
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Milestone name or title for identification and display
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the milestone deliverables and criteria
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Target date for milestone completion
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Actual date when the milestone was completed
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Indicates whether the milestone has been completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Collection of task identifiers that must be completed for this milestone
    /// </summary>
    public List<Guid> RequiredTaskIds { get; set; } = new();

    /// <summary>
    /// Timestamp when the milestone record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Computed property indicating whether the milestone is past its due date
    /// </summary>
    public bool IsOverdue => !IsCompleted && DueDate < DateTime.UtcNow;

    /// <summary>
    /// Computed property returning the number of days remaining until the due date
    /// </summary>
    public int DaysRemaining => Math.Max(0, (DueDate - DateTime.UtcNow).Days);
}

/// <summary>
/// Project document entity representing files and documents associated with a project.
/// Used for storing project-related documentation, specifications, and deliverables.
/// </summary>
/// <remarks>
/// Important for document management, version control, and ensuring all project
/// stakeholders have access to relevant project documentation and files.
/// </remarks>
public class ProjectDocument
{
    /// <summary>
    /// Unique identifier for the document record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the project this document belongs to
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Document name or title for identification and display
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the document content and purpose
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Original filename of the uploaded document
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File type or extension of the document
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// Size of the document file in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Storage path or location of the document file
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Category classification of the document for organization
    /// </summary>
    public DocumentCategory Category { get; set; } = DocumentCategory.General;

    /// <summary>
    /// Timestamp when the document was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifier of the user who uploaded this document
    /// </summary>
    public string UploadedBy { get; set; } = string.Empty;
}

/// <summary>
/// Task comment entity representing discussions and notes related to specific tasks.
/// Used for team communication, progress updates, and issue tracking within tasks.
/// </summary>
/// <remarks>
/// Facilitates collaboration and communication within project teams, providing
/// a record of discussions, decisions, and updates related to task execution.
/// </remarks>
public class TaskComment
{
    /// <summary>
    /// Unique identifier for the comment record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the task this comment belongs to
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Content of the comment or discussion
    /// </summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the staff member who authored this comment
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Staff member who authored this comment (navigation property)
    /// </summary>
    public Staff? Author { get; set; }

    /// <summary>
    /// Timestamp when the comment was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the comment was last updated (null if never updated)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Task attachment entity representing files and documents attached to specific tasks.
/// Used for sharing task-related files, specifications, and supporting materials.
/// </summary>
/// <remarks>
/// Enables file sharing and document management at the task level, ensuring
/// team members have access to all necessary files for task completion.
/// </remarks>
public class TaskAttachment
{
    /// <summary>
    /// Unique identifier for the attachment record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the task this attachment belongs to
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Display name or title for the attachment
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Original filename of the uploaded attachment
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File type or extension of the attachment
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// Size of the attachment file in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Storage path or location of the attachment file
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the attachment was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifier of the user who uploaded this attachment
    /// </summary>
    public string UploadedBy { get; set; } = string.Empty;
}

/// <summary>
/// Time entry entity representing time spent by staff members on projects and tasks.
/// Used for time tracking, billing, and project cost analysis.
/// </summary>
/// <remarks>
/// Essential for accurate project costing, billing clients for time spent,
/// and analyzing resource utilization and project profitability.
/// </remarks>
public class TimeEntry
{
    /// <summary>
    /// Unique identifier for the time entry record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the project this time entry is associated with
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Project this time entry is associated with (navigation property)
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    /// Identifier of the specific task this time entry is for (optional)
    /// </summary>
    public Guid? TaskId { get; set; }

    /// <summary>
    /// Specific task this time entry is for (navigation property)
    /// </summary>
    public ProjectTask? Task { get; set; }

    /// <summary>
    /// Identifier of the staff member who logged this time
    /// </summary>
    public Guid StaffId { get; set; }

    /// <summary>
    /// Staff member who logged this time (navigation property)
    /// </summary>
    public Staff? Staff { get; set; }

    /// <summary>
    /// Date when the work was performed
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;

    /// <summary>
    /// Time when work started on this date
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Time when work ended on this date
    /// </summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>
    /// Computed property returning the duration of work performed
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Computed property returning the duration in decimal hours
    /// </summary>
    public decimal Hours => (decimal)Duration.TotalHours;

    /// <summary>
    /// Description of the work performed during this time entry
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this time entry is billable to the client
    /// </summary>
    public bool IsBillable { get; set; } = true;

    /// <summary>
    /// Hourly rate for this time entry (for billing calculations)
    /// </summary>
    public decimal HourlyRate { get; set; }

    /// <summary>
    /// Computed property returning the total billable amount for this time entry
    /// </summary>
    public decimal Amount => Hours * HourlyRate;

    /// <summary>
    /// Timestamp when the time entry record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifier of the user who created this time entry record
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// Enumeration defining the possible status values for projects in their lifecycle.
/// Used to track project progression from planning through completion or cancellation.
/// </summary>
/// <remarks>
/// Essential for project management workflows, reporting, and resource allocation
/// based on project phase and current state.
/// </remarks>
public enum ProjectStatus
{
    /// <summary>Project is in the planning and preparation phase</summary>
    Planning,

    /// <summary>Project is actively being worked on</summary>
    InProgress,

    /// <summary>Project is temporarily paused or on hold</summary>
    OnHold,

    /// <summary>Project has been completed successfully</summary>
    Completed,

    /// <summary>Project has been cancelled before completion</summary>
    Cancelled,

    /// <summary>Project has been archived for historical reference</summary>
    Archived
}

/// <summary>
/// Enumeration defining priority levels for projects to guide resource allocation.
/// Used for prioritizing work and ensuring critical projects receive appropriate attention.
/// </summary>
/// <remarks>
/// Important for project portfolio management and ensuring high-priority initiatives
/// are completed on time with adequate resources.
/// </remarks>
public enum ProjectPriority
{
    /// <summary>Low priority project with flexible timeline</summary>
    Low,

    /// <summary>Medium priority project with standard attention</summary>
    Medium,

    /// <summary>High priority project requiring focused attention</summary>
    High,

    /// <summary>Critical priority project requiring immediate and intensive focus</summary>
    Critical
}

/// <summary>
/// Enumeration defining types or categories of projects for classification.
/// Used for project categorization, resource planning, and specialized workflows.
/// </summary>
/// <remarks>
/// Enables project categorization for reporting, resource allocation, and
/// applying type-specific processes and templates.
/// </remarks>
public enum ProjectType
{
    /// <summary>Software or application development project</summary>
    Development,

    /// <summary>Design and creative services project</summary>
    Design,

    /// <summary>Consulting and advisory services project</summary>
    Consulting,

    /// <summary>Maintenance and support services project</summary>
    Maintenance,

    /// <summary>Research and analysis project</summary>
    Research,

    /// <summary>Marketing and promotional project</summary>
    Marketing,

    /// <summary>Other type of project not covered by standard categories</summary>
    Other
}

/// <summary>
/// Enumeration defining the possible status values for tasks within projects.
/// Used to track individual task progression and completion.
/// </summary>
/// <remarks>
/// Essential for task management, progress tracking, and ensuring project
/// deliverables are completed on schedule.
/// </remarks>
public enum TaskStatus
{
    /// <summary>Task has not been started yet</summary>
    NotStarted,

    /// <summary>Task is currently being worked on</summary>
    InProgress,

    /// <summary>Task is temporarily paused or on hold</summary>
    OnHold,

    /// <summary>Task has been completed successfully</summary>
    Completed,

    /// <summary>Task has been cancelled and will not be completed</summary>
    Cancelled
}

/// <summary>
/// Enumeration defining priority levels for tasks to guide work prioritization.
/// Used for task scheduling and ensuring critical tasks are completed first.
/// </summary>
/// <remarks>
/// Important for daily work planning and ensuring team members focus on
/// the most important tasks to meet project deadlines.
/// </remarks>
public enum TaskPriority
{
    /// <summary>Low priority task that can be completed when time allows</summary>
    Low,

    /// <summary>Medium priority task with standard scheduling</summary>
    Medium,

    /// <summary>High priority task requiring prompt attention</summary>
    High,

    /// <summary>Critical priority task requiring immediate attention</summary>
    Critical
}

/// <summary>
/// Enumeration defining categories for project documents and files.
/// Used for organizing and classifying project-related documentation.
/// </summary>
/// <remarks>
/// Facilitates document organization, search, and ensuring appropriate
/// document types are available for different project phases.
/// </remarks>
public enum DocumentCategory
{
    /// <summary>General project documentation</summary>
    General,

    /// <summary>Requirements and specifications documents</summary>
    Requirements,

    /// <summary>Design documents and mockups</summary>
    Design,

    /// <summary>Technical documentation and architecture</summary>
    Technical,

    /// <summary>Testing plans and results</summary>
    Testing,

    /// <summary>Deployment guides and procedures</summary>
    Deployment,

    /// <summary>Legal documents and contracts</summary>
    Legal,

    /// <summary>Financial documents and budgets</summary>
    Financial
}

/// <summary>
/// Static class containing default values, descriptions, and common options for project management.
/// Provides consistent data and user interface options across all project-related components.
/// </summary>
/// <remarks>
/// Centralizes common project management data to ensure consistency across the application.
/// Simplifies form creation, data validation, and user interface standardization.
/// </remarks>
public static class ProjectDefaults
{
    /// <summary>
    /// Human-readable descriptions for each project status value.
    /// Used in user interfaces to provide clear explanations of status meanings.
    /// </summary>
    /// <remarks>
    /// Helps users understand the significance of each status and ensures consistent
    /// interpretation across different parts of the application.
    /// </remarks>
    public static readonly Dictionary<ProjectStatus, string> StatusDescriptions = new()
    {
        { ProjectStatus.Planning, "Project is in planning phase" },
        { ProjectStatus.InProgress, "Project is actively being worked on" },
        { ProjectStatus.OnHold, "Project is temporarily paused" },
        { ProjectStatus.Completed, "Project has been completed successfully" },
        { ProjectStatus.Cancelled, "Project has been cancelled" },
        { ProjectStatus.Archived, "Project has been archived" }
    };

    /// <summary>
    /// Common tags used for categorizing and organizing projects and tasks.
    /// Provides standardized options for project tagging and classification.
    /// </summary>
    /// <remarks>
    /// Helps maintain consistency in project categorization and enables effective
    /// filtering and searching based on common technology and service categories.
    /// </remarks>
    public static readonly List<string> CommonTags = new()
    {
        "Web Development",        // Web application development
        "Mobile App",            // Mobile application development
        "E-commerce",            // E-commerce platform development
        "API Integration",       // API development and integration
        "Database",              // Database design and development
        "UI/UX Design",          // User interface and experience design
        "Testing",               // Quality assurance and testing
        "Deployment",            // Application deployment and DevOps
        "Maintenance",           // Ongoing maintenance and support
        "Bug Fix",               // Bug fixes and issue resolution
        "Feature Enhancement",   // New feature development
        "Performance Optimization" // Performance improvements
    };

    /// <summary>
    /// Default milestone templates for common project types.
    /// Provides standard milestone structures for project planning and tracking.
    /// </summary>
    /// <remarks>
    /// Helps project managers quickly set up milestone tracking for new projects
    /// using proven milestone structures that cover typical project phases.
    /// </remarks>
    public static readonly List<string> DefaultMilestones = new()
    {
        "Project Kickoff",           // Initial project meeting and setup
        "Requirements Gathering",    // Collection and documentation of requirements
        "Design Approval",          // Design review and client approval
        "Development Phase 1",      // Initial development milestone
        "Testing Phase",            // Quality assurance and testing
        "User Acceptance Testing",  // Client testing and feedback
        "Deployment",               // Production deployment
        "Project Completion"        // Final delivery and project closure
    };
}
