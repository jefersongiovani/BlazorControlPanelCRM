/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

using BlazorControlPanel.Models;
using Blazored.LocalStorage;

namespace BlazorControlPanel.Services;

/// <summary>
/// Interface defining estimate management operations and quote generation functionality.
/// Provides contract for estimate CRUD operations, approval workflow, and invoice conversion.
/// </summary>
/// <remarks>
/// Defines the service layer contract for comprehensive estimate management including
/// quote generation, customer approval workflow, and conversion to invoices.
/// </remarks>
public interface IEstimateService
{
    Task<List<Estimate>> GetAllEstimatesAsync();
    Task<Estimate?> GetEstimateByIdAsync(Guid id);
    Task<Estimate> CreateEstimateAsync(Estimate estimate);
    Task<Estimate> UpdateEstimateAsync(Estimate estimate);
    Task<bool> DeleteEstimateAsync(Guid id);
    Task<List<Estimate>> GetEstimatesByCustomerAsync(Guid customerId);
    Task<List<Estimate>> GetEstimatesByStatusAsync(EstimateStatus status);
    Task<string> GenerateEstimateNumberAsync();
    Task<Estimate> ConvertToInvoiceAsync(Guid estimateId);
    Task<Estimate> SendEstimateAsync(Guid estimateId);
    Task<Estimate> AcceptEstimateAsync(Guid estimateId);
    Task<Estimate> RejectEstimateAsync(Guid estimateId);
}

public class EstimateService : IEstimateService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ICustomerService _customerService;
    private const string ESTIMATES_KEY = "estimates";

    public EstimateService(ILocalStorageService localStorage, ICustomerService customerService)
    {
        _localStorage = localStorage;
        _customerService = customerService;
    }

    public async Task<List<Estimate>> GetAllEstimatesAsync()
    {
        try
        {
            var estimates = await _localStorage.GetItemAsync<List<Estimate>>(ESTIMATES_KEY);
            if (estimates == null || estimates.Count == 0)
            {
                estimates = await GetSampleEstimatesAsync();
                await _localStorage.SetItemAsync(ESTIMATES_KEY, estimates);
            }
            
            // Load customer data
            await LoadCustomerDataAsync(estimates);
            return estimates.OrderByDescending(e => e.CreatedDate).ToList();
        }
        catch
        {
            return await GetSampleEstimatesAsync();
        }
    }

    public async Task<Estimate?> GetEstimateByIdAsync(Guid id)
    {
        var estimates = await GetAllEstimatesAsync();
        return estimates.FirstOrDefault(e => e.Id == id);
    }

    public async Task<Estimate> CreateEstimateAsync(Estimate estimate)
    {
        estimate.Id = Guid.NewGuid();
        estimate.CreatedAt = DateTime.UtcNow;
        estimate.UpdatedAt = DateTime.UtcNow;
        estimate.CreatedDate = DateTime.UtcNow;
        
        if (string.IsNullOrEmpty(estimate.EstimateNumber))
        {
            estimate.EstimateNumber = await GenerateEstimateNumberAsync();
        }
        
        var estimates = await GetAllEstimatesAsync();
        estimates.Add(estimate);
        await _localStorage.SetItemAsync(ESTIMATES_KEY, estimates);
        
        return estimate;
    }

    public async Task<Estimate> UpdateEstimateAsync(Estimate estimate)
    {
        estimate.UpdatedAt = DateTime.UtcNow;
        
        var estimates = await GetAllEstimatesAsync();
        var index = estimates.FindIndex(e => e.Id == estimate.Id);
        if (index >= 0)
        {
            estimates[index] = estimate;
            await _localStorage.SetItemAsync(ESTIMATES_KEY, estimates);
        }
        
        return estimate;
    }

    public async Task<bool> DeleteEstimateAsync(Guid id)
    {
        var estimates = await GetAllEstimatesAsync();
        var estimate = estimates.FirstOrDefault(e => e.Id == id);
        if (estimate != null)
        {
            estimates.Remove(estimate);
            await _localStorage.SetItemAsync(ESTIMATES_KEY, estimates);
            return true;
        }
        return false;
    }

    public async Task<List<Estimate>> GetEstimatesByCustomerAsync(Guid customerId)
    {
        var estimates = await GetAllEstimatesAsync();
        return estimates.Where(e => e.CustomerId == customerId).ToList();
    }

    public async Task<List<Estimate>> GetEstimatesByStatusAsync(EstimateStatus status)
    {
        var estimates = await GetAllEstimatesAsync();
        return estimates.Where(e => e.Status == status).ToList();
    }

    public async Task<string> GenerateEstimateNumberAsync()
    {
        var estimates = await GetAllEstimatesAsync();
        var year = DateTime.UtcNow.Year;
        var count = estimates.Count(e => e.CreatedDate.Year == year) + 1;
        return $"EST-{year}-{count:D4}";
    }

    public async Task<Estimate> ConvertToInvoiceAsync(Guid estimateId)
    {
        var estimate = await GetEstimateByIdAsync(estimateId);
        if (estimate != null && estimate.CanBeConverted)
        {
            estimate.Status = EstimateStatus.Converted;
            await UpdateEstimateAsync(estimate);
        }
        return estimate!;
    }

    public async Task<Estimate> SendEstimateAsync(Guid estimateId)
    {
        var estimate = await GetEstimateByIdAsync(estimateId);
        if (estimate != null)
        {
            estimate.Status = EstimateStatus.Sent;
            estimate.SentDate = DateTime.UtcNow;
            estimate.ExpiryDate = DateTime.UtcNow.AddDays(30); // 30 days validity
            await UpdateEstimateAsync(estimate);
        }
        return estimate!;
    }

    public async Task<Estimate> AcceptEstimateAsync(Guid estimateId)
    {
        var estimate = await GetEstimateByIdAsync(estimateId);
        if (estimate != null)
        {
            estimate.Status = EstimateStatus.Accepted;
            estimate.AcceptedDate = DateTime.UtcNow;
            await UpdateEstimateAsync(estimate);
        }
        return estimate!;
    }

    public async Task<Estimate> RejectEstimateAsync(Guid estimateId)
    {
        var estimate = await GetEstimateByIdAsync(estimateId);
        if (estimate != null)
        {
            estimate.Status = EstimateStatus.Rejected;
            estimate.RejectedDate = DateTime.UtcNow;
            await UpdateEstimateAsync(estimate);
        }
        return estimate!;
    }

    private async Task LoadCustomerDataAsync(List<Estimate> estimates)
    {
        var customers = await _customerService.GetAllCustomersAsync();
        foreach (var estimate in estimates)
        {
            estimate.Customer = customers.FirstOrDefault(c => c.Id == estimate.CustomerId);
        }
    }

    private async Task<List<Estimate>> GetSampleEstimatesAsync()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        var sampleEstimates = new List<Estimate>();

        if (customers.Any())
        {
            var customer1 = customers.First();
            var customer2 = customers.Skip(1).FirstOrDefault() ?? customer1;

            sampleEstimates.AddRange(new List<Estimate>
            {
                new Estimate
                {
                    EstimateNumber = "EST-2024-0001",
                    CustomerId = customer1.Id,
                    Customer = customer1,
                    Title = "Website Redesign Project",
                    Description = "Complete redesign of company website with modern UI/UX",
                    Status = EstimateStatus.Sent,
                    CreatedDate = DateTime.UtcNow.AddDays(-10),
                    SentDate = DateTime.UtcNow.AddDays(-8),
                    ExpiryDate = DateTime.UtcNow.AddDays(20),
                    TaxRate = 0.08m,
                    Terms = FinancialDefaults.DefaultEstimateTerms,
                    Items = new List<EstimateItem>
                    {
                        new EstimateItem { Description = "UI/UX Design", Quantity = 40, UnitPrice = 125, Unit = "hour", SortOrder = 1 },
                        new EstimateItem { Description = "Frontend Development", Quantity = 60, UnitPrice = 100, Unit = "hour", SortOrder = 2 },
                        new EstimateItem { Description = "Backend Integration", Quantity = 20, UnitPrice = 120, Unit = "hour", SortOrder = 3 },
                        new EstimateItem { Description = "Testing & QA", Quantity = 16, UnitPrice = 80, Unit = "hour", SortOrder = 4 }
                    }
                },
                new Estimate
                {
                    EstimateNumber = "EST-2024-0002",
                    CustomerId = customer2.Id,
                    Customer = customer2,
                    Title = "Mobile App Development",
                    Description = "Native iOS and Android mobile application",
                    Status = EstimateStatus.Draft,
                    CreatedDate = DateTime.UtcNow.AddDays(-5),
                    TaxRate = 0.10m,
                    Terms = FinancialDefaults.DefaultEstimateTerms,
                    Items = new List<EstimateItem>
                    {
                        new EstimateItem { Description = "App Design", Quantity = 30, UnitPrice = 130, Unit = "hour", SortOrder = 1 },
                        new EstimateItem { Description = "iOS Development", Quantity = 80, UnitPrice = 140, Unit = "hour", SortOrder = 2 },
                        new EstimateItem { Description = "Android Development", Quantity = 80, UnitPrice = 140, Unit = "hour", SortOrder = 3 },
                        new EstimateItem { Description = "API Integration", Quantity = 25, UnitPrice = 120, Unit = "hour", SortOrder = 4 },
                        new EstimateItem { Description = "App Store Deployment", Quantity = 1, UnitPrice = 500, Unit = "project", SortOrder = 5 }
                    }
                },
                new Estimate
                {
                    EstimateNumber = "EST-2024-0003",
                    CustomerId = customer1.Id,
                    Customer = customer1,
                    Title = "Database Migration",
                    Description = "Migration from legacy system to modern cloud database",
                    Status = EstimateStatus.Accepted,
                    CreatedDate = DateTime.UtcNow.AddDays(-15),
                    SentDate = DateTime.UtcNow.AddDays(-12),
                    AcceptedDate = DateTime.UtcNow.AddDays(-3),
                    TaxRate = 0.08m,
                    Terms = FinancialDefaults.DefaultEstimateTerms,
                    Items = new List<EstimateItem>
                    {
                        new EstimateItem { Description = "Data Analysis & Planning", Quantity = 20, UnitPrice = 150, Unit = "hour", SortOrder = 1 },
                        new EstimateItem { Description = "Database Setup", Quantity = 15, UnitPrice = 140, Unit = "hour", SortOrder = 2 },
                        new EstimateItem { Description = "Data Migration", Quantity = 35, UnitPrice = 130, Unit = "hour", SortOrder = 3 },
                        new EstimateItem { Description = "Testing & Validation", Quantity = 20, UnitPrice = 120, Unit = "hour", SortOrder = 4 }
                    }
                }
            });
        }

        return sampleEstimates;
    }
}
