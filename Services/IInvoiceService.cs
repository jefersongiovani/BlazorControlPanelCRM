using BlazorControlPanel.Models;
using Blazored.LocalStorage;

namespace BlazorControlPanel.Services;

public interface IInvoiceService
{
    Task<List<Invoice>> GetAllInvoicesAsync();
    Task<Invoice?> GetInvoiceByIdAsync(Guid id);
    Task<Invoice> CreateInvoiceAsync(Invoice invoice);
    Task<Invoice> UpdateInvoiceAsync(Invoice invoice);
    Task<bool> DeleteInvoiceAsync(Guid id);
    Task<List<Invoice>> GetInvoicesByCustomerAsync(Guid customerId);
    Task<List<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status);
    Task<string> GenerateInvoiceNumberAsync();
    Task<Invoice> CreateFromEstimateAsync(Guid estimateId);
    Task<Invoice> SendInvoiceAsync(Guid invoiceId);
    Task<Invoice> MarkAsPaidAsync(Guid invoiceId, decimal amount, PaymentMethod method, string reference);
    Task<List<Invoice>> GetOverdueInvoicesAsync();
    Task<decimal> GetTotalOutstandingAsync();
}

public interface IPaymentService
{
    Task<List<Payment>> GetPaymentsByInvoiceAsync(Guid invoiceId);
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task<List<Payment>> GetAllPaymentsAsync();
    Task<decimal> GetTotalPaymentsAsync(DateTime? fromDate = null, DateTime? toDate = null);
}

public class InvoiceService : IInvoiceService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ICustomerService _customerService;
    private readonly IEstimateService _estimateService;
    private const string INVOICES_KEY = "invoices";

    public InvoiceService(ILocalStorageService localStorage, ICustomerService customerService, IEstimateService estimateService)
    {
        _localStorage = localStorage;
        _customerService = customerService;
        _estimateService = estimateService;
    }

    public async Task<List<Invoice>> GetAllInvoicesAsync()
    {
        try
        {
            var invoices = await _localStorage.GetItemAsync<List<Invoice>>(INVOICES_KEY);
            if (invoices == null || invoices.Count == 0)
            {
                invoices = await GetSampleInvoicesAsync();
                await _localStorage.SetItemAsync(INVOICES_KEY, invoices);
            }
            
            // Load customer data
            await LoadCustomerDataAsync(invoices);
            return invoices.OrderByDescending(i => i.CreatedDate).ToList();
        }
        catch
        {
            return await GetSampleInvoicesAsync();
        }
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(Guid id)
    {
        var invoices = await GetAllInvoicesAsync();
        return invoices.FirstOrDefault(i => i.Id == id);
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
    {
        invoice.Id = Guid.NewGuid();
        invoice.CreatedAt = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;
        invoice.CreatedDate = DateTime.UtcNow;
        
        if (string.IsNullOrEmpty(invoice.InvoiceNumber))
        {
            invoice.InvoiceNumber = await GenerateInvoiceNumberAsync();
        }
        
        var invoices = await GetAllInvoicesAsync();
        invoices.Add(invoice);
        await _localStorage.SetItemAsync(INVOICES_KEY, invoices);
        
        return invoice;
    }

    public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
    {
        invoice.UpdatedAt = DateTime.UtcNow;
        
        var invoices = await GetAllInvoicesAsync();
        var index = invoices.FindIndex(i => i.Id == invoice.Id);
        if (index >= 0)
        {
            invoices[index] = invoice;
            await _localStorage.SetItemAsync(INVOICES_KEY, invoices);
        }
        
        return invoice;
    }

    public async Task<bool> DeleteInvoiceAsync(Guid id)
    {
        var invoices = await GetAllInvoicesAsync();
        var invoice = invoices.FirstOrDefault(i => i.Id == id);
        if (invoice != null)
        {
            invoices.Remove(invoice);
            await _localStorage.SetItemAsync(INVOICES_KEY, invoices);
            return true;
        }
        return false;
    }

    public async Task<List<Invoice>> GetInvoicesByCustomerAsync(Guid customerId)
    {
        var invoices = await GetAllInvoicesAsync();
        return invoices.Where(i => i.CustomerId == customerId).ToList();
    }

    public async Task<List<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status)
    {
        var invoices = await GetAllInvoicesAsync();
        return invoices.Where(i => i.Status == status).ToList();
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var invoices = await GetAllInvoicesAsync();
        var year = DateTime.UtcNow.Year;
        var count = invoices.Count(i => i.CreatedDate.Year == year) + 1;
        return $"INV-{year}-{count:D4}";
    }

    public async Task<Invoice> CreateFromEstimateAsync(Guid estimateId)
    {
        var estimate = await _estimateService.GetEstimateByIdAsync(estimateId);
        if (estimate == null || !estimate.CanBeConverted)
            throw new InvalidOperationException("Estimate cannot be converted to invoice");

        var invoice = new Invoice
        {
            CustomerId = estimate.CustomerId,
            Customer = estimate.Customer,
            EstimateId = estimate.Id,
            Estimate = estimate,
            Title = estimate.Title,
            Description = estimate.Description,
            TaxRate = estimate.TaxRate,
            Notes = estimate.Notes,
            Terms = FinancialDefaults.DefaultTerms,
            DueDate = DateTime.UtcNow.AddDays(30),
            Items = estimate.Items.Select(ei => new InvoiceItem
            {
                Description = ei.Description,
                Quantity = ei.Quantity,
                UnitPrice = ei.UnitPrice,
                Unit = ei.Unit,
                SortOrder = ei.SortOrder
            }).ToList()
        };

        await CreateInvoiceAsync(invoice);
        await _estimateService.ConvertToInvoiceAsync(estimateId);
        
        return invoice;
    }

    public async Task<Invoice> SendInvoiceAsync(Guid invoiceId)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        if (invoice != null)
        {
            invoice.Status = InvoiceStatus.Sent;
            invoice.SentDate = DateTime.UtcNow;
            if (!invoice.DueDate.HasValue)
            {
                invoice.DueDate = DateTime.UtcNow.AddDays(30);
            }
            await UpdateInvoiceAsync(invoice);
        }
        return invoice!;
    }

    public async Task<Invoice> MarkAsPaidAsync(Guid invoiceId, decimal amount, PaymentMethod method, string reference)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        if (invoice != null)
        {
            invoice.AmountPaid += amount;
            invoice.PaidDate = DateTime.UtcNow;
            
            if (invoice.AmountPaid >= invoice.Total)
            {
                invoice.Status = InvoiceStatus.Paid;
            }
            else if (invoice.AmountPaid > 0)
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
            }
            
            await UpdateInvoiceAsync(invoice);
        }
        return invoice!;
    }

    public async Task<List<Invoice>> GetOverdueInvoicesAsync()
    {
        var invoices = await GetAllInvoicesAsync();
        return invoices.Where(i => i.IsOverdue).ToList();
    }

    public async Task<decimal> GetTotalOutstandingAsync()
    {
        var invoices = await GetAllInvoicesAsync();
        return invoices.Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled)
                      .Sum(i => i.AmountDue);
    }

    private async Task LoadCustomerDataAsync(List<Invoice> invoices)
    {
        var customers = await _customerService.GetAllCustomersAsync();
        foreach (var invoice in invoices)
        {
            invoice.Customer = customers.FirstOrDefault(c => c.Id == invoice.CustomerId);
        }
    }

    private async Task<List<Invoice>> GetSampleInvoicesAsync()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        var sampleInvoices = new List<Invoice>();

        if (customers.Any())
        {
            var customer1 = customers.First();
            var customer2 = customers.Skip(1).FirstOrDefault() ?? customer1;

            sampleInvoices.AddRange(new List<Invoice>
            {
                new Invoice
                {
                    InvoiceNumber = "INV-2024-0001",
                    CustomerId = customer1.Id,
                    Customer = customer1,
                    Title = "Website Maintenance - Q4 2024",
                    Description = "Quarterly website maintenance and updates",
                    Status = InvoiceStatus.Paid,
                    CreatedDate = DateTime.UtcNow.AddDays(-45),
                    SentDate = DateTime.UtcNow.AddDays(-43),
                    DueDate = DateTime.UtcNow.AddDays(-13),
                    PaidDate = DateTime.UtcNow.AddDays(-10),
                    TaxRate = 0.08m,
                    AmountPaid = 2700m,
                    Terms = FinancialDefaults.DefaultTerms,
                    Items = new List<InvoiceItem>
                    {
                        new InvoiceItem { Description = "Content Updates", Quantity = 8, UnitPrice = 100, Unit = "hour", SortOrder = 1 },
                        new InvoiceItem { Description = "Security Updates", Quantity = 4, UnitPrice = 120, Unit = "hour", SortOrder = 2 },
                        new InvoiceItem { Description = "Performance Optimization", Quantity = 6, UnitPrice = 130, Unit = "hour", SortOrder = 3 },
                        new InvoiceItem { Description = "Backup & Monitoring", Quantity = 1, UnitPrice = 200, Unit = "month", SortOrder = 4 }
                    }
                },
                new Invoice
                {
                    InvoiceNumber = "INV-2024-0002",
                    CustomerId = customer2.Id,
                    Customer = customer2,
                    Title = "E-commerce Integration",
                    Description = "Payment gateway and inventory system integration",
                    Status = InvoiceStatus.Sent,
                    CreatedDate = DateTime.UtcNow.AddDays(-20),
                    SentDate = DateTime.UtcNow.AddDays(-18),
                    DueDate = DateTime.UtcNow.AddDays(12),
                    TaxRate = 0.10m,
                    Terms = FinancialDefaults.DefaultTerms,
                    Items = new List<InvoiceItem>
                    {
                        new InvoiceItem { Description = "Payment Gateway Setup", Quantity = 12, UnitPrice = 150, Unit = "hour", SortOrder = 1 },
                        new InvoiceItem { Description = "Inventory System Integration", Quantity = 20, UnitPrice = 140, Unit = "hour", SortOrder = 2 },
                        new InvoiceItem { Description = "Testing & Documentation", Quantity = 8, UnitPrice = 100, Unit = "hour", SortOrder = 3 }
                    }
                },
                new Invoice
                {
                    InvoiceNumber = "INV-2024-0003",
                    CustomerId = customer1.Id,
                    Customer = customer1,
                    Title = "Emergency Bug Fixes",
                    Description = "Critical bug fixes for production system",
                    Status = InvoiceStatus.Overdue,
                    CreatedDate = DateTime.UtcNow.AddDays(-50),
                    SentDate = DateTime.UtcNow.AddDays(-48),
                    DueDate = DateTime.UtcNow.AddDays(-18),
                    TaxRate = 0.08m,
                    Terms = FinancialDefaults.DefaultTerms,
                    Items = new List<InvoiceItem>
                    {
                        new InvoiceItem { Description = "Emergency Response", Quantity = 6, UnitPrice = 200, Unit = "hour", SortOrder = 1 },
                        new InvoiceItem { Description = "Bug Investigation", Quantity = 4, UnitPrice = 150, Unit = "hour", SortOrder = 2 },
                        new InvoiceItem { Description = "Code Fixes", Quantity = 8, UnitPrice = 140, Unit = "hour", SortOrder = 3 },
                        new InvoiceItem { Description = "Testing & Deployment", Quantity = 3, UnitPrice = 120, Unit = "hour", SortOrder = 4 }
                    }
                }
            });
        }

        return sampleInvoices;
    }
}

public class PaymentService : IPaymentService
{
    private readonly ILocalStorageService _localStorage;
    private const string PAYMENTS_KEY = "payments";

    public PaymentService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<List<Payment>> GetPaymentsByInvoiceAsync(Guid invoiceId)
    {
        var payments = await GetAllPaymentsAsync();
        return payments.Where(p => p.InvoiceId == invoiceId).ToList();
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        payment.Id = Guid.NewGuid();
        payment.CreatedAt = DateTime.UtcNow;
        
        var payments = await GetAllPaymentsAsync();
        payments.Add(payment);
        await _localStorage.SetItemAsync(PAYMENTS_KEY, payments);
        
        return payment;
    }

    public async Task<List<Payment>> GetAllPaymentsAsync()
    {
        try
        {
            var payments = await _localStorage.GetItemAsync<List<Payment>>(PAYMENTS_KEY);
            return payments ?? new List<Payment>();
        }
        catch
        {
            return new List<Payment>();
        }
    }

    public async Task<decimal> GetTotalPaymentsAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var payments = await GetAllPaymentsAsync();
        
        if (fromDate.HasValue)
            payments = payments.Where(p => p.PaymentDate >= fromDate.Value).ToList();
            
        if (toDate.HasValue)
            payments = payments.Where(p => p.PaymentDate <= toDate.Value).ToList();
            
        return payments.Sum(p => p.Amount);
    }
}
