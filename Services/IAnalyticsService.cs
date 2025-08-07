using BlazorControlPanel.Models;
using Blazored.LocalStorage;

namespace BlazorControlPanel.Services;

public interface IAnalyticsService
{
    Task<DashboardAnalytics> GetDashboardAnalyticsAsync();
    Task<CustomerAnalytics> GetCustomerAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<LeadAnalytics> GetLeadAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ProjectAnalytics> GetProjectAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<FinancialAnalytics> GetFinancialAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<StaffAnalytics> GetStaffAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<PerformanceMetrics> GetPerformanceMetricsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ChartData>> GetChartDataAsync(string chartType, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<TimeSeriesData>> GetTimeSeriesDataAsync(string metric, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ComparisonData>> GetComparisonDataAsync(string category, DateTime? startDate = null, DateTime? endDate = null);
    Task<ReportResult> GenerateReportAsync(ReportRequest request);
    Task<List<ReportResult>> GetReportHistoryAsync();
    Task<byte[]> GetReportDataAsync(Guid reportId);
}

public interface IReportService
{
    Task<ReportResult> GenerateCustomerReportAsync(ReportRequest request);
    Task<ReportResult> GenerateLeadReportAsync(ReportRequest request);
    Task<ReportResult> GenerateProjectReportAsync(ReportRequest request);
    Task<ReportResult> GenerateFinancialReportAsync(ReportRequest request);
    Task<ReportResult> GenerateStaffPerformanceReportAsync(ReportRequest request);
    Task<ReportResult> GenerateExecutiveSummaryAsync(ReportRequest request);
    Task<byte[]> ExportToPdfAsync(object data, string templateName);
    Task<byte[]> ExportToExcelAsync(object data, string sheetName);
    Task<byte[]> ExportToCsvAsync(object data);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly ICustomerService _customerService;
    private readonly ILeadService _leadService;
    private readonly IProjectService _projectService;
    private readonly IEstimateService _estimateService;
    private readonly IInvoiceService _invoiceService;
    private readonly IStaffService _staffService;
    private readonly ILocalStorageService _localStorage;
    private const string REPORTS_KEY = "analytics_reports";

    public AnalyticsService(
        ICustomerService customerService,
        ILeadService leadService,
        IProjectService projectService,
        IEstimateService estimateService,
        IInvoiceService invoiceService,
        IStaffService staffService,
        ILocalStorageService localStorage)
    {
        _customerService = customerService;
        _leadService = leadService;
        _projectService = projectService;
        _estimateService = estimateService;
        _invoiceService = invoiceService;
        _staffService = staffService;
        _localStorage = localStorage;
    }

    public async Task<DashboardAnalytics> GetDashboardAnalyticsAsync()
    {
        var analytics = new DashboardAnalytics
        {
            Customers = await GetCustomerAnalyticsAsync(),
            Leads = await GetLeadAnalyticsAsync(),
            Projects = await GetProjectAnalyticsAsync(),
            Financial = await GetFinancialAnalyticsAsync(),
            Staff = await GetStaffAnalyticsAsync(),
            Performance = await GetPerformanceMetricsAsync(),
            GeneratedAt = DateTime.UtcNow
        };

        return analytics;
    }

    public async Task<CustomerAnalytics> GetCustomerAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var customers = await _customerService.GetAllCustomersAsync();
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        
        var endDateValue = endDate ?? DateTime.UtcNow;
        var startDateValue = startDate ?? endDateValue.AddMonths(-12);

        var filteredCustomers = customers.Where(c => c.CreatedAt >= startDateValue && c.CreatedAt <= endDateValue).ToList();
        var currentMonth = DateTime.UtcNow.Month;
        var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;

        var newThisMonth = customers.Count(c => c.CreatedAt.Month == currentMonth && c.CreatedAt.Year == DateTime.UtcNow.Year);
        var newLastMonth = customers.Count(c => c.CreatedAt.Month == lastMonth);

        var customerRevenue = invoices
            .Where(i => i.Status == InvoiceStatus.Paid)
            .GroupBy(i => i.CustomerId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Total));

        return new CustomerAnalytics
        {
            TotalCustomers = customers.Count,
            ActiveCustomers = customers.Count(c => c.Status == CustomerStatus.Active),
            NewCustomersThisMonth = newThisMonth,
            NewCustomersLastMonth = newLastMonth,
            CustomerGrowthRate = newLastMonth > 0 ? ((decimal)(newThisMonth - newLastMonth) / newLastMonth) * 100 : 0,
            AverageCustomerValue = customerRevenue.Any() ? customerRevenue.Values.Average() : 0,
            CustomersByType = customers.GroupBy(c => c.Type).ToDictionary(g => g.Key, g => g.Count()),
            CustomersByStatus = customers.GroupBy(c => c.Status).ToDictionary(g => g.Key, g => g.Count()),
            MonthlyTrends = GenerateCustomerTrends(customers, invoices, startDateValue, endDateValue),
            TopCustomers = GenerateTopCustomers(customers, customerRevenue)
        };
    }

    public async Task<LeadAnalytics> GetLeadAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var leads = await _leadService.GetAllLeadsAsync();
        var statistics = await _leadService.GetLeadStatisticsAsync();
        
        var endDateValue = endDate ?? DateTime.UtcNow;
        var startDateValue = startDate ?? endDateValue.AddMonths(-12);

        var filteredLeads = leads.Where(l => l.CreatedAt >= startDateValue && l.CreatedAt <= endDateValue).ToList();
        var currentMonth = DateTime.UtcNow.Month;

        return new LeadAnalytics
        {
            TotalLeads = statistics.TotalLeads,
            ActiveLeads = leads.Count(l => l.Status != LeadStatus.Converted && l.Status != LeadStatus.Lost),
            ConvertedLeads = statistics.ConvertedLeads,
            NewLeadsThisMonth = leads.Count(l => l.CreatedAt.Month == currentMonth && l.CreatedAt.Year == DateTime.UtcNow.Year),
            ConversionRate = statistics.ConversionRate,
            AverageLeadValue = statistics.AverageLeadValue,
            TotalPipelineValue = statistics.TotalPipelineValue,
            OverdueFollowUps = statistics.OverdueFollowUps,
            LeadsByStatus = leads.GroupBy(l => l.Status).ToDictionary(g => g.Key, g => g.Count()),
            LeadsBySource = leads.GroupBy(l => l.Source).ToDictionary(g => g.Key, g => g.Count()),
            LeadsByPriority = leads.GroupBy(l => l.Priority).ToDictionary(g => g.Key, g => g.Count()),
            MonthlyTrends = GenerateLeadTrends(leads, startDateValue, endDateValue),
            SourcePerformance = GenerateSourcePerformance(leads)
        };
    }

    public async Task<ProjectAnalytics> GetProjectAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var projects = await _projectService.GetAllProjectsAsync();
        var statistics = await _projectService.GetProjectStatisticsAsync();
        
        var endDateValue = endDate ?? DateTime.UtcNow;
        var startDateValue = startDate ?? endDateValue.AddMonths(-12);

        return new ProjectAnalytics
        {
            TotalProjects = statistics.TotalProjects,
            ActiveProjects = statistics.ActiveProjects,
            CompletedProjects = statistics.CompletedProjects,
            OverdueProjects = statistics.OverdueProjects,
            TotalBudget = statistics.TotalBudget,
            TotalActualCost = statistics.TotalActualCost,
            BudgetVariance = statistics.BudgetVariance,
            AverageProjectCompletion = statistics.AverageProjectCompletion,
            TotalTasks = statistics.TotalTasks,
            CompletedTasks = statistics.CompletedTasks,
            OverdueTasks = statistics.OverdueTasks,
            TaskCompletionRate = statistics.TotalTasks > 0 ? ((decimal)statistics.CompletedTasks / statistics.TotalTasks) * 100 : 0,
            ProjectsByStatus = statistics.ProjectsByStatus,
            ProjectsByType = statistics.ProjectsByType,
            MonthlyTrends = GenerateProjectTrends(projects, startDateValue, endDateValue),
            ProjectPerformance = GenerateProjectPerformance(projects)
        };
    }

    public async Task<FinancialAnalytics> GetFinancialAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        var estimates = await _estimateService.GetAllEstimatesAsync();
        
        var endDateValue = endDate ?? DateTime.UtcNow;
        var startDateValue = startDate ?? endDateValue.AddMonths(-12);

        var paidInvoices = invoices.Where(i => i.Status == InvoiceStatus.Paid).ToList();
        var currentMonth = DateTime.UtcNow.Month;
        var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;

        var revenueThisMonth = paidInvoices
            .Where(i => i.PaidDate?.Month == currentMonth && i.PaidDate?.Year == DateTime.UtcNow.Year)
            .Sum(i => i.Total);

        var revenueLastMonth = paidInvoices
            .Where(i => i.PaidDate?.Month == lastMonth)
            .Sum(i => i.Total);

        return new FinancialAnalytics
        {
            TotalRevenue = paidInvoices.Sum(i => i.Total),
            RevenueThisMonth = revenueThisMonth,
            RevenueLastMonth = revenueLastMonth,
            RevenueGrowthRate = revenueLastMonth > 0 ? ((revenueThisMonth - revenueLastMonth) / revenueLastMonth) * 100 : 0,
            TotalEstimateValue = estimates.Sum(e => e.Total),
            TotalInvoiceValue = invoices.Sum(i => i.Total),
            TotalPaidAmount = paidInvoices.Sum(i => i.Total),
            TotalOutstanding = invoices.Where(i => i.Status != InvoiceStatus.Paid).Sum(i => i.Total),
            AverageInvoiceValue = invoices.Any() ? invoices.Average(i => i.Total) : 0,
            TotalEstimates = estimates.Count,
            TotalInvoices = invoices.Count,
            PaidInvoices = paidInvoices.Count,
            OverdueInvoices = invoices.Count(i => i.IsOverdue),
            PaymentCollectionRate = invoices.Any() ? ((decimal)paidInvoices.Count / invoices.Count) * 100 : 0,
            MonthlyRevenue = GenerateMonthlyRevenue(invoices, startDateValue, endDateValue),
            PaymentTrends = GeneratePaymentTrends(invoices, startDateValue, endDateValue),
            RevenueByCustomer = GenerateRevenueByCustomer(paidInvoices)
        };
    }

    public async Task<StaffAnalytics> GetStaffAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var staff = await _staffService.GetAllStaffAsync();
        var projects = await _projectService.GetAllProjectsAsync();
        var leads = await _leadService.GetAllLeadsAsync();

        return new StaffAnalytics
        {
            TotalStaff = staff.Count,
            ActiveStaff = staff.Count(s => s.IsActive),
            AverageProjectsPerStaff = staff.Any() ? (decimal)projects.Count / staff.Count : 0,
            AverageLeadsPerStaff = staff.Any() ? (decimal)leads.Count / staff.Count : 0,
            StaffByRole = staff.GroupBy(s => s.JobTitle).ToDictionary(g => g.Key, g => g.Count()),
            StaffPerformance = GenerateStaffPerformance(staff, projects, leads),
            StaffWorkload = GenerateStaffWorkload(staff, projects, leads)
        };
    }

    public async Task<PerformanceMetrics> GetPerformanceMetricsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var projects = await _projectService.GetAllProjectsAsync();
        var leads = await _leadService.GetAllLeadsAsync();
        var invoices = await _invoiceService.GetAllInvoicesAsync();

        var completedProjects = projects.Where(p => p.Status == ProjectStatus.Completed).ToList();
        var onTimeProjects = completedProjects.Where(p => p.ActualEndDate <= p.EndDate).Count();
        var convertedLeads = leads.Where(l => l.Status == LeadStatus.Converted).Count();
        var paidInvoices = invoices.Where(i => i.Status == InvoiceStatus.Paid).ToList();

        return new PerformanceMetrics
        {
            CustomerSatisfactionScore = 85.0m, // This would come from surveys/feedback
            ProjectDeliveryRate = completedProjects.Any() ? ((decimal)onTimeProjects / completedProjects.Count) * 100 : 0,
            LeadResponseTime = 2.5m, // Average hours to respond to leads
            AverageProjectDuration = completedProjects.Any() ? 
                (decimal)completedProjects.Average(p => (p.ActualEndDate ?? p.EndDate ?? DateTime.UtcNow).Subtract(p.ActualStartDate ?? p.StartDate).Days) : 0,
            ResourceUtilization = 75.0m, // Percentage of staff capacity utilized
            ProfitMargin = paidInvoices.Any() ?
                ((paidInvoices.Sum(i => i.Total) - projects.Sum(p => p.ActualCost)) / paidInvoices.Sum(i => i.Total)) * 100 : 0,
            KPITrends = GenerateKPITrends(startDate, endDate)
        };
    }

    public async Task<List<ChartData>> GetChartDataAsync(string chartType, DateTime? startDate = null, DateTime? endDate = null)
    {
        return chartType.ToLower() switch
        {
            "customer-status" => await GetCustomerStatusChartData(),
            "lead-source" => await GetLeadSourceChartData(),
            "project-status" => await GetProjectStatusChartData(),
            "revenue-by-month" => await GetRevenueByMonthChartData(startDate, endDate),
            _ => new List<ChartData>()
        };
    }

    public async Task<List<TimeSeriesData>> GetTimeSeriesDataAsync(string metric, DateTime? startDate = null, DateTime? endDate = null)
    {
        var endDateValue = endDate ?? DateTime.UtcNow;
        var startDateValue = startDate ?? endDateValue.AddMonths(-12);

        return metric.ToLower() switch
        {
            "revenue" => await GetRevenueTimeSeries(startDateValue, endDateValue),
            "leads" => await GetLeadsTimeSeries(startDateValue, endDateValue),
            "projects" => await GetProjectsTimeSeries(startDateValue, endDateValue),
            _ => new List<TimeSeriesData>()
        };
    }

    public async Task<List<ComparisonData>> GetComparisonDataAsync(string category, DateTime? startDate = null, DateTime? endDate = null)
    {
        return category.ToLower() switch
        {
            "monthly-performance" => await GetMonthlyPerformanceComparison(),
            "staff-performance" => await GetStaffPerformanceComparison(),
            _ => new List<ComparisonData>()
        };
    }

    public async Task<ReportResult> GenerateReportAsync(ReportRequest request)
    {
        var report = new ReportResult
        {
            Type = request.Type,
            Format = request.Format,
            GeneratedAt = DateTime.UtcNow,
            GeneratedBy = "System", // This would come from current user context
            Status = ReportStatus.Generating
        };

        try
        {
            // Generate report data based on type
            var data = request.Type switch
            {
                ReportType.CustomerReport => await GenerateCustomerReportData(request),
                ReportType.LeadReport => await GenerateLeadReportData(request),
                ReportType.ProjectReport => await GenerateProjectReportData(request),
                ReportType.FinancialReport => await GenerateFinancialReportData(request),
                ReportType.StaffPerformanceReport => await GenerateStaffReportData(request),
                ReportType.ExecutiveSummary => await GenerateExecutiveSummaryData(request),
                _ => new { Message = "Report type not supported" }
            };

            // Convert to requested format
            report.Data = request.Format switch
            {
                ReportFormat.PDF => await ConvertToPdf(data, request.Type.ToString()),
                ReportFormat.Excel => await ConvertToExcel(data, request.Type.ToString()),
                ReportFormat.CSV => await ConvertToCsv(data),
                ReportFormat.JSON => System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(data)),
                _ => Array.Empty<byte>()
            };

            report.FileName = $"{request.Type}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{request.Format.ToString().ToLower()}";
            report.FileSize = report.Data.Length;
            report.Status = ReportStatus.Generated;
            report.Title = $"{request.Type} Report - {DateTime.UtcNow:MMM dd, yyyy}";

            // Save report to storage
            await SaveReportAsync(report);
        }
        catch (Exception)
        {
            report.Status = ReportStatus.Failed;
        }

        return report;
    }

    public async Task<List<ReportResult>> GetReportHistoryAsync()
    {
        try
        {
            var reports = await _localStorage.GetItemAsync<List<ReportResult>>(REPORTS_KEY);
            return reports ?? new List<ReportResult>();
        }
        catch
        {
            return new List<ReportResult>();
        }
    }

    public async Task<byte[]> GetReportDataAsync(Guid reportId)
    {
        var reports = await GetReportHistoryAsync();
        var report = reports.FirstOrDefault(r => r.Id == reportId);
        return report?.Data ?? Array.Empty<byte>();
    }

    // Private helper methods would be implemented here
    private List<CustomerTrend> GenerateCustomerTrends(List<Customer> customers, List<Invoice> invoices, DateTime startDate, DateTime endDate)
    {
        // Implementation for generating customer trends
        return new List<CustomerTrend>();
    }

    private List<TopCustomer> GenerateTopCustomers(List<Customer> customers, Dictionary<Guid, decimal> customerRevenue)
    {
        // Implementation for generating top customers
        return new List<TopCustomer>();
    }

    private List<LeadTrend> GenerateLeadTrends(List<Lead> leads, DateTime startDate, DateTime endDate)
    {
        // Implementation for generating lead trends
        return new List<LeadTrend>();
    }

    private List<SourcePerformance> GenerateSourcePerformance(List<Lead> leads)
    {
        // Implementation for generating source performance
        return new List<SourcePerformance>();
    }

    private List<ProjectTrend> GenerateProjectTrends(List<Project> projects, DateTime startDate, DateTime endDate)
    {
        // Implementation for generating project trends
        return new List<ProjectTrend>();
    }

    private List<ProjectPerformance> GenerateProjectPerformance(List<Project> projects)
    {
        // Implementation for generating project performance
        return new List<ProjectPerformance>();
    }

    private List<MonthlyRevenue> GenerateMonthlyRevenue(List<Invoice> invoices, DateTime startDate, DateTime endDate)
    {
        // Implementation for generating monthly revenue
        return new List<MonthlyRevenue>();
    }

    private List<PaymentTrend> GeneratePaymentTrends(List<Invoice> invoices, DateTime startDate, DateTime endDate)
    {
        // Implementation for generating payment trends
        return new List<PaymentTrend>();
    }

    private Dictionary<string, decimal> GenerateRevenueByCustomer(List<Invoice> paidInvoices)
    {
        // Implementation for generating revenue by customer
        return new Dictionary<string, decimal>();
    }

    private List<StaffPerformance> GenerateStaffPerformance(List<Staff> staff, List<Project> projects, List<Lead> leads)
    {
        // Implementation for generating staff performance
        return new List<StaffPerformance>();
    }

    private List<StaffWorkload> GenerateStaffWorkload(List<Staff> staff, List<Project> projects, List<Lead> leads)
    {
        // Implementation for generating staff workload
        return new List<StaffWorkload>();
    }

    private List<KPITrend> GenerateKPITrends(DateTime? startDate, DateTime? endDate)
    {
        // Implementation for generating KPI trends
        return new List<KPITrend>();
    }

    private async Task<List<ChartData>> GetCustomerStatusChartData()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return customers.GroupBy(c => c.Status)
            .Select(g => new ChartData
            {
                Label = g.Key.ToString(),
                Value = g.Count(),
                Color = GetStatusColor(g.Key.ToString())
            }).ToList();
    }

    private async Task<List<ChartData>> GetLeadSourceChartData()
    {
        var leads = await _leadService.GetAllLeadsAsync();
        return leads.GroupBy(l => l.Source)
            .Select(g => new ChartData
            {
                Label = g.Key.ToString(),
                Value = g.Count(),
                Color = GetSourceColor(g.Key.ToString())
            }).ToList();
    }

    private async Task<List<ChartData>> GetProjectStatusChartData()
    {
        var projects = await _projectService.GetAllProjectsAsync();
        return projects.GroupBy(p => p.Status)
            .Select(g => new ChartData
            {
                Label = g.Key.ToString(),
                Value = g.Count(),
                Color = GetProjectStatusColor(g.Key.ToString())
            }).ToList();
    }

    private async Task<List<ChartData>> GetRevenueByMonthChartData(DateTime? startDate, DateTime? endDate)
    {
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        var paidInvoices = invoices.Where(i => i.Status == InvoiceStatus.Paid && i.PaidDate.HasValue).ToList();
        
        return paidInvoices.GroupBy(i => new { i.PaidDate!.Value.Year, i.PaidDate.Value.Month })
            .Select(g => new ChartData
            {
                Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                Value = g.Sum(i => i.Total),
                Color = AnalyticsDefaults.ChartColors["Primary"]
            }).ToList();
    }

    private async Task<List<TimeSeriesData>> GetRevenueTimeSeries(DateTime startDate, DateTime endDate)
    {
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        var paidInvoices = invoices.Where(i => i.Status == InvoiceStatus.Paid && i.PaidDate.HasValue).ToList();
        
        return paidInvoices.GroupBy(i => i.PaidDate!.Value.Date)
            .Select(g => new TimeSeriesData
            {
                Date = g.Key,
                Value = g.Sum(i => i.Total),
                Series = "Revenue"
            }).ToList();
    }

    private async Task<List<TimeSeriesData>> GetLeadsTimeSeries(DateTime startDate, DateTime endDate)
    {
        var leads = await _leadService.GetAllLeadsAsync();
        
        return leads.GroupBy(l => l.CreatedAt.Date)
            .Select(g => new TimeSeriesData
            {
                Date = g.Key,
                Value = g.Count(),
                Series = "New Leads"
            }).ToList();
    }

    private async Task<List<TimeSeriesData>> GetProjectsTimeSeries(DateTime startDate, DateTime endDate)
    {
        var projects = await _projectService.GetAllProjectsAsync();
        
        return projects.GroupBy(p => p.CreatedAt.Date)
            .Select(g => new TimeSeriesData
            {
                Date = g.Key,
                Value = g.Count(),
                Series = "New Projects"
            }).ToList();
    }

    private async Task<List<ComparisonData>> GetMonthlyPerformanceComparison()
    {
        // Implementation for monthly performance comparison
        return new List<ComparisonData>();
    }

    private async Task<List<ComparisonData>> GetStaffPerformanceComparison()
    {
        // Implementation for staff performance comparison
        return new List<ComparisonData>();
    }

    private async Task<object> GenerateCustomerReportData(ReportRequest request)
    {
        var customers = await _customerService.GetAllCustomersAsync();
        var analytics = await GetCustomerAnalyticsAsync(request.StartDate, request.EndDate);
        
        return new
        {
            Summary = analytics,
            Customers = customers.Where(c => request.CustomerIds.Count == 0 || request.CustomerIds.Contains(c.Id)),
            GeneratedAt = DateTime.UtcNow,
            Period = $"{request.StartDate:MMM dd, yyyy} - {request.EndDate:MMM dd, yyyy}"
        };
    }

    private async Task<object> GenerateLeadReportData(ReportRequest request)
    {
        var leads = await _leadService.GetAllLeadsAsync();
        var analytics = await GetLeadAnalyticsAsync(request.StartDate, request.EndDate);
        
        return new
        {
            Summary = analytics,
            Leads = leads.Where(l => l.CreatedAt >= request.StartDate && l.CreatedAt <= request.EndDate),
            GeneratedAt = DateTime.UtcNow,
            Period = $"{request.StartDate:MMM dd, yyyy} - {request.EndDate:MMM dd, yyyy}"
        };
    }

    private async Task<object> GenerateProjectReportData(ReportRequest request)
    {
        var projects = await _projectService.GetAllProjectsAsync();
        var analytics = await GetProjectAnalyticsAsync(request.StartDate, request.EndDate);
        
        return new
        {
            Summary = analytics,
            Projects = projects.Where(p => request.ProjectIds.Count == 0 || request.ProjectIds.Contains(p.Id)),
            GeneratedAt = DateTime.UtcNow,
            Period = $"{request.StartDate:MMM dd, yyyy} - {request.EndDate:MMM dd, yyyy}"
        };
    }

    private async Task<object> GenerateFinancialReportData(ReportRequest request)
    {
        var analytics = await GetFinancialAnalyticsAsync(request.StartDate, request.EndDate);
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        var estimates = await _estimateService.GetAllEstimatesAsync();
        
        return new
        {
            Summary = analytics,
            Invoices = invoices.Where(i => i.CreatedAt >= request.StartDate && i.CreatedAt <= request.EndDate),
            Estimates = estimates.Where(e => e.CreatedAt >= request.StartDate && e.CreatedAt <= request.EndDate),
            GeneratedAt = DateTime.UtcNow,
            Period = $"{request.StartDate:MMM dd, yyyy} - {request.EndDate:MMM dd, yyyy}"
        };
    }

    private async Task<object> GenerateStaffReportData(ReportRequest request)
    {
        var analytics = await GetStaffAnalyticsAsync(request.StartDate, request.EndDate);
        var staff = await _staffService.GetAllStaffAsync();
        
        return new
        {
            Summary = analytics,
            Staff = staff.Where(s => request.StaffIds.Count == 0 || request.StaffIds.Contains(s.Id)),
            GeneratedAt = DateTime.UtcNow,
            Period = $"{request.StartDate:MMM dd, yyyy} - {request.EndDate:MMM dd, yyyy}"
        };
    }

    private async Task<object> GenerateExecutiveSummaryData(ReportRequest request)
    {
        var dashboard = await GetDashboardAnalyticsAsync();
        
        return new
        {
            ExecutiveSummary = dashboard,
            GeneratedAt = DateTime.UtcNow,
            Period = $"{request.StartDate:MMM dd, yyyy} - {request.EndDate:MMM dd, yyyy}"
        };
    }

    private async Task<byte[]> ConvertToPdf(object data, string templateName)
    {
        // Implementation for PDF conversion
        // This would use a library like iTextSharp or similar
        return System.Text.Encoding.UTF8.GetBytes($"PDF Report: {templateName}");
    }

    private async Task<byte[]> ConvertToExcel(object data, string sheetName)
    {
        // Implementation for Excel conversion
        // This would use a library like EPPlus or similar
        return System.Text.Encoding.UTF8.GetBytes($"Excel Report: {sheetName}");
    }

    private async Task<byte[]> ConvertToCsv(object data)
    {
        // Implementation for CSV conversion
        return System.Text.Encoding.UTF8.GetBytes("CSV Report Data");
    }

    private async Task SaveReportAsync(ReportResult report)
    {
        var reports = await GetReportHistoryAsync();
        reports.Add(report);
        
        // Keep only last 50 reports
        if (reports.Count > 50)
        {
            reports = reports.OrderByDescending(r => r.GeneratedAt).Take(50).ToList();
        }
        
        await _localStorage.SetItemAsync(REPORTS_KEY, reports);
    }

    private string GetStatusColor(string status) => status switch
    {
        "Active" => AnalyticsDefaults.ChartColors["Success"],
        "Inactive" => AnalyticsDefaults.ChartColors["Error"],
        "Prospect" => AnalyticsDefaults.ChartColors["Warning"],
        _ => AnalyticsDefaults.ChartColors["Primary"]
    };

    private string GetSourceColor(string source) => source switch
    {
        "Website" => AnalyticsDefaults.ChartColors["Primary"],
        "Referral" => AnalyticsDefaults.ChartColors["Success"],
        "SocialMedia" => AnalyticsDefaults.ChartColors["Info"],
        "Email" => AnalyticsDefaults.ChartColors["Warning"],
        _ => AnalyticsDefaults.ChartColors["Secondary"]
    };

    private string GetProjectStatusColor(string status) => status switch
    {
        "Planning" => AnalyticsDefaults.ChartColors["Info"],
        "InProgress" => AnalyticsDefaults.ChartColors["Primary"],
        "Completed" => AnalyticsDefaults.ChartColors["Success"],
        "OnHold" => AnalyticsDefaults.ChartColors["Warning"],
        "Cancelled" => AnalyticsDefaults.ChartColors["Error"],
        _ => AnalyticsDefaults.ChartColors["Dark"]
    };
}
