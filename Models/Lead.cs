/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

namespace BlazorControlPanel.Models;

/// <summary>
/// Lead entity representing potential customers in the sales pipeline.
/// Contains comprehensive lead information including contact details, project requirements,
/// and sales tracking data for effective lead management and conversion.
/// </summary>
/// <remarks>
/// Central entity for sales pipeline management, used for lead qualification,
/// opportunity tracking, and conversion to customers. Supports activity tracking
/// and automated follow-up scheduling for optimal sales process management.
/// </remarks>
public class Lead
{
    /// <summary>
    /// Unique identifier for the lead record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Lead's first name or primary contact first name for business leads
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Lead's last name or primary contact last name for business leads
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Primary email address for lead communication and follow-up
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Primary phone number for lead contact and qualification calls
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Company or organization name (for business leads)
    /// </summary>
    public string Company { get; set; } = string.Empty;

    /// <summary>
    /// Job title or position of the lead within their organization
    /// </summary>
    public string JobTitle { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the lead in the sales pipeline
    /// </summary>
    public LeadStatus Status { get; set; } = LeadStatus.New;

    /// <summary>
    /// Source channel through which the lead was acquired
    /// </summary>
    public LeadSource Source { get; set; } = LeadSource.Website;

    /// <summary>
    /// Priority level assigned to this lead for resource allocation
    /// </summary>
    public LeadPriority Priority { get; set; } = LeadPriority.Medium;

    /// <summary>
    /// Estimated monetary value of the potential project or deal
    /// </summary>
    public decimal EstimatedValue { get; set; }

    /// <summary>
    /// Expected date for closing the deal or making a decision
    /// </summary>
    public DateTime ExpectedCloseDate { get; set; } = DateTime.UtcNow.AddDays(30);

    /// <summary>
    /// Detailed description of the project or service the lead is interested in
    /// </summary>
    public string ProjectDescription { get; set; } = string.Empty;

    /// <summary>
    /// Specific requirements and specifications provided by the lead
    /// </summary>
    public string Requirements { get; set; } = string.Empty;

    /// <summary>
    /// Budget information or budget range provided by the lead
    /// </summary>
    public string Budget { get; set; } = string.Empty;

    /// <summary>
    /// Timeline expectations or constraints for the project
    /// </summary>
    public string Timeline { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes and observations about the lead
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Collection of tags for categorizing and organizing leads
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Collection of activities and interactions associated with this lead
    /// </summary>
    public List<LeadActivity> Activities { get; set; } = new();

    /// <summary>
    /// Identifier of the staff member assigned to manage this lead
    /// </summary>
    public Guid? AssignedToStaffId { get; set; }

    /// <summary>
    /// Staff member assigned to manage this lead (navigation property)
    /// </summary>
    public Staff? AssignedToStaff { get; set; }

    /// <summary>
    /// Identifier of the customer record if this lead has been converted
    /// </summary>
    public Guid? ConvertedCustomerId { get; set; }

    /// <summary>
    /// Customer record created from this lead (navigation property)
    /// </summary>
    public Customer? ConvertedCustomer { get; set; }

    /// <summary>
    /// Date when the lead was successfully converted to a customer
    /// </summary>
    public DateTime? ConvertedDate { get; set; }

    /// <summary>
    /// Timestamp when the lead record was initially created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the lead record was last modified
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date of the most recent contact or interaction with this lead
    /// </summary>
    public DateTime? LastContactDate { get; set; }

    /// <summary>
    /// Scheduled date for the next follow-up activity with this lead
    /// </summary>
    public DateTime? NextFollowUpDate { get; set; }

    /// <summary>
    /// Identifier of the user who created this lead record
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the user who last updated this lead record
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Computed property returning the lead's full name by combining first and last names
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Computed property returning a display-friendly name, prioritizing company name for business leads
    /// </summary>
    public string DisplayName => !string.IsNullOrEmpty(Company) ? $"{Company} ({FullName})" : FullName;

    /// <summary>
    /// Computed property indicating whether this lead has been successfully converted to a customer
    /// </summary>
    public bool IsConverted => Status == LeadStatus.Converted && ConvertedCustomerId.HasValue;

    /// <summary>
    /// Computed property indicating whether the next follow-up is overdue
    /// </summary>
    public bool IsOverdue => NextFollowUpDate.HasValue && NextFollowUpDate.Value < DateTime.UtcNow && Status != LeadStatus.Converted && Status != LeadStatus.Lost;

    /// <summary>
    /// Computed property returning the number of days until the next scheduled follow-up
    /// </summary>
    public int DaysUntilFollowUp => NextFollowUpDate.HasValue ? (NextFollowUpDate.Value - DateTime.UtcNow).Days : 0;

    /// <summary>
    /// Computed property returning the number of days since the lead was created
    /// </summary>
    public int DaysSinceCreated => (DateTime.UtcNow - CreatedAt).Days;

    /// <summary>
    /// Computed property returning the number of days since the last contact with this lead
    /// </summary>
    public int DaysSinceLastContact => LastContactDate.HasValue ? (DateTime.UtcNow - LastContactDate.Value).Days : DaysSinceCreated;
}

/// <summary>
/// Lead activity entity representing individual interactions and tasks associated with a lead.
/// Tracks communication history, scheduled activities, and follow-up tasks for comprehensive
/// lead management and relationship building.
/// </summary>
/// <remarks>
/// Essential for maintaining detailed interaction history, scheduling follow-ups,
/// and ensuring no lead falls through the cracks. Supports activity-based sales
/// process management and performance tracking.
/// </remarks>
public class LeadActivity
{
    /// <summary>
    /// Unique identifier for the activity record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the lead this activity is associated with
    /// </summary>
    public Guid LeadId { get; set; }

    /// <summary>
    /// Type of activity (Call, Email, Meeting, Demo, etc.)
    /// </summary>
    public LeadActivityType Type { get; set; }

    /// <summary>
    /// Brief subject or title describing the activity
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the activity and its purpose
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the activity occurred or was logged
    /// </summary>
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Scheduled date and time for future activities (null for completed activities)
    /// </summary>
    public DateTime? ScheduledDate { get; set; }

    /// <summary>
    /// Indicates whether the activity has been completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Date and time when the activity was marked as completed
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Identifier of the staff member assigned to complete this activity
    /// </summary>
    public Guid? AssignedToStaffId { get; set; }

    /// <summary>
    /// Staff member assigned to complete this activity (navigation property)
    /// </summary>
    public Staff? AssignedToStaff { get; set; }

    /// <summary>
    /// Result or outcome of the completed activity
    /// </summary>
    public string Outcome { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the activity record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifier of the user who created this activity record
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Computed property indicating whether a scheduled activity is overdue
    /// </summary>
    public bool IsOverdue => ScheduledDate.HasValue && ScheduledDate.Value < DateTime.UtcNow && !IsCompleted;

    /// <summary>
    /// Computed property returning a user-friendly status description
    /// </summary>
    public string StatusDisplay => IsCompleted ? "Completed" : (IsOverdue ? "Overdue" : "Pending");
}

/// <summary>
/// Lead conversion entity tracking the successful conversion of leads to customers.
/// Records conversion details, value, and associated projects for conversion analysis
/// and sales performance measurement.
/// </summary>
/// <remarks>
/// Critical for sales analytics, conversion rate tracking, and measuring the
/// effectiveness of lead generation and nurturing efforts. Links leads to their
/// resulting customer relationships and initial projects.
/// </remarks>
public class LeadConversion
{
    /// <summary>
    /// Unique identifier for the conversion record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Identifier of the lead that was converted
    /// </summary>
    public Guid LeadId { get; set; }

    /// <summary>
    /// Lead that was converted (navigation property)
    /// </summary>
    public Lead? Lead { get; set; }

    /// <summary>
    /// Identifier of the customer record created from the lead
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Customer record created from the lead (navigation property)
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// Date and time when the conversion occurred
    /// </summary>
    public DateTime ConversionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Monetary value of the conversion (initial project value or deal size)
    /// </summary>
    public decimal ConversionValue { get; set; }

    /// <summary>
    /// Additional notes about the conversion process and outcome
    /// </summary>
    public string ConversionNotes { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the first project created for the converted customer
    /// </summary>
    public Guid? FirstProjectId { get; set; }

    /// <summary>
    /// Identifier of the first estimate/quote that led to the conversion
    /// </summary>
    public Guid? FirstEstimateId { get; set; }

    /// <summary>
    /// Identifier of the user who performed the conversion
    /// </summary>
    public string ConvertedBy { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the conversion record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Enumeration defining the possible status values for leads in the sales pipeline.
/// Represents the progression of leads through the sales process from initial contact to conversion.
/// </summary>
/// <remarks>
/// Essential for sales pipeline management, conversion tracking, and sales process optimization.
/// Each status represents a distinct stage in the lead qualification and conversion process.
/// </remarks>
public enum LeadStatus
{
    /// <summary>New lead that has not yet been contacted</summary>
    New,

    /// <summary>Initial contact has been made with the lead</summary>
    Contacted,

    /// <summary>Lead has been qualified as a potential customer</summary>
    Qualified,

    /// <summary>Proposal or quote has been sent to the lead</summary>
    Proposal,

    /// <summary>Currently in negotiation phase with the lead</summary>
    Negotiation,

    /// <summary>Lead has been successfully converted to a customer</summary>
    Converted,

    /// <summary>Lead was lost to competitor or declined services</summary>
    Lost,

    /// <summary>Lead does not meet qualification criteria</summary>
    Unqualified
}

/// <summary>
/// Enumeration defining the possible sources through which leads can be acquired.
/// Used for tracking lead generation effectiveness and marketing ROI analysis.
/// </summary>
/// <remarks>
/// Critical for marketing attribution, source performance analysis, and optimizing
/// lead generation strategies based on conversion rates and cost effectiveness.
/// </remarks>
public enum LeadSource
{
    /// <summary>Lead came through website contact form or inquiry</summary>
    Website,

    /// <summary>Lead was referred by an existing customer</summary>
    Referral,

    /// <summary>Lead found through social media channels</summary>
    SocialMedia,

    /// <summary>Lead responded to email marketing campaign</summary>
    EmailMarketing,

    /// <summary>Lead generated through cold calling efforts</summary>
    ColdCall,

    /// <summary>Lead met at trade show or industry event</summary>
    TradeShow,

    /// <summary>Lead responded to advertisement (online or offline)</summary>
    Advertisement,

    /// <summary>Lead referred by business partner or affiliate</summary>
    PartnerReferral,

    /// <summary>Lead responded to direct mail campaign</summary>
    DirectMail,

    /// <summary>Lead source not specified or other source</summary>
    Other
}

/// <summary>
/// Enumeration defining priority levels for leads to guide resource allocation.
/// Used for prioritizing sales efforts and ensuring high-value leads receive appropriate attention.
/// </summary>
/// <remarks>
/// Essential for sales team productivity, ensuring critical opportunities are not missed,
/// and optimizing resource allocation based on lead potential and urgency.
/// </remarks>
public enum LeadPriority
{
    /// <summary>Low priority lead with standard follow-up schedule</summary>
    Low,

    /// <summary>Medium priority lead with regular follow-up</summary>
    Medium,

    /// <summary>High priority lead requiring frequent attention</summary>
    High,

    /// <summary>Critical priority lead requiring immediate and intensive focus</summary>
    Critical
}

/// <summary>
/// Enumeration defining types of activities that can be performed with leads.
/// Used for categorizing interactions and tracking communication history.
/// </summary>
/// <remarks>
/// Important for activity tracking, sales process management, and ensuring
/// comprehensive communication history for effective lead nurturing.
/// </remarks>
public enum LeadActivityType
{
    /// <summary>Phone call with the lead</summary>
    Call,

    /// <summary>Email communication with the lead</summary>
    Email,

    /// <summary>In-person or virtual meeting with the lead</summary>
    Meeting,

    /// <summary>Product or service demonstration</summary>
    Demo,

    /// <summary>Proposal presentation or delivery</summary>
    Proposal,

    /// <summary>Follow-up contact or check-in</summary>
    FollowUp,

    /// <summary>Internal note or observation about the lead</summary>
    Note,

    /// <summary>Task or action item related to the lead</summary>
    Task,

    /// <summary>Scheduled appointment or meeting</summary>
    Appointment
}

/// <summary>
/// Static class containing default values, descriptions, and common options for lead management.
/// Provides consistent data and user interface options across all lead-related components.
/// </summary>
/// <remarks>
/// Centralizes common lead management data to ensure consistency across the application.
/// Simplifies form creation, data validation, and user interface standardization.
/// </remarks>
public static class LeadDefaults
{
    /// <summary>
    /// Human-readable descriptions for each lead status value.
    /// Used in user interfaces to provide clear explanations of status meanings.
    /// </summary>
    /// <remarks>
    /// Helps users understand the significance of each status and ensures consistent
    /// interpretation across different parts of the application.
    /// </remarks>
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

    /// <summary>
    /// Human-readable descriptions for each lead source value.
    /// Used in user interfaces and reports to explain lead acquisition channels.
    /// </summary>
    /// <remarks>
    /// Provides context for lead sources in analytics and helps users understand
    /// the effectiveness of different marketing and acquisition channels.
    /// </remarks>
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

    /// <summary>
    /// Common tags used for categorizing and organizing leads.
    /// Provides standardized options for lead tagging and classification.
    /// </summary>
    /// <remarks>
    /// Helps maintain consistency in lead categorization and enables effective
    /// filtering and searching based on common business categories.
    /// </remarks>
    public static readonly List<string> CommonTags = new()
    {
        "Hot Lead",           // High-priority, immediate opportunity
        "Enterprise",         // Large enterprise customer
        "Small Business",     // Small business customer
        "Web Development",    // Web development project
        "Mobile App",         // Mobile application project
        "E-commerce",         // E-commerce platform project
        "Consulting",         // Consulting services
        "Maintenance",        // Maintenance and support services
        "Urgent",            // Time-sensitive project
        "Budget Conscious",   // Price-sensitive customer
        "High Value",        // High-value opportunity
        "Repeat Business"    // Returning customer or referral
    };

    /// <summary>
    /// Common budget ranges for lead qualification and project scoping.
    /// Provides standardized budget categories for consistent lead evaluation.
    /// </summary>
    /// <remarks>
    /// Helps sales teams quickly categorize leads by budget potential and
    /// prioritize efforts based on project value and resource requirements.
    /// </remarks>
    public static readonly List<string> CommonBudgetRanges = new()
    {
        "Under $5,000",       // Small projects and basic services
        "$5,000 - $10,000",   // Small to medium projects
        "$10,000 - $25,000",  // Medium projects
        "$25,000 - $50,000",  // Large projects
        "$50,000 - $100,000", // Enterprise projects
        "Over $100,000",      // Major enterprise initiatives
        "Budget not disclosed" // Budget information not available
    };

    /// <summary>
    /// Common timeline expectations for project completion.
    /// Provides standardized timeline options for project planning and resource allocation.
    /// </summary>
    /// <remarks>
    /// Helps in project planning, resource scheduling, and setting appropriate
    /// expectations with leads based on their timeline requirements.
    /// </remarks>
    public static readonly List<string> CommonTimelines = new()
    {
        "ASAP",              // Immediate start required
        "Within 1 month",    // Quick turnaround project
        "1-3 months",        // Standard project timeline
        "3-6 months",        // Medium-term project
        "6-12 months",       // Long-term project
        "Over 1 year",       // Extended timeline project
        "Flexible timeline"  // No specific timeline constraints
    };
}
