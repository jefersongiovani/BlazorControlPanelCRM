/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

namespace BlazorControlPanel.Models;

/// <summary>
/// Main dashboard analytics container that aggregates all business metrics and KPIs.
/// This class serves as the primary data model for the analytics dashboard, providing
/// a comprehensive view of business performance across all departments.
/// </summary>
/// <remarks>
/// Used by the analytics service to present unified business intelligence data
/// including customer metrics, lead performance, project status, financial health,
/// staff productivity, and overall performance indicators.
/// </remarks>
public sealed class DashboardAnalytics
{
    /// <summary>
    /// Customer-related analytics including growth rates, segmentation, and trends
    /// </summary>
    public CustomerAnalytics Customers { get; set; } = new();

    /// <summary>
    /// Lead management analytics including conversion rates and pipeline metrics
    /// </summary>
    public LeadAnalytics Leads { get; set; } = new();

    /// <summary>
    /// Project performance analytics including budget variance and completion rates
    /// </summary>
    public ProjectAnalytics Projects { get; set; } = new();

    /// <summary>
    /// Financial analytics including revenue, expenses, and profitability metrics
    /// </summary>
    public FinancialAnalytics Financial { get; set; } = new();

    /// <summary>
    /// Staff performance and workload analytics
    /// </summary>
    public StaffAnalytics Staff { get; set; } = new();

    /// <summary>
    /// Overall business performance metrics and KPI tracking
    /// </summary>
    public PerformanceMetrics Performance { get; set; } = new();

    /// <summary>
    /// Timestamp when this analytics snapshot was generated
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Customer analytics data model containing comprehensive customer metrics and trends.
/// Provides insights into customer acquisition, retention, segmentation, and value analysis.
/// </summary>
/// <remarks>
/// Used for customer relationship management reporting, growth analysis, and strategic
/// decision making regarding customer acquisition and retention strategies.
/// </remarks>
public class CustomerAnalytics
{
    /// <summary>
    /// Total number of customers in the system across all statuses
    /// </summary>
    public int TotalCustomers { get; set; }

    /// <summary>
    /// Number of customers with active status (currently engaged)
    /// </summary>
    public int ActiveCustomers { get; set; }

    /// <summary>
    /// Number of new customers acquired in the current month
    /// </summary>
    public int NewCustomersThisMonth { get; set; }

    /// <summary>
    /// Number of new customers acquired in the previous month for comparison
    /// </summary>
    public int NewCustomersLastMonth { get; set; }

    /// <summary>
    /// Month-over-month customer growth rate as a percentage
    /// </summary>
    public decimal CustomerGrowthRate { get; set; }

    /// <summary>
    /// Average revenue value per customer across all active customers
    /// </summary>
    public decimal AverageCustomerValue { get; set; }

    /// <summary>
    /// Distribution of customers by type (Individual, Corporate, Enterprise, etc.)
    /// </summary>
    public Dictionary<CustomerType, int> CustomersByType { get; set; } = new();

    /// <summary>
    /// Distribution of customers by current status (Active, Inactive, Prospect, etc.)
    /// </summary>
    public Dictionary<CustomerStatus, int> CustomersByStatus { get; set; } = new();

    /// <summary>
    /// Historical monthly trends showing customer acquisition and revenue patterns
    /// </summary>
    public List<CustomerTrend> MonthlyTrends { get; set; } = new();

    /// <summary>
    /// List of highest value customers ranked by total revenue contribution
    /// </summary>
    public List<TopCustomer> TopCustomers { get; set; } = new();
}

/// <summary>
/// Lead management analytics providing comprehensive insights into sales pipeline performance.
/// Tracks lead acquisition, conversion rates, pipeline value, and source effectiveness.
/// </summary>
/// <remarks>
/// Essential for sales team performance evaluation, lead source optimization, and
/// sales forecasting. Helps identify bottlenecks in the sales process and optimize
/// conversion strategies.
/// </remarks>
public class LeadAnalytics
{
    /// <summary>
    /// Total number of leads in the system across all statuses and time periods
    /// </summary>
    public int TotalLeads { get; set; }

    /// <summary>
    /// Number of leads currently being actively pursued (not converted or closed)
    /// </summary>
    public int ActiveLeads { get; set; }

    /// <summary>
    /// Number of leads that have been successfully converted to customers
    /// </summary>
    public int ConvertedLeads { get; set; }

    /// <summary>
    /// Number of new leads generated in the current month
    /// </summary>
    public int NewLeadsThisMonth { get; set; }

    /// <summary>
    /// Lead to customer conversion rate as a percentage
    /// </summary>
    public decimal ConversionRate { get; set; }

    /// <summary>
    /// Average potential value per lead in the pipeline
    /// </summary>
    public decimal AverageLeadValue { get; set; }

    /// <summary>
    /// Total monetary value of all active leads in the sales pipeline
    /// </summary>
    public decimal TotalPipelineValue { get; set; }

    /// <summary>
    /// Number of leads with overdue follow-up activities requiring attention
    /// </summary>
    public int OverdueFollowUps { get; set; }

    /// <summary>
    /// Distribution of leads by current status (New, Qualified, Proposal, etc.)
    /// </summary>
    public Dictionary<LeadStatus, int> LeadsByStatus { get; set; } = new();

    /// <summary>
    /// Distribution of leads by acquisition source (Website, Referral, Social Media, etc.)
    /// </summary>
    public Dictionary<LeadSource, int> LeadsBySource { get; set; } = new();

    /// <summary>
    /// Distribution of leads by priority level (High, Medium, Low)
    /// </summary>
    public Dictionary<LeadPriority, int> LeadsByPriority { get; set; } = new();

    /// <summary>
    /// Historical monthly trends showing lead generation and conversion patterns
    /// </summary>
    public List<LeadTrend> MonthlyTrends { get; set; } = new();

    /// <summary>
    /// Performance metrics for each lead source including ROI and conversion rates
    /// </summary>
    public List<SourcePerformance> SourcePerformance { get; set; } = new();
}

/// <summary>
/// Project management analytics providing comprehensive insights into project performance,
/// budget management, task completion, and resource utilization across all active projects.
/// </summary>
/// <remarks>
/// Critical for project managers and executives to monitor project health, identify
/// at-risk projects, optimize resource allocation, and ensure on-time, on-budget delivery.
/// </remarks>
public class ProjectAnalytics
{
    /// <summary>
    /// Total number of projects in the system across all statuses and time periods
    /// </summary>
    public int TotalProjects { get; set; }

    /// <summary>
    /// Number of projects currently in progress (not completed or cancelled)
    /// </summary>
    public int ActiveProjects { get; set; }

    /// <summary>
    /// Number of projects that have been successfully completed
    /// </summary>
    public int CompletedProjects { get; set; }

    /// <summary>
    /// Number of projects that are past their scheduled completion date
    /// </summary>
    public int OverdueProjects { get; set; }

    /// <summary>
    /// Combined budget allocation for all projects
    /// </summary>
    public decimal TotalBudget { get; set; }

    /// <summary>
    /// Actual costs incurred across all projects to date
    /// </summary>
    public decimal TotalActualCost { get; set; }

    /// <summary>
    /// Variance between budgeted and actual costs (positive = under budget, negative = over budget)
    /// </summary>
    public decimal BudgetVariance { get; set; }

    /// <summary>
    /// Average completion percentage across all active projects
    /// </summary>
    public decimal AverageProjectCompletion { get; set; }

    /// <summary>
    /// Total number of tasks across all projects
    /// </summary>
    public int TotalTasks { get; set; }

    /// <summary>
    /// Number of tasks that have been marked as completed
    /// </summary>
    public int CompletedTasks { get; set; }

    /// <summary>
    /// Number of tasks that are past their due date
    /// </summary>
    public int OverdueTasks { get; set; }

    /// <summary>
    /// Percentage of tasks completed on time across all projects
    /// </summary>
    public decimal TaskCompletionRate { get; set; }

    /// <summary>
    /// Distribution of projects by current status (Planning, Active, On Hold, Completed, etc.)
    /// </summary>
    public Dictionary<ProjectStatus, int> ProjectsByStatus { get; set; } = new();

    /// <summary>
    /// Distribution of projects by type (Development, Consulting, Maintenance, etc.)
    /// </summary>
    public Dictionary<ProjectType, int> ProjectsByType { get; set; } = new();

    /// <summary>
    /// Historical monthly trends showing project initiation and completion patterns
    /// </summary>
    public List<ProjectTrend> MonthlyTrends { get; set; } = new();

    /// <summary>
    /// Individual project performance metrics including budget and schedule variance
    /// </summary>
    public List<ProjectPerformance> ProjectPerformance { get; set; } = new();
}

/// <summary>
/// Financial analytics providing comprehensive insights into revenue, expenses, cash flow,
/// and overall financial health of the business. Includes invoice management and payment tracking.
/// </summary>
/// <remarks>
/// Essential for financial planning, cash flow management, and business performance evaluation.
/// Used by finance teams and executives for strategic financial decision making.
/// </remarks>
public class FinancialAnalytics
{
    /// <summary>
    /// Total revenue generated across all time periods and customers
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Revenue generated in the current month
    /// </summary>
    public decimal RevenueThisMonth { get; set; }

    /// <summary>
    /// Revenue generated in the previous month for comparison
    /// </summary>
    public decimal RevenueLastMonth { get; set; }

    /// <summary>
    /// Month-over-month revenue growth rate as a percentage
    /// </summary>
    public decimal RevenueGrowthRate { get; set; }

    /// <summary>
    /// Total value of all estimates/quotes issued to prospects
    /// </summary>
    public decimal TotalEstimateValue { get; set; }

    /// <summary>
    /// Total value of all invoices issued to customers
    /// </summary>
    public decimal TotalInvoiceValue { get; set; }

    /// <summary>
    /// Total amount actually received from customers (paid invoices)
    /// </summary>
    public decimal TotalPaidAmount { get; set; }

    /// <summary>
    /// Total amount still owed by customers (unpaid invoices)
    /// </summary>
    public decimal TotalOutstanding { get; set; }

    /// <summary>
    /// Average monetary value per invoice across all customers
    /// </summary>
    public decimal AverageInvoiceValue { get; set; }

    /// <summary>
    /// Total number of estimates/quotes created
    /// </summary>
    public int TotalEstimates { get; set; }

    /// <summary>
    /// Total number of invoices issued
    /// </summary>
    public int TotalInvoices { get; set; }

    /// <summary>
    /// Number of invoices that have been fully paid
    /// </summary>
    public int PaidInvoices { get; set; }

    /// <summary>
    /// Number of invoices that are past their due date
    /// </summary>
    public int OverdueInvoices { get; set; }

    /// <summary>
    /// Percentage of invoices collected on time
    /// </summary>
    public decimal PaymentCollectionRate { get; set; }

    /// <summary>
    /// Historical monthly revenue, expense, and profit data
    /// </summary>
    public List<MonthlyRevenue> MonthlyRevenue { get; set; } = new();

    /// <summary>
    /// Historical payment collection trends and patterns
    /// </summary>
    public List<PaymentTrend> PaymentTrends { get; set; } = new();

    /// <summary>
    /// Revenue breakdown by customer showing top revenue contributors
    /// </summary>
    public Dictionary<string, decimal> RevenueByCustomer { get; set; } = new();
}

/// <summary>
/// Staff analytics providing insights into team performance, workload distribution,
/// and productivity metrics across all staff members and roles.
/// </summary>
/// <remarks>
/// Used by HR and management for performance evaluation, workload balancing,
/// and resource planning. Helps identify high performers and optimize team efficiency.
/// </remarks>
public class StaffAnalytics
{
    /// <summary>
    /// Total number of staff members in the organization
    /// </summary>
    public int TotalStaff { get; set; }

    /// <summary>
    /// Number of staff members currently active (not on leave or inactive)
    /// </summary>
    public int ActiveStaff { get; set; }

    /// <summary>
    /// Average number of projects assigned per staff member
    /// </summary>
    public decimal AverageProjectsPerStaff { get; set; }

    /// <summary>
    /// Average number of leads assigned per staff member
    /// </summary>
    public decimal AverageLeadsPerStaff { get; set; }

    /// <summary>
    /// Distribution of staff members by role/position (Manager, Developer, Sales, etc.)
    /// </summary>
    public Dictionary<string, int> StaffByRole { get; set; } = new();

    /// <summary>
    /// Individual performance metrics for each staff member
    /// </summary>
    public List<StaffPerformance> StaffPerformance { get; set; } = new();

    /// <summary>
    /// Current workload analysis for each staff member
    /// </summary>
    public List<StaffWorkload> StaffWorkload { get; set; } = new();
}

/// <summary>
/// Overall business performance metrics and key performance indicators (KPIs).
/// Provides high-level insights into business health and operational efficiency.
/// </summary>
/// <remarks>
/// Used by executives and managers for strategic decision making and performance monitoring.
/// Contains the most critical business metrics for dashboard display.
/// </remarks>
public class PerformanceMetrics
{
    /// <summary>
    /// Customer satisfaction score based on feedback and surveys (0-100 scale)
    /// </summary>
    public decimal CustomerSatisfactionScore { get; set; }

    /// <summary>
    /// Percentage of projects delivered on time and within scope
    /// </summary>
    public decimal ProjectDeliveryRate { get; set; }

    /// <summary>
    /// Average time in hours to respond to new leads
    /// </summary>
    public decimal LeadResponseTime { get; set; }

    /// <summary>
    /// Average duration in days for project completion
    /// </summary>
    public decimal AverageProjectDuration { get; set; }

    /// <summary>
    /// Percentage of available resources currently utilized
    /// </summary>
    public decimal ResourceUtilization { get; set; }

    /// <summary>
    /// Overall profit margin as a percentage of revenue
    /// </summary>
    public decimal ProfitMargin { get; set; }

    /// <summary>
    /// Historical trends for all key performance indicators
    /// </summary>
    public List<KPITrend> KPITrends { get; set; } = new();
}

// Supporting classes for trends and performance

/// <summary>
/// Monthly customer trend data for tracking customer acquisition and revenue patterns over time.
/// Used for historical analysis and forecasting customer growth.
/// </summary>
/// <remarks>
/// Essential for understanding seasonal patterns, growth trends, and the effectiveness
/// of customer acquisition strategies over time.
/// </remarks>
public class CustomerTrend
{
    /// <summary>
    /// The month and year this trend data represents
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Number of new customers acquired during this month
    /// </summary>
    public int NewCustomers { get; set; }

    /// <summary>
    /// Total cumulative customers at the end of this month
    /// </summary>
    public int TotalCustomers { get; set; }

    /// <summary>
    /// Total revenue generated from customers during this month
    /// </summary>
    public decimal Revenue { get; set; }
}

/// <summary>
/// Monthly lead trend data for tracking lead generation and conversion patterns over time.
/// Used for sales pipeline analysis and lead source optimization.
/// </summary>
/// <remarks>
/// Critical for sales forecasting, identifying seasonal trends, and measuring
/// the effectiveness of marketing campaigns and lead generation efforts.
/// </remarks>
public class LeadTrend
{
    /// <summary>
    /// The month and year this trend data represents
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Number of new leads generated during this month
    /// </summary>
    public int NewLeads { get; set; }

    /// <summary>
    /// Number of leads converted to customers during this month
    /// </summary>
    public int ConvertedLeads { get; set; }

    /// <summary>
    /// Lead to customer conversion rate for this month as a percentage
    /// </summary>
    public decimal ConversionRate { get; set; }

    /// <summary>
    /// Total value of the sales pipeline at the end of this month
    /// </summary>
    public decimal PipelineValue { get; set; }
}

/// <summary>
/// Monthly project trend data for tracking project initiation and completion patterns over time.
/// Used for capacity planning and project delivery analysis.
/// </summary>
/// <remarks>
/// Important for resource planning, identifying delivery bottlenecks, and
/// understanding project lifecycle patterns for better scheduling.
/// </remarks>
public class ProjectTrend
{
    /// <summary>
    /// The month and year this trend data represents
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Number of new projects started during this month
    /// </summary>
    public int NewProjects { get; set; }

    /// <summary>
    /// Number of projects completed during this month
    /// </summary>
    public int CompletedProjects { get; set; }

    /// <summary>
    /// Total budget allocated for projects during this month
    /// </summary>
    public decimal TotalBudget { get; set; }

    /// <summary>
    /// Average completion rate of active projects during this month as a percentage
    /// </summary>
    public decimal CompletionRate { get; set; }
}

/// <summary>
/// Monthly revenue data including income, expenses, and profitability metrics.
/// Used for financial trend analysis and business performance tracking.
/// </summary>
/// <remarks>
/// Essential for financial planning, budget analysis, and understanding
/// the financial health and growth trajectory of the business.
/// </remarks>
public class MonthlyRevenue
{
    /// <summary>
    /// The month and year this revenue data represents
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Total revenue generated during this month
    /// </summary>
    public decimal Revenue { get; set; }

    /// <summary>
    /// Total expenses incurred during this month
    /// </summary>
    public decimal Expenses { get; set; }

    /// <summary>
    /// Net profit (revenue minus expenses) for this month
    /// </summary>
    public decimal Profit { get; set; }

    /// <summary>
    /// Profit margin as a percentage of revenue for this month
    /// </summary>
    public decimal ProfitMargin { get; set; }
}

/// <summary>
/// Monthly payment collection trend data for tracking invoice and payment patterns.
/// Used for cash flow analysis and accounts receivable management.
/// </summary>
/// <remarks>
/// Critical for cash flow forecasting, identifying collection issues, and
/// optimizing payment terms and collection processes.
/// </remarks>
public class PaymentTrend
{
    /// <summary>
    /// The month and year this payment trend data represents
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Total amount invoiced to customers during this month
    /// </summary>
    public decimal TotalInvoiced { get; set; }

    /// <summary>
    /// Total amount actually collected from customers during this month
    /// </summary>
    public decimal TotalPaid { get; set; }

    /// <summary>
    /// Total amount still outstanding (unpaid) at the end of this month
    /// </summary>
    public decimal Outstanding { get; set; }

    /// <summary>
    /// Payment collection rate as a percentage for this month
    /// </summary>
    public decimal CollectionRate { get; set; }
}

/// <summary>
/// Top customer data model representing high-value customers ranked by revenue contribution.
/// Used for customer relationship management and VIP customer identification.
/// </summary>
/// <remarks>
/// Essential for account management, identifying key customers for special attention,
/// and understanding which customers drive the most business value.
/// </remarks>
public class TopCustomer
{
    /// <summary>
    /// Unique identifier for the customer
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Display name of the customer
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Total revenue generated from this customer across all projects and time periods
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Number of projects completed or in progress for this customer
    /// </summary>
    public int ProjectCount { get; set; }

    /// <summary>
    /// Date and time of the customer's most recent activity or interaction
    /// </summary>
    public DateTime LastActivity { get; set; }
}

/// <summary>
/// Lead source performance metrics for evaluating the effectiveness of different lead generation channels.
/// Used for marketing ROI analysis and lead source optimization.
/// </summary>
/// <remarks>
/// Critical for marketing budget allocation, identifying the most effective lead sources,
/// and optimizing marketing strategies for better conversion rates.
/// </remarks>
public class SourcePerformance
{
    /// <summary>
    /// The lead source being analyzed (Website, Referral, Social Media, etc.)
    /// </summary>
    public LeadSource Source { get; set; }

    /// <summary>
    /// Total number of leads generated from this source
    /// </summary>
    public int TotalLeads { get; set; }

    /// <summary>
    /// Number of leads from this source that converted to customers
    /// </summary>
    public int ConvertedLeads { get; set; }

    /// <summary>
    /// Conversion rate for this source as a percentage
    /// </summary>
    public decimal ConversionRate { get; set; }

    /// <summary>
    /// Average value of leads generated from this source
    /// </summary>
    public decimal AverageValue { get; set; }

    /// <summary>
    /// Return on investment for this lead source as a percentage
    /// </summary>
    public decimal ROI { get; set; }
}

/// <summary>
/// Individual project performance metrics for tracking project health and delivery success.
/// Used for project management and identifying at-risk projects.
/// </summary>
/// <remarks>
/// Essential for project managers to monitor project health, identify issues early,
/// and ensure successful project delivery within budget and timeline constraints.
/// </remarks>
public class ProjectPerformance
{
    /// <summary>
    /// Unique identifier for the project
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Display name of the project
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Budget variance as a percentage (positive = under budget, negative = over budget)
    /// </summary>
    public decimal BudgetVariance { get; set; }

    /// <summary>
    /// Time variance as a percentage (positive = ahead of schedule, negative = behind schedule)
    /// </summary>
    public decimal TimeVariance { get; set; }

    /// <summary>
    /// Current completion percentage of the project (0-100)
    /// </summary>
    public decimal CompletionPercentage { get; set; }

    /// <summary>
    /// Indicates whether the project is currently on schedule
    /// </summary>
    public bool IsOnTime { get; set; }

    /// <summary>
    /// Indicates whether the project is currently within budget
    /// </summary>
    public bool IsOnBudget { get; set; }
}

/// <summary>
/// Individual staff member performance metrics for evaluating productivity and effectiveness.
/// Used for performance reviews, bonus calculations, and identifying training needs.
/// </summary>
/// <remarks>
/// Essential for HR management, performance evaluation, and identifying high-performing
/// team members. Helps in making decisions about promotions, training, and resource allocation.
/// </remarks>
public class StaffPerformance
{
    /// <summary>
    /// Unique identifier for the staff member
    /// </summary>
    public Guid StaffId { get; set; }

    /// <summary>
    /// Display name of the staff member
    /// </summary>
    public string StaffName { get; set; } = string.Empty;

    /// <summary>
    /// Number of projects currently managed or assigned to this staff member
    /// </summary>
    public int ProjectsManaged { get; set; }

    /// <summary>
    /// Number of leads currently assigned to this staff member
    /// </summary>
    public int LeadsAssigned { get; set; }

    /// <summary>
    /// Number of tasks completed by this staff member in the current period
    /// </summary>
    public int TasksCompleted { get; set; }

    /// <summary>
    /// Average completion percentage across all projects managed by this staff member
    /// </summary>
    public decimal AverageProjectCompletion { get; set; }

    /// <summary>
    /// Lead to customer conversion rate for this staff member as a percentage
    /// </summary>
    public decimal LeadConversionRate { get; set; }

    /// <summary>
    /// Overall productivity score calculated from various performance metrics (0-100)
    /// </summary>
    public decimal ProductivityScore { get; set; }
}

/// <summary>
/// Current workload analysis for individual staff members to ensure balanced resource allocation.
/// Used for workload management and preventing staff burnout.
/// </summary>
/// <remarks>
/// Critical for resource planning, identifying overloaded team members, and ensuring
/// equitable work distribution across the team for optimal productivity.
/// </remarks>
public class StaffWorkload
{
    /// <summary>
    /// Unique identifier for the staff member
    /// </summary>
    public Guid StaffId { get; set; }

    /// <summary>
    /// Display name of the staff member
    /// </summary>
    public string StaffName { get; set; } = string.Empty;

    /// <summary>
    /// Number of active projects currently assigned to this staff member
    /// </summary>
    public int ActiveProjects { get; set; }

    /// <summary>
    /// Number of active tasks currently assigned to this staff member
    /// </summary>
    public int ActiveTasks { get; set; }

    /// <summary>
    /// Number of active leads currently assigned to this staff member
    /// </summary>
    public int ActiveLeads { get; set; }

    /// <summary>
    /// Calculated workload score based on assigned work (0-100, higher = more loaded)
    /// </summary>
    public decimal WorkloadScore { get; set; }

    /// <summary>
    /// Descriptive workload status: Low, Normal, High, or Overloaded
    /// </summary>
    public string WorkloadStatus { get; set; } = string.Empty;
}

/// <summary>
/// Key Performance Indicator trend data for tracking business metrics over time.
/// Used for performance monitoring and identifying trends in critical business metrics.
/// </summary>
/// <remarks>
/// Essential for executive dashboards, performance monitoring, and identifying
/// areas that need attention or are performing exceptionally well.
/// </remarks>
public class KPITrend
{
    /// <summary>
    /// Name of the KPI being tracked (e.g., "Customer Satisfaction", "Revenue Growth")
    /// </summary>
    public string KPIName { get; set; } = string.Empty;

    /// <summary>
    /// The month and year this KPI data represents
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Actual value of the KPI for this time period
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Target or goal value for this KPI
    /// </summary>
    public decimal Target { get; set; }

    /// <summary>
    /// Variance from target as a percentage (positive = above target, negative = below target)
    /// </summary>
    public decimal VariancePercentage { get; set; }
}

// Report models

/// <summary>
/// Report generation request model containing all parameters needed to generate custom reports.
/// Used by the reporting service to create tailored analytics reports based on user requirements.
/// </summary>
/// <remarks>
/// Supports flexible report generation with date ranges, entity filtering, and format options.
/// Essential for creating customized reports for different stakeholders and use cases.
/// </remarks>
public class ReportRequest
{
    /// <summary>
    /// Type of report to generate (Customer, Lead, Project, Financial, etc.)
    /// </summary>
    public ReportType Type { get; set; }

    /// <summary>
    /// Start date for the report data range (inclusive)
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date for the report data range (inclusive)
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Optional list of specific customer IDs to include in the report (empty = all customers)
    /// </summary>
    public List<Guid> CustomerIds { get; set; } = new();

    /// <summary>
    /// Optional list of specific staff member IDs to include in the report (empty = all staff)
    /// </summary>
    public List<Guid> StaffIds { get; set; } = new();

    /// <summary>
    /// Optional list of specific project IDs to include in the report (empty = all projects)
    /// </summary>
    public List<Guid> ProjectIds { get; set; } = new();

    /// <summary>
    /// Desired output format for the report (PDF, Excel, CSV, JSON)
    /// </summary>
    public ReportFormat Format { get; set; } = ReportFormat.PDF;

    /// <summary>
    /// Whether to include charts and visualizations in the report
    /// </summary>
    public bool IncludeCharts { get; set; } = true;

    /// <summary>
    /// Whether to include detailed data tables in addition to summary information
    /// </summary>
    public bool IncludeDetails { get; set; } = true;
}

/// <summary>
/// Generated report result containing the report data and metadata.
/// Used to store and manage generated reports for download and archival purposes.
/// </summary>
/// <remarks>
/// Contains the actual report data along with metadata for tracking, downloading,
/// and managing report lifecycle including expiration and cleanup.
/// </remarks>
public class ReportResult
{
    /// <summary>
    /// Unique identifier for this generated report
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Human-readable title of the report
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Type of report that was generated
    /// </summary>
    public ReportType Type { get; set; }

    /// <summary>
    /// Timestamp when the report was generated
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Name or identifier of the user who requested the report
    /// </summary>
    public string GeneratedBy { get; set; } = string.Empty;

    /// <summary>
    /// Format of the generated report file
    /// </summary>
    public ReportFormat Format { get; set; }

    /// <summary>
    /// Binary data of the generated report file
    /// </summary>
    public byte[] Data { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Suggested filename for downloading the report
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Size of the report file in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Current status of the report (Generating, Generated, Failed, Expired)
    /// </summary>
    public ReportStatus Status { get; set; } = ReportStatus.Generated;
}

/// <summary>
/// Enumeration of available report types for the analytics reporting system.
/// Defines the different categories of reports that can be generated.
/// </summary>
/// <remarks>
/// Used by the reporting service to determine report content, layout, and data sources.
/// Each type corresponds to a specific report template and data aggregation logic.
/// </remarks>
public enum ReportType
{
    /// <summary>Customer-focused report including acquisition, retention, and value metrics</summary>
    CustomerReport,

    /// <summary>Lead management report including pipeline, conversion, and source analysis</summary>
    LeadReport,

    /// <summary>Project performance report including budget, timeline, and completion metrics</summary>
    ProjectReport,

    /// <summary>Financial report including revenue, expenses, and profitability analysis</summary>
    FinancialReport,

    /// <summary>Staff performance report including productivity and workload metrics</summary>
    StaffPerformanceReport,

    /// <summary>High-level executive summary with key business metrics and KPIs</summary>
    ExecutiveSummary,

    /// <summary>Custom report with user-defined parameters and data selections</summary>
    CustomReport
}

/// <summary>
/// Enumeration of supported report output formats.
/// Defines the file formats available for report generation and download.
/// </summary>
/// <remarks>
/// Used by the reporting service to determine output formatting and file generation logic.
/// Each format has specific use cases and target audiences.
/// </remarks>
public enum ReportFormat
{
    /// <summary>Portable Document Format - ideal for formal reports and presentations</summary>
    PDF,

    /// <summary>Microsoft Excel format - suitable for data analysis and manipulation</summary>
    Excel,

    /// <summary>Comma-Separated Values - for data import/export and integration</summary>
    CSV,

    /// <summary>JavaScript Object Notation - for API consumption and web applications</summary>
    JSON
}

/// <summary>
/// Enumeration of report generation and lifecycle statuses.
/// Tracks the current state of report generation and availability.
/// </summary>
/// <remarks>
/// Used by the reporting service to manage report lifecycle, user notifications,
/// and cleanup processes for expired or failed reports.
/// </remarks>
public enum ReportStatus
{
    /// <summary>Report is currently being generated</summary>
    Generating,

    /// <summary>Report has been successfully generated and is available for download</summary>
    Generated,

    /// <summary>Report generation failed due to an error</summary>
    Failed,

    /// <summary>Report has expired and is no longer available for download</summary>
    Expired
}

// Chart data models

/// <summary>
/// Basic chart data model for simple charts like pie charts, bar charts, and donut charts.
/// Represents a single data point with label, value, and visual styling information.
/// </summary>
/// <remarks>
/// Used by chart components to render visual analytics data. Supports custom styling
/// and metadata for enhanced chart functionality and interactivity.
/// </remarks>
public class ChartData
{
    /// <summary>
    /// Display label for this data point (e.g., "Q1 Sales", "Marketing Leads")
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Numeric value for this data point
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Color code (hex, RGB, or named color) for visual representation
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata for enhanced chart functionality (tooltips, drill-down data, etc.)
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Time series data model for line charts and trend analysis over time.
/// Represents data points with temporal dimension for tracking changes over time.
/// </summary>
/// <remarks>
/// Essential for trend analysis, forecasting, and historical performance tracking.
/// Used in dashboard charts showing performance over time periods.
/// </remarks>
public class TimeSeriesData
{
    /// <summary>
    /// Date and time for this data point
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Numeric value at this point in time
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Series name for multi-series charts (e.g., "Revenue", "Expenses", "Profit")
    /// </summary>
    public string Series { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata for enhanced chart functionality and context
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Comparison data model for showing period-over-period changes and trends.
/// Used for displaying growth rates, variance analysis, and performance comparisons.
/// </summary>
/// <remarks>
/// Critical for executive dashboards and performance monitoring, showing how
/// current performance compares to previous periods with trend indicators.
/// </remarks>
public class ComparisonData
{
    /// <summary>
    /// Category or metric name being compared (e.g., "Revenue", "Customer Count")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Current period value
    /// </summary>
    public decimal CurrentValue { get; set; }

    /// <summary>
    /// Previous period value for comparison
    /// </summary>
    public decimal PreviousValue { get; set; }

    /// <summary>
    /// Percentage change from previous to current period
    /// </summary>
    public decimal ChangePercentage { get; set; }

    /// <summary>
    /// Trend direction indicator: "Up", "Down", or "Stable"
    /// </summary>
    public string Trend { get; set; } = string.Empty;
}

/// <summary>
/// Static class containing default values, constants, and configuration settings for the analytics system.
/// Provides consistent styling, formatting, and target values across all analytics components.
/// </summary>
/// <remarks>
/// Centralizes configuration to ensure consistency across charts, reports, and dashboards.
/// Simplifies maintenance and allows for easy customization of analytics appearance and behavior.
/// </remarks>
public static class AnalyticsDefaults
{
    /// <summary>
    /// Standard color palette for charts and visualizations.
    /// Provides consistent branding and accessibility-friendly colors across all analytics components.
    /// </summary>
    /// <remarks>
    /// Colors are chosen for optimal contrast and accessibility. Used by chart components
    /// to ensure consistent visual branding throughout the application.
    /// </remarks>
    public static readonly Dictionary<string, string> ChartColors = new()
    {
        { "Primary", "#594AE2" },    // Main brand color for primary data series
        { "Secondary", "#FF6B6B" },  // Secondary brand color for comparative data
        { "Success", "#51CF66" },    // Green for positive metrics and success indicators
        { "Warning", "#FFD43B" },    // Yellow for warning states and attention items
        { "Error", "#FF6B6B" },      // Red for error states and negative metrics
        { "Info", "#339AF0" },       // Blue for informational data and neutral metrics
        { "Dark", "#495057" },       // Dark gray for text and borders
        { "Light", "#F8F9FA" }       // Light gray for backgrounds and subtle elements
    };

    /// <summary>
    /// Abbreviated month names for chart labels and time series displays.
    /// Provides consistent month formatting across all temporal analytics components.
    /// </summary>
    /// <remarks>
    /// Used in chart axes, report headers, and trend analysis displays to maintain
    /// consistent date formatting and save space in visualizations.
    /// </remarks>
    public static readonly List<string> MonthNames = new()
    {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    /// <summary>
    /// Default target values for key performance indicators (KPIs).
    /// Provides baseline targets for performance evaluation and variance analysis.
    /// </summary>
    /// <remarks>
    /// These targets can be customized per organization but provide sensible defaults
    /// for common business metrics. Used in KPI dashboards and performance reports.
    /// </remarks>
    public static readonly Dictionary<string, decimal> DefaultTargets = new()
    {
        { "LeadConversionRate", 25.0m },      // Target: 25% lead to customer conversion
        { "CustomerSatisfaction", 85.0m },    // Target: 85% customer satisfaction score
        { "ProjectDeliveryRate", 90.0m },     // Target: 90% on-time project delivery
        { "PaymentCollectionRate", 95.0m },   // Target: 95% invoice collection rate
        { "ProfitMargin", 20.0m }             // Target: 20% profit margin
    };
}
