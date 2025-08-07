namespace BlazorControlPanel.Models;

public class DashboardAnalytics
{
    public CustomerAnalytics Customers { get; set; } = new();
    public LeadAnalytics Leads { get; set; } = new();
    public ProjectAnalytics Projects { get; set; } = new();
    public FinancialAnalytics Financial { get; set; } = new();
    public StaffAnalytics Staff { get; set; } = new();
    public PerformanceMetrics Performance { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class CustomerAnalytics
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int NewCustomersThisMonth { get; set; }
    public int NewCustomersLastMonth { get; set; }
    public decimal CustomerGrowthRate { get; set; }
    public decimal AverageCustomerValue { get; set; }
    public Dictionary<CustomerType, int> CustomersByType { get; set; } = new();
    public Dictionary<CustomerStatus, int> CustomersByStatus { get; set; } = new();
    public List<CustomerTrend> MonthlyTrends { get; set; } = new();
    public List<TopCustomer> TopCustomers { get; set; } = new();
}

public class LeadAnalytics
{
    public int TotalLeads { get; set; }
    public int ActiveLeads { get; set; }
    public int ConvertedLeads { get; set; }
    public int NewLeadsThisMonth { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal AverageLeadValue { get; set; }
    public decimal TotalPipelineValue { get; set; }
    public int OverdueFollowUps { get; set; }
    public Dictionary<LeadStatus, int> LeadsByStatus { get; set; } = new();
    public Dictionary<LeadSource, int> LeadsBySource { get; set; } = new();
    public Dictionary<LeadPriority, int> LeadsByPriority { get; set; } = new();
    public List<LeadTrend> MonthlyTrends { get; set; } = new();
    public List<SourcePerformance> SourcePerformance { get; set; } = new();
}

public class ProjectAnalytics
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int OverdueProjects { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalActualCost { get; set; }
    public decimal BudgetVariance { get; set; }
    public decimal AverageProjectCompletion { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public decimal TaskCompletionRate { get; set; }
    public Dictionary<ProjectStatus, int> ProjectsByStatus { get; set; } = new();
    public Dictionary<ProjectType, int> ProjectsByType { get; set; } = new();
    public List<ProjectTrend> MonthlyTrends { get; set; } = new();
    public List<ProjectPerformance> ProjectPerformance { get; set; } = new();
}

public class FinancialAnalytics
{
    public decimal TotalRevenue { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
    public decimal RevenueGrowthRate { get; set; }
    public decimal TotalEstimateValue { get; set; }
    public decimal TotalInvoiceValue { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal AverageInvoiceValue { get; set; }
    public int TotalEstimates { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal PaymentCollectionRate { get; set; }
    public List<MonthlyRevenue> MonthlyRevenue { get; set; } = new();
    public List<PaymentTrend> PaymentTrends { get; set; } = new();
    public Dictionary<string, decimal> RevenueByCustomer { get; set; } = new();
}

public class StaffAnalytics
{
    public int TotalStaff { get; set; }
    public int ActiveStaff { get; set; }
    public decimal AverageProjectsPerStaff { get; set; }
    public decimal AverageLeadsPerStaff { get; set; }
    public Dictionary<string, int> StaffByRole { get; set; } = new();
    public List<StaffPerformance> StaffPerformance { get; set; } = new();
    public List<StaffWorkload> StaffWorkload { get; set; } = new();
}

public class PerformanceMetrics
{
    public decimal CustomerSatisfactionScore { get; set; }
    public decimal ProjectDeliveryRate { get; set; }
    public decimal LeadResponseTime { get; set; }
    public decimal AverageProjectDuration { get; set; }
    public decimal ResourceUtilization { get; set; }
    public decimal ProfitMargin { get; set; }
    public List<KPITrend> KPITrends { get; set; } = new();
}

// Supporting classes for trends and performance
public class CustomerTrend
{
    public DateTime Month { get; set; }
    public int NewCustomers { get; set; }
    public int TotalCustomers { get; set; }
    public decimal Revenue { get; set; }
}

public class LeadTrend
{
    public DateTime Month { get; set; }
    public int NewLeads { get; set; }
    public int ConvertedLeads { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal PipelineValue { get; set; }
}

public class ProjectTrend
{
    public DateTime Month { get; set; }
    public int NewProjects { get; set; }
    public int CompletedProjects { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal CompletionRate { get; set; }
}

public class MonthlyRevenue
{
    public DateTime Month { get; set; }
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal Profit { get; set; }
    public decimal ProfitMargin { get; set; }
}

public class PaymentTrend
{
    public DateTime Month { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
    public decimal CollectionRate { get; set; }
}

public class TopCustomer
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int ProjectCount { get; set; }
    public DateTime LastActivity { get; set; }
}

public class SourcePerformance
{
    public LeadSource Source { get; set; }
    public int TotalLeads { get; set; }
    public int ConvertedLeads { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal AverageValue { get; set; }
    public decimal ROI { get; set; }
}

public class ProjectPerformance
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal BudgetVariance { get; set; }
    public decimal TimeVariance { get; set; }
    public decimal CompletionPercentage { get; set; }
    public bool IsOnTime { get; set; }
    public bool IsOnBudget { get; set; }
}

public class StaffPerformance
{
    public Guid StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public int ProjectsManaged { get; set; }
    public int LeadsAssigned { get; set; }
    public int TasksCompleted { get; set; }
    public decimal AverageProjectCompletion { get; set; }
    public decimal LeadConversionRate { get; set; }
    public decimal ProductivityScore { get; set; }
}

public class StaffWorkload
{
    public Guid StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public int ActiveProjects { get; set; }
    public int ActiveTasks { get; set; }
    public int ActiveLeads { get; set; }
    public decimal WorkloadScore { get; set; }
    public string WorkloadStatus { get; set; } = string.Empty; // Low, Normal, High, Overloaded
}

public class KPITrend
{
    public string KPIName { get; set; } = string.Empty;
    public DateTime Month { get; set; }
    public decimal Value { get; set; }
    public decimal Target { get; set; }
    public decimal VariancePercentage { get; set; }
}

// Report models
public class ReportRequest
{
    public ReportType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<Guid> CustomerIds { get; set; } = new();
    public List<Guid> StaffIds { get; set; } = new();
    public List<Guid> ProjectIds { get; set; } = new();
    public ReportFormat Format { get; set; } = ReportFormat.PDF;
    public bool IncludeCharts { get; set; } = true;
    public bool IncludeDetails { get; set; } = true;
}

public class ReportResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string GeneratedBy { get; set; } = string.Empty;
    public ReportFormat Format { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Generated;
}

public enum ReportType
{
    CustomerReport,
    LeadReport,
    ProjectReport,
    FinancialReport,
    StaffPerformanceReport,
    ExecutiveSummary,
    CustomReport
}

public enum ReportFormat
{
    PDF,
    Excel,
    CSV,
    JSON
}

public enum ReportStatus
{
    Generating,
    Generated,
    Failed,
    Expired
}

// Chart data models
public class ChartData
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Color { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class TimeSeriesData
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string Series { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ComparisonData
{
    public string Category { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public decimal PreviousValue { get; set; }
    public decimal ChangePercentage { get; set; }
    public string Trend { get; set; } = string.Empty; // Up, Down, Stable
}

public static class AnalyticsDefaults
{
    public static readonly Dictionary<string, string> ChartColors = new()
    {
        { "Primary", "#594AE2" },
        { "Secondary", "#FF6B6B" },
        { "Success", "#51CF66" },
        { "Warning", "#FFD43B" },
        { "Error", "#FF6B6B" },
        { "Info", "#339AF0" },
        { "Dark", "#495057" },
        { "Light", "#F8F9FA" }
    };

    public static readonly List<string> MonthNames = new()
    {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    public static readonly Dictionary<string, decimal> DefaultTargets = new()
    {
        { "LeadConversionRate", 25.0m },
        { "CustomerSatisfaction", 85.0m },
        { "ProjectDeliveryRate", 90.0m },
        { "PaymentCollectionRate", 95.0m },
        { "ProfitMargin", 20.0m }
    };
}
