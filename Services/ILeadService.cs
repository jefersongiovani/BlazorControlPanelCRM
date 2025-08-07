using BlazorControlPanel.Models;
using Blazored.LocalStorage;

namespace BlazorControlPanel.Services;

public interface ILeadService
{
    Task<List<Lead>> GetAllLeadsAsync();
    Task<Lead?> GetLeadByIdAsync(Guid id);
    Task<Lead> CreateLeadAsync(Lead lead);
    Task<Lead> UpdateLeadAsync(Lead lead);
    Task<bool> DeleteLeadAsync(Guid id);
    Task<List<Lead>> GetLeadsByStatusAsync(LeadStatus status);
    Task<List<Lead>> GetLeadsBySourceAsync(LeadSource source);
    Task<List<Lead>> GetLeadsByPriorityAsync(LeadPriority priority);
    Task<List<Lead>> GetLeadsByAssignedStaffAsync(Guid staffId);
    Task<List<Lead>> SearchLeadsAsync(string searchTerm);
    Task<Lead> ConvertLeadToCustomerAsync(Guid leadId, Customer customer);
    Task<Lead> UpdateLeadStatusAsync(Guid leadId, LeadStatus status);
    Task<Lead> AssignLeadToStaffAsync(Guid leadId, Guid staffId);
    Task<List<Lead>> GetOverdueLeadsAsync();
    Task<List<Lead>> GetLeadsRequiringFollowUpAsync();
    Task<LeadStatistics> GetLeadStatisticsAsync();
}

public interface ILeadActivityService
{
    Task<List<LeadActivity>> GetActivitiesByLeadAsync(Guid leadId);
    Task<LeadActivity> CreateActivityAsync(LeadActivity activity);
    Task<LeadActivity> UpdateActivityAsync(LeadActivity activity);
    Task<bool> DeleteActivityAsync(Guid activityId);
    Task<LeadActivity> CompleteActivityAsync(Guid activityId, string outcome);
    Task<List<LeadActivity>> GetUpcomingActivitiesAsync();
    Task<List<LeadActivity>> GetOverdueActivitiesAsync();
}

public class LeadStatistics
{
    public int TotalLeads { get; set; }
    public int NewLeads { get; set; }
    public int QualifiedLeads { get; set; }
    public int ConvertedLeads { get; set; }
    public int LostLeads { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal AverageLeadValue { get; set; }
    public decimal TotalPipelineValue { get; set; }
    public int OverdueFollowUps { get; set; }
    public Dictionary<LeadSource, int> LeadsBySource { get; set; } = new();
    public Dictionary<LeadStatus, int> LeadsByStatus { get; set; } = new();
}

public class LeadService : ILeadService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ICustomerService _customerService;
    private readonly IStaffService _staffService;
    private const string LEADS_KEY = "leads";

    public LeadService(ILocalStorageService localStorage, ICustomerService customerService, IStaffService staffService)
    {
        _localStorage = localStorage;
        _customerService = customerService;
        _staffService = staffService;
    }

    public async Task<List<Lead>> GetAllLeadsAsync()
    {
        try
        {
            var leads = await _localStorage.GetItemAsync<List<Lead>>(LEADS_KEY);
            if (leads == null || leads.Count == 0)
            {
                leads = await GetSampleLeadsAsync();
                await _localStorage.SetItemAsync(LEADS_KEY, leads);
            }
            
            // Load related data
            await LoadRelatedDataAsync(leads);
            return leads.OrderByDescending(l => l.CreatedAt).ToList();
        }
        catch
        {
            return await GetSampleLeadsAsync();
        }
    }

    public async Task<Lead?> GetLeadByIdAsync(Guid id)
    {
        var leads = await GetAllLeadsAsync();
        return leads.FirstOrDefault(l => l.Id == id);
    }

    public async Task<Lead> CreateLeadAsync(Lead lead)
    {
        lead.Id = Guid.NewGuid();
        lead.CreatedAt = DateTime.UtcNow;
        lead.UpdatedAt = DateTime.UtcNow;
        
        var leads = await GetAllLeadsAsync();
        leads.Add(lead);
        await _localStorage.SetItemAsync(LEADS_KEY, leads);
        
        return lead;
    }

    public async Task<Lead> UpdateLeadAsync(Lead lead)
    {
        lead.UpdatedAt = DateTime.UtcNow;
        
        var leads = await GetAllLeadsAsync();
        var index = leads.FindIndex(l => l.Id == lead.Id);
        if (index >= 0)
        {
            leads[index] = lead;
            await _localStorage.SetItemAsync(LEADS_KEY, leads);
        }
        
        return lead;
    }

    public async Task<bool> DeleteLeadAsync(Guid id)
    {
        var leads = await GetAllLeadsAsync();
        var lead = leads.FirstOrDefault(l => l.Id == id);
        if (lead != null)
        {
            leads.Remove(lead);
            await _localStorage.SetItemAsync(LEADS_KEY, leads);
            return true;
        }
        return false;
    }

    public async Task<List<Lead>> GetLeadsByStatusAsync(LeadStatus status)
    {
        var leads = await GetAllLeadsAsync();
        return leads.Where(l => l.Status == status).ToList();
    }

    public async Task<List<Lead>> GetLeadsBySourceAsync(LeadSource source)
    {
        var leads = await GetAllLeadsAsync();
        return leads.Where(l => l.Source == source).ToList();
    }

    public async Task<List<Lead>> GetLeadsByPriorityAsync(LeadPriority priority)
    {
        var leads = await GetAllLeadsAsync();
        return leads.Where(l => l.Priority == priority).ToList();
    }

    public async Task<List<Lead>> GetLeadsByAssignedStaffAsync(Guid staffId)
    {
        var leads = await GetAllLeadsAsync();
        return leads.Where(l => l.AssignedToStaffId == staffId).ToList();
    }

    public async Task<List<Lead>> SearchLeadsAsync(string searchTerm)
    {
        var leads = await GetAllLeadsAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return leads;

        searchTerm = searchTerm.ToLower();
        return leads.Where(l => 
            l.FirstName.ToLower().Contains(searchTerm) ||
            l.LastName.ToLower().Contains(searchTerm) ||
            l.Email.ToLower().Contains(searchTerm) ||
            l.Company.ToLower().Contains(searchTerm) ||
            l.Phone.Contains(searchTerm) ||
            l.ProjectDescription.ToLower().Contains(searchTerm)
        ).ToList();
    }

    public async Task<Lead> ConvertLeadToCustomerAsync(Guid leadId, Customer customer)
    {
        var lead = await GetLeadByIdAsync(leadId);
        if (lead == null)
            throw new InvalidOperationException("Lead not found");

        // Create the customer
        var createdCustomer = await _customerService.CreateCustomerAsync(customer);
        
        // Update the lead
        lead.Status = LeadStatus.Converted;
        lead.ConvertedCustomerId = createdCustomer.Id;
        lead.ConvertedCustomer = createdCustomer;
        lead.ConvertedDate = DateTime.UtcNow;
        
        await UpdateLeadAsync(lead);
        return lead;
    }

    public async Task<Lead> UpdateLeadStatusAsync(Guid leadId, LeadStatus status)
    {
        var lead = await GetLeadByIdAsync(leadId);
        if (lead != null)
        {
            lead.Status = status;
            await UpdateLeadAsync(lead);
        }
        return lead!;
    }

    public async Task<Lead> AssignLeadToStaffAsync(Guid leadId, Guid staffId)
    {
        var lead = await GetLeadByIdAsync(leadId);
        if (lead != null)
        {
            lead.AssignedToStaffId = staffId;
            await UpdateLeadAsync(lead);
        }
        return lead!;
    }

    public async Task<List<Lead>> GetOverdueLeadsAsync()
    {
        var leads = await GetAllLeadsAsync();
        return leads.Where(l => l.IsOverdue).ToList();
    }

    public async Task<List<Lead>> GetLeadsRequiringFollowUpAsync()
    {
        var leads = await GetAllLeadsAsync();
        var today = DateTime.UtcNow.Date;
        return leads.Where(l => 
            l.NextFollowUpDate.HasValue && 
            l.NextFollowUpDate.Value.Date <= today.AddDays(7) &&
            l.Status != LeadStatus.Converted && 
            l.Status != LeadStatus.Lost
        ).ToList();
    }

    public async Task<LeadStatistics> GetLeadStatisticsAsync()
    {
        var leads = await GetAllLeadsAsync();
        var totalLeads = leads.Count;
        var convertedLeads = leads.Count(l => l.Status == LeadStatus.Converted);
        
        return new LeadStatistics
        {
            TotalLeads = totalLeads,
            NewLeads = leads.Count(l => l.Status == LeadStatus.New),
            QualifiedLeads = leads.Count(l => l.Status == LeadStatus.Qualified),
            ConvertedLeads = convertedLeads,
            LostLeads = leads.Count(l => l.Status == LeadStatus.Lost),
            ConversionRate = totalLeads > 0 ? (decimal)convertedLeads / totalLeads * 100 : 0,
            AverageLeadValue = leads.Any() ? leads.Average(l => l.EstimatedValue) : 0,
            TotalPipelineValue = leads.Where(l => l.Status != LeadStatus.Converted && l.Status != LeadStatus.Lost)
                                     .Sum(l => l.EstimatedValue),
            OverdueFollowUps = leads.Count(l => l.IsOverdue),
            LeadsBySource = leads.GroupBy(l => l.Source).ToDictionary(g => g.Key, g => g.Count()),
            LeadsByStatus = leads.GroupBy(l => l.Status).ToDictionary(g => g.Key, g => g.Count())
        };
    }

    private async Task LoadRelatedDataAsync(List<Lead> leads)
    {
        var staff = await _staffService.GetAllStaffAsync();
        var customers = await _customerService.GetAllCustomersAsync();
        
        foreach (var lead in leads)
        {
            if (lead.AssignedToStaffId.HasValue)
            {
                lead.AssignedToStaff = staff.FirstOrDefault(s => s.Id == lead.AssignedToStaffId.Value);
            }
            
            if (lead.ConvertedCustomerId.HasValue)
            {
                lead.ConvertedCustomer = customers.FirstOrDefault(c => c.Id == lead.ConvertedCustomerId.Value);
            }
        }
    }

    private async Task<List<Lead>> GetSampleLeadsAsync()
    {
        var staff = await _staffService.GetAllStaffAsync();
        var sampleLeads = new List<Lead>();

        var staffMember = staff.FirstOrDefault();

        sampleLeads.AddRange(new List<Lead>
        {
            new Lead
            {
                FirstName = "Sarah",
                LastName = "Wilson",
                Email = "sarah.wilson@techstartup.com",
                Phone = "+1 (555) 234-5678",
                Company = "TechStartup Inc",
                JobTitle = "CTO",
                Status = LeadStatus.Qualified,
                Source = LeadSource.Website,
                Priority = LeadPriority.High,
                EstimatedValue = 45000,
                ExpectedCloseDate = DateTime.UtcNow.AddDays(15),
                ProjectDescription = "Custom e-commerce platform development",
                Requirements = "Multi-vendor marketplace with payment integration",
                Budget = "$40,000 - $50,000",
                Timeline = "3-4 months",
                Tags = new List<string> { "Hot Lead", "E-commerce", "High Value" },
                AssignedToStaffId = staffMember?.Id,
                AssignedToStaff = staffMember,
                LastContactDate = DateTime.UtcNow.AddDays(-2),
                NextFollowUpDate = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new Lead
            {
                FirstName = "Michael",
                LastName = "Chen",
                Email = "m.chen@consulting.com",
                Phone = "+1 (555) 345-6789",
                Company = "Chen Consulting",
                JobTitle = "Managing Partner",
                Status = LeadStatus.Proposal,
                Source = LeadSource.Referral,
                Priority = LeadPriority.Medium,
                EstimatedValue = 25000,
                ExpectedCloseDate = DateTime.UtcNow.AddDays(30),
                ProjectDescription = "Business process automation system",
                Requirements = "Workflow automation and document management",
                Budget = "$20,000 - $30,000",
                Timeline = "2-3 months",
                Tags = new List<string> { "Consulting", "Automation" },
                AssignedToStaffId = staffMember?.Id,
                AssignedToStaff = staffMember,
                LastContactDate = DateTime.UtcNow.AddDays(-5),
                NextFollowUpDate = DateTime.UtcNow.AddDays(3),
                CreatedAt = DateTime.UtcNow.AddDays(-14)
            },
            new Lead
            {
                FirstName = "Emily",
                LastName = "Rodriguez",
                Email = "emily@localrestaurant.com",
                Phone = "+1 (555) 456-7890",
                Company = "Local Restaurant Group",
                JobTitle = "Operations Manager",
                Status = LeadStatus.New,
                Source = LeadSource.SocialMedia,
                Priority = LeadPriority.Low,
                EstimatedValue = 8000,
                ExpectedCloseDate = DateTime.UtcNow.AddDays(45),
                ProjectDescription = "Restaurant website and online ordering",
                Requirements = "Simple website with online menu and ordering system",
                Budget = "$5,000 - $10,000",
                Timeline = "1-2 months",
                Tags = new List<string> { "Small Business", "Website" },
                LastContactDate = null,
                NextFollowUpDate = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Lead
            {
                FirstName = "David",
                LastName = "Thompson",
                Email = "david.thompson@enterprise.com",
                Phone = "+1 (555) 567-8901",
                Company = "Enterprise Solutions Ltd",
                JobTitle = "IT Director",
                Status = LeadStatus.Negotiation,
                Source = LeadSource.TradeShow,
                Priority = LeadPriority.Critical,
                EstimatedValue = 120000,
                ExpectedCloseDate = DateTime.UtcNow.AddDays(10),
                ProjectDescription = "Enterprise resource planning system",
                Requirements = "Custom ERP with integration to existing systems",
                Budget = "$100,000 - $150,000",
                Timeline = "6-8 months",
                Tags = new List<string> { "Enterprise", "ERP", "High Value", "Critical" },
                AssignedToStaffId = staffMember?.Id,
                AssignedToStaff = staffMember,
                LastContactDate = DateTime.UtcNow.AddDays(-1),
                NextFollowUpDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow.AddDays(-21)
            },
            new Lead
            {
                FirstName = "Lisa",
                LastName = "Anderson",
                Email = "lisa@nonprofit.org",
                Phone = "+1 (555) 678-9012",
                Company = "Community Nonprofit",
                JobTitle = "Executive Director",
                Status = LeadStatus.Lost,
                Source = LeadSource.EmailMarketing,
                Priority = LeadPriority.Low,
                EstimatedValue = 12000,
                ExpectedCloseDate = DateTime.UtcNow.AddDays(-10),
                ProjectDescription = "Donor management system",
                Requirements = "Simple CRM for donor tracking and communications",
                Budget = "$10,000 - $15,000",
                Timeline = "2-3 months",
                Tags = new List<string> { "Nonprofit", "CRM" },
                LastContactDate = DateTime.UtcNow.AddDays(-15),
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            }
        });

        return sampleLeads;
    }
}

public class LeadActivityService : ILeadActivityService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IStaffService _staffService;
    private const string ACTIVITIES_KEY = "lead_activities";

    public LeadActivityService(ILocalStorageService localStorage, IStaffService staffService)
    {
        _localStorage = localStorage;
        _staffService = staffService;
    }

    public async Task<List<LeadActivity>> GetActivitiesByLeadAsync(Guid leadId)
    {
        var activities = await GetAllActivitiesAsync();
        return activities.Where(a => a.LeadId == leadId).OrderByDescending(a => a.ActivityDate).ToList();
    }

    public async Task<LeadActivity> CreateActivityAsync(LeadActivity activity)
    {
        activity.Id = Guid.NewGuid();
        activity.CreatedAt = DateTime.UtcNow;
        
        var activities = await GetAllActivitiesAsync();
        activities.Add(activity);
        await _localStorage.SetItemAsync(ACTIVITIES_KEY, activities);
        
        return activity;
    }

    public async Task<LeadActivity> UpdateActivityAsync(LeadActivity activity)
    {
        var activities = await GetAllActivitiesAsync();
        var index = activities.FindIndex(a => a.Id == activity.Id);
        if (index >= 0)
        {
            activities[index] = activity;
            await _localStorage.SetItemAsync(ACTIVITIES_KEY, activities);
        }
        
        return activity;
    }

    public async Task<bool> DeleteActivityAsync(Guid activityId)
    {
        var activities = await GetAllActivitiesAsync();
        var activity = activities.FirstOrDefault(a => a.Id == activityId);
        if (activity != null)
        {
            activities.Remove(activity);
            await _localStorage.SetItemAsync(ACTIVITIES_KEY, activities);
            return true;
        }
        return false;
    }

    public async Task<LeadActivity> CompleteActivityAsync(Guid activityId, string outcome)
    {
        var activities = await GetAllActivitiesAsync();
        var activity = activities.FirstOrDefault(a => a.Id == activityId);
        if (activity != null)
        {
            activity.IsCompleted = true;
            activity.CompletedDate = DateTime.UtcNow;
            activity.Outcome = outcome;
            await _localStorage.SetItemAsync(ACTIVITIES_KEY, activities);
        }
        return activity!;
    }

    public async Task<List<LeadActivity>> GetUpcomingActivitiesAsync()
    {
        var activities = await GetAllActivitiesAsync();
        var today = DateTime.UtcNow.Date;
        return activities.Where(a => 
            a.ScheduledDate.HasValue && 
            a.ScheduledDate.Value.Date >= today && 
            !a.IsCompleted
        ).OrderBy(a => a.ScheduledDate).ToList();
    }

    public async Task<List<LeadActivity>> GetOverdueActivitiesAsync()
    {
        var activities = await GetAllActivitiesAsync();
        return activities.Where(a => a.IsOverdue).ToList();
    }

    private async Task<List<LeadActivity>> GetAllActivitiesAsync()
    {
        try
        {
            var activities = await _localStorage.GetItemAsync<List<LeadActivity>>(ACTIVITIES_KEY);
            if (activities == null)
            {
                activities = new List<LeadActivity>();
                await _localStorage.SetItemAsync(ACTIVITIES_KEY, activities);
            }
            
            // Load staff data
            var staff = await _staffService.GetAllStaffAsync();
            foreach (var activity in activities)
            {
                if (activity.AssignedToStaffId.HasValue)
                {
                    activity.AssignedToStaff = staff.FirstOrDefault(s => s.Id == activity.AssignedToStaffId.Value);
                }
            }
            
            return activities;
        }
        catch
        {
            return new List<LeadActivity>();
        }
    }
}
