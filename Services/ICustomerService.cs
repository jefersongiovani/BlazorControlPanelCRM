/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

using BlazorControlPanel.Models;
using Blazored.LocalStorage;

namespace BlazorControlPanel.Services;

/// <summary>
/// Interface defining customer management operations and data access methods.
/// Provides contract for customer CRUD operations, search functionality, and filtering capabilities.
/// </summary>
/// <remarks>
/// Defines the service layer contract for customer management, enabling dependency injection
/// and testability while providing comprehensive customer data operations.
/// </remarks>
public interface ICustomerService
{
    Task<List<Customer>> GetAllCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(Guid id);
    Task<Customer> CreateCustomerAsync(Customer customer);
    Task<Customer> UpdateCustomerAsync(Customer customer);
    Task<bool> DeleteCustomerAsync(Guid id);
    Task<List<Customer>> SearchCustomersAsync(string searchTerm);
    Task<List<Customer>> GetCustomersByStatusAsync(CustomerStatus status);
    Task<List<Customer>> GetCustomersByTypeAsync(CustomerType type);
}

/// <summary>
/// Customer service implementation providing customer management operations using local storage.
/// Implements comprehensive customer CRUD operations, search functionality, and data persistence.
/// </summary>
/// <remarks>
/// Uses browser local storage for data persistence in this demo implementation.
/// Provides sample data initialization and maintains customer data across browser sessions.
/// In a production environment, this would be replaced with database operations.
/// </remarks>
public class CustomerService : ICustomerService
{
    private readonly ILocalStorageService _localStorage;
    private const string CUSTOMERS_KEY = "customers";

    public CustomerService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        try
        {
            var customers = await _localStorage.GetItemAsync<List<Customer>>(CUSTOMERS_KEY);
            if (customers == null || customers.Count == 0)
            {
                // Initialize with sample data
                customers = GetSampleCustomers();
                await _localStorage.SetItemAsync(CUSTOMERS_KEY, customers);
            }
            return customers;
        }
        catch
        {
            return GetSampleCustomers();
        }
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid id)
    {
        var customers = await GetAllCustomersAsync();
        return customers.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        customer.Id = Guid.NewGuid();
        customer.CreatedAt = DateTime.UtcNow;
        customer.UpdatedAt = DateTime.UtcNow;
        
        var customers = await GetAllCustomersAsync();
        customers.Add(customer);
        await _localStorage.SetItemAsync(CUSTOMERS_KEY, customers);
        
        return customer;
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        
        var customers = await GetAllCustomersAsync();
        var index = customers.FindIndex(c => c.Id == customer.Id);
        if (index >= 0)
        {
            customers[index] = customer;
            await _localStorage.SetItemAsync(CUSTOMERS_KEY, customers);
        }
        
        return customer;
    }

    public async Task<bool> DeleteCustomerAsync(Guid id)
    {
        var customers = await GetAllCustomersAsync();
        var customer = customers.FirstOrDefault(c => c.Id == id);
        if (customer != null)
        {
            customers.Remove(customer);
            await _localStorage.SetItemAsync(CUSTOMERS_KEY, customers);
            return true;
        }
        return false;
    }

    public async Task<List<Customer>> SearchCustomersAsync(string searchTerm)
    {
        var customers = await GetAllCustomersAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return customers;

        searchTerm = searchTerm.ToLower();
        return customers.Where(c => 
            c.FirstName.ToLower().Contains(searchTerm) ||
            c.LastName.ToLower().Contains(searchTerm) ||
            c.Email.ToLower().Contains(searchTerm) ||
            c.Company.ToLower().Contains(searchTerm) ||
            c.Phone.Contains(searchTerm)
        ).ToList();
    }

    public async Task<List<Customer>> GetCustomersByStatusAsync(CustomerStatus status)
    {
        var customers = await GetAllCustomersAsync();
        return customers.Where(c => c.Status == status).ToList();
    }

    public async Task<List<Customer>> GetCustomersByTypeAsync(CustomerType type)
    {
        var customers = await GetAllCustomersAsync();
        return customers.Where(c => c.Type == type).ToList();
    }

    private List<Customer> GetSampleCustomers()
    {
        return new List<Customer>
        {
            new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1 (555) 123-4567",
                Company = "ABC Corporation",
                JobTitle = "CEO",
                Status = CustomerStatus.Active,
                Type = CustomerType.Business,
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                    Country = "USA"
                },
                TotalRevenue = 125000,
                ProjectCount = 3,
                LastContactDate = DateTime.Now.AddDays(-5),
                Tags = new List<string> { "VIP", "Enterprise" }
            },
            new Customer
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@techcorp.com",
                Phone = "+1 (555) 987-6543",
                Company = "TechCorp Solutions",
                JobTitle = "CTO",
                Status = CustomerStatus.Active,
                Type = CustomerType.Business,
                Address = new Address
                {
                    Street = "456 Tech Ave",
                    City = "San Francisco",
                    State = "CA",
                    PostalCode = "94105",
                    Country = "USA"
                },
                TotalRevenue = 89000,
                ProjectCount = 2,
                LastContactDate = DateTime.Now.AddDays(-12),
                Tags = new List<string> { "Technology", "Recurring" }
            },
            new Customer
            {
                FirstName = "Michael",
                LastName = "Johnson",
                Email = "m.johnson@email.com",
                Phone = "+1 (555) 456-7890",
                Company = "",
                JobTitle = "Freelancer",
                Status = CustomerStatus.Prospect,
                Type = CustomerType.Individual,
                Address = new Address
                {
                    Street = "789 Oak St",
                    City = "Chicago",
                    State = "IL",
                    PostalCode = "60601",
                    Country = "USA"
                },
                TotalRevenue = 15000,
                ProjectCount = 1,
                LastContactDate = DateTime.Now.AddDays(-3),
                Tags = new List<string> { "Individual", "Small Project" }
            }
        };
    }
}
