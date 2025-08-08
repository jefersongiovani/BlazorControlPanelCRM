/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

using BlazorControlPanel.Models;
using Blazored.LocalStorage;

namespace BlazorControlPanel.Services;

/// <summary>
/// Interface defining staff management operations and data access methods.
/// Provides contract for staff CRUD operations, search functionality, and organizational structure management.
/// </summary>
/// <remarks>
/// Defines the service layer contract for staff management, enabling comprehensive
/// employee data operations, department management, and role-based access control.
/// </remarks>
public interface IStaffService
{
    Task<List<Staff>> GetAllStaffAsync();
    Task<Staff?> GetStaffByIdAsync(Guid id);
    Task<Staff> CreateStaffAsync(Staff staff);
    Task<Staff> UpdateStaffAsync(Staff staff);
    Task<bool> DeleteStaffAsync(Guid id);
    Task<List<Staff>> SearchStaffAsync(string searchTerm);
    Task<List<Staff>> GetStaffByStatusAsync(StaffStatus status);
    Task<List<Staff>> GetStaffByDepartmentAsync(string department);
    Task<List<string>> GetDepartmentsAsync();
}

public interface IRoleService
{
    Task<List<Role>> GetAllRolesAsync();
    Task<Role?> GetRoleByIdAsync(Guid id);
    Task<Role> CreateRoleAsync(Role role);
    Task<Role> UpdateRoleAsync(Role role);
    Task<bool> DeleteRoleAsync(Guid id);
    Task<List<Permission>> GetAllPermissionsAsync();
    Task InitializeDefaultRolesAsync();
}

public class StaffService : IStaffService
{
    private readonly ILocalStorageService _localStorage;
    private const string STAFF_KEY = "staff";

    public StaffService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<List<Staff>> GetAllStaffAsync()
    {
        try
        {
            var staff = await _localStorage.GetItemAsync<List<Staff>>(STAFF_KEY);
            if (staff == null || staff.Count == 0)
            {
                staff = GetSampleStaff();
                await _localStorage.SetItemAsync(STAFF_KEY, staff);
            }
            return staff;
        }
        catch
        {
            return GetSampleStaff();
        }
    }

    public async Task<Staff?> GetStaffByIdAsync(Guid id)
    {
        var staff = await GetAllStaffAsync();
        return staff.FirstOrDefault(s => s.Id == id);
    }

    public async Task<Staff> CreateStaffAsync(Staff staff)
    {
        staff.Id = Guid.NewGuid();
        staff.CreatedAt = DateTime.UtcNow;
        staff.UpdatedAt = DateTime.UtcNow;
        
        var staffList = await GetAllStaffAsync();
        staffList.Add(staff);
        await _localStorage.SetItemAsync(STAFF_KEY, staffList);
        
        return staff;
    }

    public async Task<Staff> UpdateStaffAsync(Staff staff)
    {
        staff.UpdatedAt = DateTime.UtcNow;
        
        var staffList = await GetAllStaffAsync();
        var index = staffList.FindIndex(s => s.Id == staff.Id);
        if (index >= 0)
        {
            staffList[index] = staff;
            await _localStorage.SetItemAsync(STAFF_KEY, staffList);
        }
        
        return staff;
    }

    public async Task<bool> DeleteStaffAsync(Guid id)
    {
        var staffList = await GetAllStaffAsync();
        var staff = staffList.FirstOrDefault(s => s.Id == id);
        if (staff != null)
        {
            staffList.Remove(staff);
            await _localStorage.SetItemAsync(STAFF_KEY, staffList);
            return true;
        }
        return false;
    }

    public async Task<List<Staff>> SearchStaffAsync(string searchTerm)
    {
        var staff = await GetAllStaffAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return staff;

        searchTerm = searchTerm.ToLower();
        return staff.Where(s => 
            s.FirstName.ToLower().Contains(searchTerm) ||
            s.LastName.ToLower().Contains(searchTerm) ||
            s.Email.ToLower().Contains(searchTerm) ||
            s.JobTitle.ToLower().Contains(searchTerm) ||
            s.Department.ToLower().Contains(searchTerm) ||
            s.Phone.Contains(searchTerm)
        ).ToList();
    }

    public async Task<List<Staff>> GetStaffByStatusAsync(StaffStatus status)
    {
        var staff = await GetAllStaffAsync();
        return staff.Where(s => s.Status == status).ToList();
    }

    public async Task<List<Staff>> GetStaffByDepartmentAsync(string department)
    {
        var staff = await GetAllStaffAsync();
        return staff.Where(s => s.Department.Equals(department, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<string>> GetDepartmentsAsync()
    {
        var staff = await GetAllStaffAsync();
        return staff.Select(s => s.Department)
                  .Where(d => !string.IsNullOrEmpty(d))
                  .Distinct()
                  .OrderBy(d => d)
                  .ToList();
    }

    private List<Staff> GetSampleStaff()
    {
        return new List<Staff>
        {
            new Staff
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@company.com",
                Phone = "+1 (555) 123-4567",
                JobTitle = "Senior Developer",
                Department = "Engineering",
                Status = StaffStatus.Active,
                HireDate = DateTime.Now.AddYears(-2),
                Salary = 95000,
                EmploymentType = EmploymentType.FullTime,
                Address = new Address
                {
                    Street = "123 Tech Street",
                    City = "San Francisco",
                    State = "CA",
                    PostalCode = "94105",
                    Country = "USA"
                },
                Skills = new List<string> { "C#", "Blazor", "JavaScript", "SQL" },
                VacationDaysUsed = 8,
                VacationDaysTotal = 25
            },
            new Staff
            {
                FirstName = "Bob",
                LastName = "Smith",
                Email = "bob.smith@company.com",
                Phone = "+1 (555) 987-6543",
                JobTitle = "Sales Manager",
                Department = "Sales",
                Status = StaffStatus.Active,
                HireDate = DateTime.Now.AddYears(-3),
                Salary = 85000,
                EmploymentType = EmploymentType.FullTime,
                Address = new Address
                {
                    Street = "456 Business Ave",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                    Country = "USA"
                },
                Skills = new List<string> { "Sales", "CRM", "Negotiation", "Lead Generation" },
                VacationDaysUsed = 12,
                VacationDaysTotal = 20
            },
            new Staff
            {
                FirstName = "Carol",
                LastName = "Davis",
                Email = "carol.davis@company.com",
                Phone = "+1 (555) 456-7890",
                JobTitle = "Project Manager",
                Department = "Operations",
                Status = StaffStatus.Active,
                HireDate = DateTime.Now.AddYears(-1),
                Salary = 78000,
                EmploymentType = EmploymentType.FullTime,
                Address = new Address
                {
                    Street = "789 Management Blvd",
                    City = "Chicago",
                    State = "IL",
                    PostalCode = "60601",
                    Country = "USA"
                },
                Skills = new List<string> { "Project Management", "Agile", "Scrum", "Team Leadership" },
                VacationDaysUsed = 5,
                VacationDaysTotal = 20
            },
            new Staff
            {
                FirstName = "David",
                LastName = "Wilson",
                Email = "david.wilson@company.com",
                Phone = "+1 (555) 321-0987",
                JobTitle = "UX Designer",
                Department = "Design",
                Status = StaffStatus.OnLeave,
                HireDate = DateTime.Now.AddMonths(-8),
                Salary = 72000,
                EmploymentType = EmploymentType.FullTime,
                Address = new Address
                {
                    Street = "321 Creative Lane",
                    City = "Austin",
                    State = "TX",
                    PostalCode = "73301",
                    Country = "USA"
                },
                Skills = new List<string> { "UI/UX Design", "Figma", "Adobe Creative Suite", "User Research" },
                VacationDaysUsed = 15,
                VacationDaysTotal = 20
            }
        };
    }
}

public class RoleService : IRoleService
{
    private readonly ILocalStorageService _localStorage;
    private const string ROLES_KEY = "roles";

    public RoleService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<List<Role>> GetAllRolesAsync()
    {
        try
        {
            var roles = await _localStorage.GetItemAsync<List<Role>>(ROLES_KEY);
            if (roles == null || roles.Count == 0)
            {
                await InitializeDefaultRolesAsync();
                roles = await _localStorage.GetItemAsync<List<Role>>(ROLES_KEY) ?? new List<Role>();
            }
            return roles;
        }
        catch
        {
            await InitializeDefaultRolesAsync();
            return await _localStorage.GetItemAsync<List<Role>>(ROLES_KEY) ?? new List<Role>();
        }
    }

    public async Task<Role?> GetRoleByIdAsync(Guid id)
    {
        var roles = await GetAllRolesAsync();
        return roles.FirstOrDefault(r => r.Id == id);
    }

    public async Task<Role> CreateRoleAsync(Role role)
    {
        role.Id = Guid.NewGuid();
        role.CreatedAt = DateTime.UtcNow;
        role.UpdatedAt = DateTime.UtcNow;
        
        var roles = await GetAllRolesAsync();
        roles.Add(role);
        await _localStorage.SetItemAsync(ROLES_KEY, roles);
        
        return role;
    }

    public async Task<Role> UpdateRoleAsync(Role role)
    {
        role.UpdatedAt = DateTime.UtcNow;
        
        var roles = await GetAllRolesAsync();
        var index = roles.FindIndex(r => r.Id == role.Id);
        if (index >= 0)
        {
            roles[index] = role;
            await _localStorage.SetItemAsync(ROLES_KEY, roles);
        }
        
        return role;
    }

    public async Task<bool> DeleteRoleAsync(Guid id)
    {
        var roles = await GetAllRolesAsync();
        var role = roles.FirstOrDefault(r => r.Id == id);
        if (role != null && !role.IsSystemRole)
        {
            roles.Remove(role);
            await _localStorage.SetItemAsync(ROLES_KEY, roles);
            return true;
        }
        return false;
    }

    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
        return SystemPermissions.AllPermissions;
    }

    public async Task InitializeDefaultRolesAsync()
    {
        var existingRoles = await _localStorage.GetItemAsync<List<Role>>(ROLES_KEY);
        if (existingRoles == null || existingRoles.Count == 0)
        {
            await _localStorage.SetItemAsync(ROLES_KEY, SystemPermissions.DefaultRoles);
        }
    }


}
