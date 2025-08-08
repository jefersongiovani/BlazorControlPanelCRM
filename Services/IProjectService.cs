/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

using BlazorControlPanel.Models;
using Blazored.LocalStorage;

namespace BlazorControlPanel.Services;

/// <summary>
/// Interface defining project management operations and data access methods.
/// Provides contract for project CRUD operations, task management, and project analytics.
/// </summary>
/// <remarks>
/// Defines the service layer contract for comprehensive project management including
/// project lifecycle management, team assignment, progress tracking, and reporting capabilities.
/// </remarks>
public interface IProjectService
{
    Task<List<Project>> GetAllProjectsAsync();
    Task<Project?> GetProjectByIdAsync(Guid id);
    Task<Project> CreateProjectAsync(Project project);
    Task<Project> UpdateProjectAsync(Project project);
    Task<bool> DeleteProjectAsync(Guid id);
    Task<List<Project>> GetProjectsByCustomerAsync(Guid customerId);
    Task<List<Project>> GetProjectsByStatusAsync(ProjectStatus status);
    Task<List<Project>> GetProjectsByManagerAsync(Guid managerId);
    Task<List<Project>> GetProjectsByTeamMemberAsync(Guid staffId);
    Task<List<Project>> SearchProjectsAsync(string searchTerm);
    Task<string> GenerateProjectCodeAsync();
    Task<ProjectStatistics> GetProjectStatisticsAsync();
    Task<List<Project>> GetOverdueProjectsAsync();
    Task<List<Project>> GetActiveProjectsAsync();
}

public interface ITaskService
{
    Task<List<ProjectTask>> GetTasksByProjectAsync(Guid projectId);
    Task<ProjectTask?> GetTaskByIdAsync(Guid taskId);
    Task<ProjectTask> CreateTaskAsync(ProjectTask task);
    Task<ProjectTask> UpdateTaskAsync(ProjectTask task);
    Task<bool> DeleteTaskAsync(Guid taskId);
    Task<List<ProjectTask>> GetTasksByAssigneeAsync(Guid staffId);
    Task<List<ProjectTask>> GetTasksByStatusAsync(Models.TaskStatus status);
    Task<List<ProjectTask>> GetOverdueTasksAsync();
    Task<List<ProjectTask>> GetTasksDueSoonAsync(int days = 7);
    Task<ProjectTask> UpdateTaskProgressAsync(Guid taskId, decimal progress);
    Task<ProjectTask> CompleteTaskAsync(Guid taskId);
}

public interface ITimeTrackingService
{
    Task<List<TimeEntry>> GetTimeEntriesByProjectAsync(Guid projectId);
    Task<List<TimeEntry>> GetTimeEntriesByStaffAsync(Guid staffId);
    Task<List<TimeEntry>> GetTimeEntriesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<TimeEntry> CreateTimeEntryAsync(TimeEntry timeEntry);
    Task<TimeEntry> UpdateTimeEntryAsync(TimeEntry timeEntry);
    Task<bool> DeleteTimeEntryAsync(Guid timeEntryId);
    Task<decimal> GetTotalHoursByProjectAsync(Guid projectId);
    Task<decimal> GetTotalHoursByStaffAsync(Guid staffId, DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> GetTotalBillableAmountAsync(Guid projectId);
}

public class ProjectStatistics
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int OverdueProjects { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalActualCost { get; set; }
    public decimal BudgetVariance { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public decimal AverageProjectCompletion { get; set; }
    public Dictionary<ProjectStatus, int> ProjectsByStatus { get; set; } = new();
    public Dictionary<ProjectType, int> ProjectsByType { get; set; } = new();
}

public class ProjectService : IProjectService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ICustomerService _customerService;
    private readonly IStaffService _staffService;
    private const string PROJECTS_KEY = "projects";

    public ProjectService(ILocalStorageService localStorage, ICustomerService customerService, IStaffService staffService)
    {
        _localStorage = localStorage;
        _customerService = customerService;
        _staffService = staffService;
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
        try
        {
            var projects = await _localStorage.GetItemAsync<List<Project>>(PROJECTS_KEY);
            if (projects == null || projects.Count == 0)
            {
                projects = await GetSampleProjectsAsync();
                await _localStorage.SetItemAsync(PROJECTS_KEY, projects);
            }
            
            // Load related data
            await LoadRelatedDataAsync(projects);
            return projects.OrderByDescending(p => p.CreatedAt).ToList();
        }
        catch
        {
            return await GetSampleProjectsAsync();
        }
    }

    public async Task<Project?> GetProjectByIdAsync(Guid id)
    {
        var projects = await GetAllProjectsAsync();
        return projects.FirstOrDefault(p => p.Id == id);
    }

    public async Task<Project> CreateProjectAsync(Project project)
    {
        project.Id = Guid.NewGuid();
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        
        if (string.IsNullOrEmpty(project.ProjectCode))
        {
            project.ProjectCode = await GenerateProjectCodeAsync();
        }
        
        var projects = await GetAllProjectsAsync();
        projects.Add(project);
        await _localStorage.SetItemAsync(PROJECTS_KEY, projects);
        
        return project;
    }

    public async Task<Project> UpdateProjectAsync(Project project)
    {
        project.UpdatedAt = DateTime.UtcNow;
        
        var projects = await GetAllProjectsAsync();
        var index = projects.FindIndex(p => p.Id == project.Id);
        if (index >= 0)
        {
            projects[index] = project;
            await _localStorage.SetItemAsync(PROJECTS_KEY, projects);
        }
        
        return project;
    }

    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        var projects = await GetAllProjectsAsync();
        var project = projects.FirstOrDefault(p => p.Id == id);
        if (project != null)
        {
            projects.Remove(project);
            await _localStorage.SetItemAsync(PROJECTS_KEY, projects);
            return true;
        }
        return false;
    }

    public async Task<List<Project>> GetProjectsByCustomerAsync(Guid customerId)
    {
        var projects = await GetAllProjectsAsync();
        return projects.Where(p => p.CustomerId == customerId).ToList();
    }

    public async Task<List<Project>> GetProjectsByStatusAsync(ProjectStatus status)
    {
        var projects = await GetAllProjectsAsync();
        return projects.Where(p => p.Status == status).ToList();
    }

    public async Task<List<Project>> GetProjectsByManagerAsync(Guid managerId)
    {
        var projects = await GetAllProjectsAsync();
        return projects.Where(p => p.ProjectManagerId == managerId).ToList();
    }

    public async Task<List<Project>> GetProjectsByTeamMemberAsync(Guid staffId)
    {
        var projects = await GetAllProjectsAsync();
        return projects.Where(p => p.TeamMemberIds.Contains(staffId) || p.ProjectManagerId == staffId).ToList();
    }

    public async Task<List<Project>> SearchProjectsAsync(string searchTerm)
    {
        var projects = await GetAllProjectsAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return projects;

        searchTerm = searchTerm.ToLower();
        return projects.Where(p => 
            p.Name.ToLower().Contains(searchTerm) ||
            p.Description.ToLower().Contains(searchTerm) ||
            p.ProjectCode.ToLower().Contains(searchTerm) ||
            (p.Customer?.DisplayName?.ToLower().Contains(searchTerm) ?? false)
        ).ToList();
    }

    public async Task<string> GenerateProjectCodeAsync()
    {
        var projects = await GetAllProjectsAsync();
        var year = DateTime.UtcNow.Year;
        var count = projects.Count(p => p.CreatedAt.Year == year) + 1;
        return $"PRJ-{year}-{count:D4}";
    }

    public async Task<ProjectStatistics> GetProjectStatisticsAsync()
    {
        var projects = await GetAllProjectsAsync();
        var totalProjects = projects.Count;
        var activeProjects = projects.Count(p => p.IsActive);
        var completedProjects = projects.Count(p => p.Status == ProjectStatus.Completed);
        var overdueProjects = projects.Count(p => p.IsOverdue);

        return new ProjectStatistics
        {
            TotalProjects = totalProjects,
            ActiveProjects = activeProjects,
            CompletedProjects = completedProjects,
            OverdueProjects = overdueProjects,
            TotalBudget = projects.Sum(p => p.Budget),
            TotalActualCost = projects.Sum(p => p.ActualCost),
            BudgetVariance = projects.Sum(p => p.BudgetVariance),
            TotalTasks = projects.Sum(p => p.TotalTasksCount),
            CompletedTasks = projects.Sum(p => p.CompletedTasksCount),
            OverdueTasks = projects.SelectMany(p => p.Tasks).Count(t => t.IsOverdue),
            AverageProjectCompletion = projects.Any() ? projects.Average(p => p.ProgressPercentage) : 0,
            ProjectsByStatus = projects.GroupBy(p => p.Status).ToDictionary(g => g.Key, g => g.Count()),
            ProjectsByType = projects.GroupBy(p => p.Type).ToDictionary(g => g.Key, g => g.Count())
        };
    }

    public async Task<List<Project>> GetOverdueProjectsAsync()
    {
        var projects = await GetAllProjectsAsync();
        return projects.Where(p => p.IsOverdue).ToList();
    }

    public async Task<List<Project>> GetActiveProjectsAsync()
    {
        var projects = await GetAllProjectsAsync();
        return projects.Where(p => p.IsActive).ToList();
    }

    private async Task LoadRelatedDataAsync(List<Project> projects)
    {
        var customers = await _customerService.GetAllCustomersAsync();
        var staff = await _staffService.GetAllStaffAsync();
        
        foreach (var project in projects)
        {
            project.Customer = customers.FirstOrDefault(c => c.Id == project.CustomerId);
            
            if (project.ProjectManagerId.HasValue)
            {
                project.ProjectManager = staff.FirstOrDefault(s => s.Id == project.ProjectManagerId.Value);
            }
            
            project.TeamMembers = staff.Where(s => project.TeamMemberIds.Contains(s.Id)).ToList();
        }
    }

    private async Task<List<Project>> GetSampleProjectsAsync()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        var staff = await _staffService.GetAllStaffAsync();
        var sampleProjects = new List<Project>();

        if (customers.Any() && staff.Any())
        {
            var customer1 = customers.First();
            var customer2 = customers.Skip(1).FirstOrDefault() ?? customer1;
            var manager = staff.FirstOrDefault();
            var developer = staff.Skip(1).FirstOrDefault() ?? manager;

            sampleProjects.AddRange(new List<Project>
            {
                new Project
                {
                    Name = "E-commerce Website Redesign",
                    Description = "Complete redesign of the company's e-commerce platform with modern UI/UX and improved performance",
                    ProjectCode = "PRJ-2024-0001",
                    Status = ProjectStatus.InProgress,
                    Priority = ProjectPriority.High,
                    Type = ProjectType.Development,
                    CustomerId = customer1.Id,
                    Customer = customer1,
                    ProjectManagerId = manager?.Id,
                    ProjectManager = manager,
                    TeamMemberIds = staff.Take(3).Select(s => s.Id).ToList(),
                    TeamMembers = staff.Take(3).ToList(),
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    EndDate = DateTime.UtcNow.AddDays(45),
                    Budget = 85000,
                    ActualCost = 42000,
                    EstimatedHours = 680,
                    ActualHours = 340,
                    ProgressPercentage = 65,
                    Tags = new List<string> { "E-commerce", "Web Development", "UI/UX" },
                    Tasks = new List<ProjectTask>
                    {
                        new ProjectTask
                        {
                            Title = "Requirements Analysis",
                            Description = "Gather and analyze business requirements",
                            Status = Models.TaskStatus.Completed,
                            Priority = TaskPriority.High,
                            AssignedToId = manager?.Id,
                            EstimatedHours = 40,
                            ActualHours = 38,
                            ProgressPercentage = 100,
                            CompletedDate = DateTime.UtcNow.AddDays(-25)
                        },
                        new ProjectTask
                        {
                            Title = "UI/UX Design",
                            Description = "Create wireframes and visual designs",
                            Status = Models.TaskStatus.Completed,
                            Priority = TaskPriority.High,
                            AssignedToId = developer?.Id,
                            EstimatedHours = 80,
                            ActualHours = 85,
                            ProgressPercentage = 100,
                            CompletedDate = DateTime.UtcNow.AddDays(-15)
                        },
                        new ProjectTask
                        {
                            Title = "Frontend Development",
                            Description = "Implement responsive frontend components",
                            Status = Models.TaskStatus.InProgress,
                            Priority = TaskPriority.High,
                            AssignedToId = developer?.Id,
                            EstimatedHours = 200,
                            ActualHours = 120,
                            ProgressPercentage = 60,
                            DueDate = DateTime.UtcNow.AddDays(20)
                        }
                    },
                    CreatedAt = DateTime.UtcNow.AddDays(-35)
                },
                new Project
                {
                    Name = "Mobile App Development",
                    Description = "Native iOS and Android application for customer portal",
                    ProjectCode = "PRJ-2024-0002",
                    Status = ProjectStatus.Planning,
                    Priority = ProjectPriority.Medium,
                    Type = ProjectType.Development,
                    CustomerId = customer2.Id,
                    Customer = customer2,
                    ProjectManagerId = manager?.Id,
                    ProjectManager = manager,
                    TeamMemberIds = staff.Take(2).Select(s => s.Id).ToList(),
                    TeamMembers = staff.Take(2).ToList(),
                    StartDate = DateTime.UtcNow.AddDays(7),
                    EndDate = DateTime.UtcNow.AddDays(120),
                    Budget = 120000,
                    ActualCost = 5000,
                    EstimatedHours = 960,
                    ActualHours = 40,
                    ProgressPercentage = 15,
                    Tags = new List<string> { "Mobile App", "iOS", "Android" },
                    Tasks = new List<ProjectTask>
                    {
                        new ProjectTask
                        {
                            Title = "Project Planning",
                            Description = "Define project scope and timeline",
                            Status = Models.TaskStatus.InProgress,
                            Priority = TaskPriority.High,
                            AssignedToId = manager?.Id,
                            EstimatedHours = 24,
                            ActualHours = 18,
                            ProgressPercentage = 75,
                            DueDate = DateTime.UtcNow.AddDays(3)
                        }
                    },
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Project
                {
                    Name = "Legacy System Migration",
                    Description = "Migrate legacy database and applications to modern cloud infrastructure",
                    ProjectCode = "PRJ-2024-0003",
                    Status = ProjectStatus.Completed,
                    Priority = ProjectPriority.Critical,
                    Type = ProjectType.Development,
                    CustomerId = customer1.Id,
                    Customer = customer1,
                    ProjectManagerId = manager?.Id,
                    ProjectManager = manager,
                    TeamMemberIds = staff.Select(s => s.Id).ToList(),
                    TeamMembers = staff.ToList(),
                    StartDate = DateTime.UtcNow.AddDays(-90),
                    EndDate = DateTime.UtcNow.AddDays(-10),
                    ActualStartDate = DateTime.UtcNow.AddDays(-88),
                    ActualEndDate = DateTime.UtcNow.AddDays(-8),
                    Budget = 150000,
                    ActualCost = 145000,
                    EstimatedHours = 1200,
                    ActualHours = 1180,
                    ProgressPercentage = 100,
                    Tags = new List<string> { "Migration", "Cloud", "Database" },
                    CreatedAt = DateTime.UtcNow.AddDays(-95)
                }
            });
        }

        return sampleProjects;
    }
}

public class TaskService : ITaskService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IStaffService _staffService;
    private const string TASKS_KEY = "project_tasks";

    public TaskService(ILocalStorageService localStorage, IStaffService staffService)
    {
        _localStorage = localStorage;
        _staffService = staffService;
    }

    public async Task<List<ProjectTask>> GetTasksByProjectAsync(Guid projectId)
    {
        var tasks = await GetAllTasksAsync();
        return tasks.Where(t => t.ProjectId == projectId).OrderBy(t => t.CreatedAt).ToList();
    }

    public async Task<ProjectTask?> GetTaskByIdAsync(Guid taskId)
    {
        var tasks = await GetAllTasksAsync();
        return tasks.FirstOrDefault(t => t.Id == taskId);
    }

    public async Task<ProjectTask> CreateTaskAsync(ProjectTask task)
    {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        var tasks = await GetAllTasksAsync();
        tasks.Add(task);
        await _localStorage.SetItemAsync(TASKS_KEY, tasks);

        return task;
    }

    public async Task<ProjectTask> UpdateTaskAsync(ProjectTask task)
    {
        task.UpdatedAt = DateTime.UtcNow;

        var tasks = await GetAllTasksAsync();
        var index = tasks.FindIndex(t => t.Id == task.Id);
        if (index >= 0)
        {
            tasks[index] = task;
            await _localStorage.SetItemAsync(TASKS_KEY, tasks);
        }

        return task;
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        var tasks = await GetAllTasksAsync();
        var task = tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            tasks.Remove(task);
            await _localStorage.SetItemAsync(TASKS_KEY, tasks);
            return true;
        }
        return false;
    }

    public async Task<List<ProjectTask>> GetTasksByAssigneeAsync(Guid staffId)
    {
        var tasks = await GetAllTasksAsync();
        return tasks.Where(t => t.AssignedToId == staffId).ToList();
    }

    public async Task<List<ProjectTask>> GetTasksByStatusAsync(Models.TaskStatus status)
    {
        var tasks = await GetAllTasksAsync();
        return tasks.Where(t => t.Status == status).ToList();
    }

    public async Task<List<ProjectTask>> GetOverdueTasksAsync()
    {
        var tasks = await GetAllTasksAsync();
        return tasks.Where(t => t.IsOverdue).ToList();
    }

    public async Task<List<ProjectTask>> GetTasksDueSoonAsync(int days = 7)
    {
        var tasks = await GetAllTasksAsync();
        var cutoffDate = DateTime.UtcNow.AddDays(days);
        return tasks.Where(t =>
            t.DueDate.HasValue &&
            t.DueDate.Value <= cutoffDate &&
            t.Status != Models.TaskStatus.Completed
        ).ToList();
    }

    public async Task<ProjectTask> UpdateTaskProgressAsync(Guid taskId, decimal progress)
    {
        var task = await GetTaskByIdAsync(taskId);
        if (task != null)
        {
            task.ProgressPercentage = Math.Max(0, Math.Min(100, progress));
            if (progress >= 100)
            {
                task.Status = Models.TaskStatus.Completed;
                task.CompletedDate = DateTime.UtcNow;
            }
            await UpdateTaskAsync(task);
        }
        return task!;
    }

    public async Task<ProjectTask> CompleteTaskAsync(Guid taskId)
    {
        var task = await GetTaskByIdAsync(taskId);
        if (task != null)
        {
            task.Status = Models.TaskStatus.Completed;
            task.ProgressPercentage = 100;
            task.CompletedDate = DateTime.UtcNow;
            await UpdateTaskAsync(task);
        }
        return task!;
    }

    private async Task<List<ProjectTask>> GetAllTasksAsync()
    {
        try
        {
            var tasks = await _localStorage.GetItemAsync<List<ProjectTask>>(TASKS_KEY);
            if (tasks == null)
            {
                tasks = new List<ProjectTask>();
                await _localStorage.SetItemAsync(TASKS_KEY, tasks);
            }

            // Load staff data
            var staff = await _staffService.GetAllStaffAsync();
            foreach (var task in tasks)
            {
                if (task.AssignedToId.HasValue)
                {
                    task.AssignedTo = staff.FirstOrDefault(s => s.Id == task.AssignedToId.Value);
                }
            }

            return tasks;
        }
        catch
        {
            return new List<ProjectTask>();
        }
    }
}

public class TimeTrackingService : ITimeTrackingService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IStaffService _staffService;
    private readonly IProjectService _projectService;
    private const string TIME_ENTRIES_KEY = "time_entries";

    public TimeTrackingService(ILocalStorageService localStorage, IStaffService staffService, IProjectService projectService)
    {
        _localStorage = localStorage;
        _staffService = staffService;
        _projectService = projectService;
    }

    public async Task<List<TimeEntry>> GetTimeEntriesByProjectAsync(Guid projectId)
    {
        var entries = await GetAllTimeEntriesAsync();
        return entries.Where(e => e.ProjectId == projectId).OrderByDescending(e => e.Date).ToList();
    }

    public async Task<List<TimeEntry>> GetTimeEntriesByStaffAsync(Guid staffId)
    {
        var entries = await GetAllTimeEntriesAsync();
        return entries.Where(e => e.StaffId == staffId).OrderByDescending(e => e.Date).ToList();
    }

    public async Task<List<TimeEntry>> GetTimeEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var entries = await GetAllTimeEntriesAsync();
        return entries.Where(e => e.Date >= startDate.Date && e.Date <= endDate.Date).ToList();
    }

    public async Task<TimeEntry> CreateTimeEntryAsync(TimeEntry timeEntry)
    {
        timeEntry.Id = Guid.NewGuid();
        timeEntry.CreatedAt = DateTime.UtcNow;

        var entries = await GetAllTimeEntriesAsync();
        entries.Add(timeEntry);
        await _localStorage.SetItemAsync(TIME_ENTRIES_KEY, entries);

        return timeEntry;
    }

    public async Task<TimeEntry> UpdateTimeEntryAsync(TimeEntry timeEntry)
    {
        var entries = await GetAllTimeEntriesAsync();
        var index = entries.FindIndex(e => e.Id == timeEntry.Id);
        if (index >= 0)
        {
            entries[index] = timeEntry;
            await _localStorage.SetItemAsync(TIME_ENTRIES_KEY, entries);
        }

        return timeEntry;
    }

    public async Task<bool> DeleteTimeEntryAsync(Guid timeEntryId)
    {
        var entries = await GetAllTimeEntriesAsync();
        var entry = entries.FirstOrDefault(e => e.Id == timeEntryId);
        if (entry != null)
        {
            entries.Remove(entry);
            await _localStorage.SetItemAsync(TIME_ENTRIES_KEY, entries);
            return true;
        }
        return false;
    }

    public async Task<decimal> GetTotalHoursByProjectAsync(Guid projectId)
    {
        var entries = await GetTimeEntriesByProjectAsync(projectId);
        return entries.Sum(e => e.Hours);
    }

    public async Task<decimal> GetTotalHoursByStaffAsync(Guid staffId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var entries = await GetTimeEntriesByStaffAsync(staffId);

        if (startDate.HasValue)
            entries = entries.Where(e => e.Date >= startDate.Value.Date).ToList();

        if (endDate.HasValue)
            entries = entries.Where(e => e.Date <= endDate.Value.Date).ToList();

        return entries.Sum(e => e.Hours);
    }

    public async Task<decimal> GetTotalBillableAmountAsync(Guid projectId)
    {
        var entries = await GetTimeEntriesByProjectAsync(projectId);
        return entries.Where(e => e.IsBillable).Sum(e => e.Amount);
    }

    private async Task<List<TimeEntry>> GetAllTimeEntriesAsync()
    {
        try
        {
            var entries = await _localStorage.GetItemAsync<List<TimeEntry>>(TIME_ENTRIES_KEY);
            return entries ?? new List<TimeEntry>();
        }
        catch
        {
            return new List<TimeEntry>();
        }
    }
}
