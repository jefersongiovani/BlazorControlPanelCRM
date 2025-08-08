/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

namespace BlazorControlPanel.Models;

/// <summary>
/// Estimate entity representing project quotes and cost estimates provided to customers.
/// Contains detailed line items, pricing, and status tracking for the sales process.
/// </summary>
/// <remarks>
/// Essential for the sales process, allowing creation of detailed project estimates
/// that can be converted to invoices upon customer acceptance. Supports comprehensive
/// pricing structures with line items, taxes, and terms.
/// </remarks>
public class Estimate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EstimateNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EstimateStatus Status { get; set; } = EstimateStatus.Draft;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? SentDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public DateTime? RejectedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public List<EstimateItem> Items { get; set; } = new();
    public decimal SubTotal => Items.Sum(i => i.Total);
    public decimal TaxRate { get; set; } = 0.10m; // 10% default
    public decimal TaxAmount => SubTotal * TaxRate;
    public decimal Total => SubTotal + TaxAmount;
    public string Notes { get; set; } = string.Empty;
    public string Terms { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    
    public string DisplayNumber => !string.IsNullOrEmpty(EstimateNumber) ? EstimateNumber : $"EST-{Id.ToString()[..8]}";
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    public bool CanBeConverted => Status == EstimateStatus.Accepted && !IsExpired;
}

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public Guid? EstimateId { get; set; }
    public Estimate? Estimate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? SentDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public List<InvoiceItem> Items { get; set; } = new();
    public decimal SubTotal => Items.Sum(i => i.Total);
    public decimal TaxRate { get; set; } = 0.10m; // 10% default
    public decimal TaxAmount => SubTotal * TaxRate;
    public decimal Total => SubTotal + TaxAmount;
    public decimal AmountPaid { get; set; }
    public decimal AmountDue => Total - AmountPaid;
    public string Notes { get; set; } = string.Empty;
    public string Terms { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    
    public string DisplayNumber => !string.IsNullOrEmpty(InvoiceNumber) ? InvoiceNumber : $"INV-{Id.ToString()[..8]}";
    public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != InvoiceStatus.Paid;
    public bool IsPartiallyPaid => AmountPaid > 0 && AmountPaid < Total;
    public int DaysOverdue => IsOverdue ? (DateTime.UtcNow - DueDate!.Value).Days : 0;
}

public class EstimateItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;
    public string Unit { get; set; } = "each";
    public int SortOrder { get; set; }
}

public class InvoiceItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;
    public string Unit { get; set; } = "each";
    public int SortOrder { get; set; }
}

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentMethod Method { get; set; } = PaymentMethod.BankTransfer;
    public string Reference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
}

public enum EstimateStatus
{
    Draft,
    Sent,
    Accepted,
    Rejected,
    Expired,
    Converted
}

public enum InvoiceStatus
{
    Draft,
    Sent,
    Paid,
    Overdue,
    Cancelled,
    PartiallyPaid
}

public enum PaymentMethod
{
    Cash,
    Check,
    BankTransfer,
    CreditCard,
    DebitCard,
    PayPal,
    Stripe,
    Other
}

public static class FinancialDefaults
{
    public static readonly string DefaultTerms = @"Payment is due within 30 days of invoice date.
Late payments may be subject to a 1.5% monthly service charge.
Please include invoice number with payment.";

    public static readonly string DefaultEstimateTerms = @"This estimate is valid for 30 days from the date issued.
Prices are subject to change without notice.
A 50% deposit may be required to begin work.";

    public static readonly List<string> CommonDescriptions = new()
    {
        "Web Development Services",
        "Mobile App Development",
        "UI/UX Design",
        "Database Design & Implementation",
        "System Integration",
        "Technical Consulting",
        "Project Management",
        "Quality Assurance Testing",
        "Maintenance & Support",
        "Training Services"
    };

    public static readonly List<string> CommonUnits = new()
    {
        "each",
        "hour",
        "day",
        "week",
        "month",
        "project",
        "page",
        "feature",
        "license",
        "user"
    };
}
