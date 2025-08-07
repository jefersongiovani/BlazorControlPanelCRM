namespace BlazorControlPanel.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public Address Address { get; set; } = new();
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    public CustomerType Type { get; set; } = CustomerType.Individual;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public decimal TotalRevenue { get; set; }
    public int ProjectCount { get; set; }
    public DateTime? LastContactDate { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayName => !string.IsNullOrEmpty(Company) ? $"{Company} ({FullName})" : FullName;
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    
    public string FullAddress => $"{Street}, {City}, {State} {PostalCode}, {Country}".Trim(' ', ',');
}

public enum CustomerStatus
{
    Active,
    Inactive,
    Prospect,
    Lead,
    Archived
}

public enum CustomerType
{
    Individual,
    Business,
    Enterprise,
    Government,
    NonProfit
}
