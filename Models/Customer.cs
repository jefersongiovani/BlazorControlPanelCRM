/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

namespace BlazorControlPanel.Models;

/// <summary>
/// Customer entity representing individual or business clients in the CRM system.
/// Contains comprehensive customer information including contact details, business data,
/// and relationship tracking for effective customer relationship management.
/// </summary>
/// <remarks>
/// Central entity for customer management, used across sales, project management,
/// and analytics modules. Supports both individual and business customer types
/// with flexible categorization and status tracking.
/// </remarks>
public class Customer
{
    /// <summary>
    /// Unique identifier for the customer record
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Customer's first name (for individuals) or primary contact first name (for businesses)
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's last name (for individuals) or primary contact last name (for businesses)
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Primary email address for customer communication and correspondence
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Primary phone number for customer contact and support
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Company or organization name (applicable for business customers)
    /// </summary>
    public string Company { get; set; } = string.Empty;

    /// <summary>
    /// Job title or position of the primary contact within the organization
    /// </summary>
    public string JobTitle { get; set; } = string.Empty;

    /// <summary>
    /// Physical address information for the customer or their business location
    /// </summary>
    public Address Address { get; set; } = new();

    /// <summary>
    /// Current status of the customer relationship (Active, Inactive, Prospect, etc.)
    /// </summary>
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;

    /// <summary>
    /// Type classification of the customer (Individual, Business, Enterprise, etc.)
    /// </summary>
    public CustomerType Type { get; set; } = CustomerType.Individual;

    /// <summary>
    /// Free-form notes and additional information about the customer
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the customer record was initially created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the customer record was last modified
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifier of the user who created this customer record
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the user who last updated this customer record
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Collection of tags for categorizing and organizing customers
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Total revenue generated from this customer across all projects and transactions
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Number of projects associated with this customer
    /// </summary>
    public int ProjectCount { get; set; }

    /// <summary>
    /// Date of the most recent contact or interaction with this customer
    /// </summary>
    public DateTime? LastContactDate { get; set; }

    /// <summary>
    /// Computed property returning the customer's full name by combining first and last names
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Computed property returning a display-friendly name, prioritizing company name for business customers
    /// </summary>
    public string DisplayName => !string.IsNullOrEmpty(Company) ? $"{Company} ({FullName})" : FullName;
}

/// <summary>
/// Address entity representing physical location information for customers and businesses.
/// Provides structured storage of address components with formatted display capabilities.
/// </summary>
/// <remarks>
/// Used for customer location tracking, service delivery planning, and geographic
/// analysis. Supports international address formats through flexible field structure.
/// </remarks>
public class Address
{
    /// <summary>
    /// Street address including house number, street name, and apartment/suite information
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// City or municipality name
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State, province, or region name
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Postal code, ZIP code, or equivalent postal identifier
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Country name or country code
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Computed property returning a formatted full address string for display purposes
    /// </summary>
    public string FullAddress => $"{Street}, {City}, {State} {PostalCode}, {Country}".Trim(' ', ',');
}

/// <summary>
/// Enumeration defining the possible status values for customer relationships.
/// Used to track the current state of customer engagement and lifecycle stage.
/// </summary>
/// <remarks>
/// Essential for customer lifecycle management, sales pipeline tracking, and
/// targeted marketing campaigns based on customer relationship status.
/// </remarks>
public enum CustomerStatus
{
    /// <summary>Currently engaged customer with active projects or ongoing relationship</summary>
    Active,

    /// <summary>Customer with no current activity but maintains relationship</summary>
    Inactive,

    /// <summary>Potential customer being evaluated or in early engagement phase</summary>
    Prospect,

    /// <summary>Qualified lead with identified interest and potential for conversion</summary>
    Lead,

    /// <summary>Former customer with completed relationship, kept for historical reference</summary>
    Archived
}

/// <summary>
/// Enumeration defining customer type classifications for business segmentation.
/// Used for tailored service delivery, pricing strategies, and market analysis.
/// </summary>
/// <remarks>
/// Enables targeted business strategies, appropriate service levels, and
/// market segmentation for analytics and reporting purposes.
/// </remarks>
public enum CustomerType
{
    /// <summary>Individual person or consumer customer</summary>
    Individual,

    /// <summary>Small to medium business customer</summary>
    Business,

    /// <summary>Large enterprise customer with complex requirements</summary>
    Enterprise,

    /// <summary>Government agency or public sector organization</summary>
    Government,

    /// <summary>Non-profit organization or charitable institution</summary>
    NonProfit
}
