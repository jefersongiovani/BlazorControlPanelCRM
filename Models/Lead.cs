namespace BlazorControlPanel.Models;

public class Lead
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public LeadSource Source { get; set; } = LeadSource.Website;
    public LeadPriority Priority { get; set; } = LeadPriority.Medium;
    public decimal EstimatedValue { get; set; }
    public DateTime ExpectedCloseDate { get; set; } = DateTime.UtcNow.AddDays(30);
    public string ProjectDescription { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Budget { get; set; } = string.Empty;
    public string Timeline { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public List<LeadActivity> Activities { get; set; } = new();
    public Guid? AssignedToStaffId { get; set; }
    public Staff? AssignedToStaff { get; set; }
    public Guid? ConvertedCustomerId { get; set; }
    public Customer? ConvertedCustomer { get; set; }
    public DateTime? ConvertedDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastContactDate { get; set; }
    public DateTime? NextFollowUpDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayName => !string.IsNullOrEmpty(Company) ? $"{Company} ({FullName})" : FullName;
    public bool IsConverted => Status == LeadStatus.Converted && ConvertedCustomerId.HasValue;
    public bool IsOverdue => NextFollowUpDate.HasValue && NextFollowUpDate.Value < DateTime.UtcNow && Status != LeadStatus.Converted && Status != LeadStatus.Lost;
    public int DaysUntilFollowUp => NextFollowUpDate.HasValue ? (NextFollowUpDate.Value - DateTime.UtcNow).Days : 0;
    public int DaysSinceCreated => (DateTime.UtcNow - CreatedAt).Days;
    public int DaysSinceLastContact => LastContactDate.HasValue ? (DateTime.UtcNow - LastContactDate.Value).Days : DaysSinceCreated;
}

public class LeadActivity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LeadId { get; set; }
    public LeadActivityType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
    public DateTime? ScheduledDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Guid? AssignedToStaffId { get; set; }
    public Staff? AssignedToStaff { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    
    public bool IsOverdue => ScheduledDate.HasValue && ScheduledDate.Value < DateTime.UtcNow && !IsCompleted;
    public string StatusDisplay => IsCompleted ? "Completed" : (IsOverdue ? "Overdue" : "Pending");
}

public class LeadConversion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LeadId { get; set; }
    public Lead? Lead { get; set; }
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public DateTime ConversionDate { get; set; } = DateTime.UtcNow;
    public decimal ConversionValue { get; set; }
    public string ConversionNotes { get; set; } = string.Empty;
    public Guid? FirstProjectId { get; set; }
    public Guid? FirstEstimateId { get; set; }
    public string ConvertedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum LeadStatus
{
    New,
    Contacted,
    Qualified,
    Proposal,
    Negotiation,
    Converted,
    Lost,
    Unqualified
}

public enum LeadSource
{
    Website,
    Referral,
    SocialMedia,
    EmailMarketing,
    ColdCall,
    TradeShow,
    Advertisement,
    PartnerReferral,
    DirectMail,
    Other
}

public enum LeadPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum LeadActivityType
{
    Call,
    Email,
    Meeting,
    Demo,
    Proposal,
    FollowUp,
    Note,
    Task,
    Appointment
}

public static class LeadDefaults
{
    public static readonly Dictionary<LeadStatus, string> StatusDescriptions = new()
    {
        { LeadStatus.New, "New lead, not yet contacted" },
        { LeadStatus.Contacted, "Initial contact made" },
        { LeadStatus.Qualified, "Lead has been qualified as potential customer" },
        { LeadStatus.Proposal, "Proposal or quote has been sent" },
        { LeadStatus.Negotiation, "In negotiation phase" },
        { LeadStatus.Converted, "Successfully converted to customer" },
        { LeadStatus.Lost, "Lead was lost to competitor or declined" },
        { LeadStatus.Unqualified, "Lead does not meet qualification criteria" }
    };

    public static readonly Dictionary<LeadSource, string> SourceDescriptions = new()
    {
        { LeadSource.Website, "Came through website contact form" },
        { LeadSource.Referral, "Referred by existing customer" },
        { LeadSource.SocialMedia, "Found through social media channels" },
        { LeadSource.EmailMarketing, "Responded to email campaign" },
        { LeadSource.ColdCall, "Generated through cold calling" },
        { LeadSource.TradeShow, "Met at trade show or event" },
        { LeadSource.Advertisement, "Responded to advertisement" },
        { LeadSource.PartnerReferral, "Referred by business partner" },
        { LeadSource.DirectMail, "Responded to direct mail campaign" },
        { LeadSource.Other, "Other source" }
    };

    public static readonly List<string> CommonTags = new()
    {
        "Hot Lead",
        "Enterprise",
        "Small Business",
        "Web Development",
        "Mobile App",
        "E-commerce",
        "Consulting",
        "Maintenance",
        "Urgent",
        "Budget Conscious",
        "High Value",
        "Repeat Business"
    };

    public static readonly List<string> CommonBudgetRanges = new()
    {
        "Under $5,000",
        "$5,000 - $10,000",
        "$10,000 - $25,000",
        "$25,000 - $50,000",
        "$50,000 - $100,000",
        "Over $100,000",
        "Budget not disclosed"
    };

    public static readonly List<string> CommonTimelines = new()
    {
        "ASAP",
        "Within 1 month",
        "1-3 months",
        "3-6 months",
        "6-12 months",
        "Over 1 year",
        "Flexible timeline"
    };
}
