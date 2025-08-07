namespace BlazorControlPanel.Models;

public class Staff
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public StaffStatus Status { get; set; } = StaffStatus.Active;
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public DateTime? TerminationDate { get; set; }
    public decimal Salary { get; set; }
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public Address Address { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public List<Guid> RoleIds { get; set; } = new();
    public List<string> Skills { get; set; } = new();
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public int VacationDaysUsed { get; set; }
    public int VacationDaysTotal { get; set; } = 20;
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayName => !string.IsNullOrEmpty(JobTitle) ? $"{FullName} - {JobTitle}" : FullName;
    public int VacationDaysRemaining => Math.Max(0, VacationDaysTotal - VacationDaysUsed);
    public bool IsActive => Status == StaffStatus.Active;
}

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Permission> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsSystemRole { get; set; } = false;
}

public class StaffRole
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsSystemRole { get; set; } = false;
    public int StaffCount { get; set; } = 0;
}

public class Permission
{
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    
    public string DisplayName => $"{Module} - {Action} {Resource}";
}

public enum StaffStatus
{
    Active,
    Inactive,
    OnLeave,
    Terminated,
    Suspended
}

public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Intern,
    Consultant
}

public static class SystemPermissions
{
    public static readonly List<Permission> AllPermissions = new()
    {
        // Customer Management
        new Permission { Module = "Customers", Action = "View", Resource = "List" },
        new Permission { Module = "Customers", Action = "Create", Resource = "Customer" },
        new Permission { Module = "Customers", Action = "Edit", Resource = "Customer" },
        new Permission { Module = "Customers", Action = "Delete", Resource = "Customer" },
        
        // Staff Management
        new Permission { Module = "Staff", Action = "View", Resource = "List" },
        new Permission { Module = "Staff", Action = "Create", Resource = "Staff" },
        new Permission { Module = "Staff", Action = "Edit", Resource = "Staff" },
        new Permission { Module = "Staff", Action = "Delete", Resource = "Staff" },
        new Permission { Module = "Staff", Action = "Manage", Resource = "Roles" },
        
        // Project Management
        new Permission { Module = "Projects", Action = "View", Resource = "List" },
        new Permission { Module = "Projects", Action = "Create", Resource = "Project" },
        new Permission { Module = "Projects", Action = "Edit", Resource = "Project" },
        new Permission { Module = "Projects", Action = "Delete", Resource = "Project" },
        new Permission { Module = "Projects", Action = "Manage", Resource = "Tasks" },
        
        // Financial Management
        new Permission { Module = "Finance", Action = "View", Resource = "Estimates" },
        new Permission { Module = "Finance", Action = "Create", Resource = "Estimate" },
        new Permission { Module = "Finance", Action = "Edit", Resource = "Estimate" },
        new Permission { Module = "Finance", Action = "Delete", Resource = "Estimate" },
        new Permission { Module = "Finance", Action = "View", Resource = "Invoices" },
        new Permission { Module = "Finance", Action = "Create", Resource = "Invoice" },
        new Permission { Module = "Finance", Action = "Edit", Resource = "Invoice" },
        new Permission { Module = "Finance", Action = "Delete", Resource = "Invoice" },
        new Permission { Module = "Finance", Action = "View", Resource = "Expenses" },
        new Permission { Module = "Finance", Action = "Create", Resource = "Expense" },
        new Permission { Module = "Finance", Action = "Edit", Resource = "Expense" },
        new Permission { Module = "Finance", Action = "Delete", Resource = "Expense" },
        
        // Lead Management
        new Permission { Module = "Leads", Action = "View", Resource = "List" },
        new Permission { Module = "Leads", Action = "Create", Resource = "Lead" },
        new Permission { Module = "Leads", Action = "Edit", Resource = "Lead" },
        new Permission { Module = "Leads", Action = "Delete", Resource = "Lead" },
        new Permission { Module = "Leads", Action = "Convert", Resource = "Lead" },
        
        // System Administration
        new Permission { Module = "System", Action = "View", Resource = "Settings" },
        new Permission { Module = "System", Action = "Edit", Resource = "Settings" },
        new Permission { Module = "System", Action = "View", Resource = "Logs" },
        new Permission { Module = "System", Action = "Backup", Resource = "Data" },
        new Permission { Module = "System", Action = "Restore", Resource = "Data" }
    };
    
    public static readonly List<Role> DefaultRoles = new()
    {
        new Role
        {
            Name = "Administrator",
            Description = "Full system access with all permissions",
            IsSystemRole = true,
            Permissions = AllPermissions.ToList()
        },
        new Role
        {
            Name = "Manager",
            Description = "Management level access to most features",
            IsSystemRole = true,
            Permissions = AllPermissions.Where(p => 
                p.Module != "System" || p.Action == "View").ToList()
        },
        new Role
        {
            Name = "Sales Representative",
            Description = "Access to customer and lead management",
            IsSystemRole = true,
            Permissions = AllPermissions.Where(p => 
                p.Module == "Customers" || 
                p.Module == "Leads" || 
                (p.Module == "Finance" && (p.Resource == "Estimates" || p.Resource == "Invoices"))).ToList()
        },
        new Role
        {
            Name = "Project Manager",
            Description = "Access to project and task management",
            IsSystemRole = true,
            Permissions = AllPermissions.Where(p => 
                p.Module == "Projects" || 
                p.Module == "Customers" && p.Action == "View").ToList()
        },
        new Role
        {
            Name = "Employee",
            Description = "Basic access to view information",
            IsSystemRole = true,
            Permissions = AllPermissions.Where(p => 
                p.Action == "View" && p.Module != "System" && p.Module != "Staff").ToList()
        }
    };
}
