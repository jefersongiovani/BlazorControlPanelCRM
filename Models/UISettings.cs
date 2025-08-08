/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

namespace BlazorControlPanel.Models;

/// <summary>
/// UI settings entity representing user interface preferences and customization options.
/// Contains theme settings, layout preferences, and personalization data for individual users.
/// </summary>
/// <remarks>
/// Used for storing and managing user-specific UI preferences, enabling personalized
/// user experiences and maintaining consistent settings across sessions.
/// </remarks>
public class UISettings
{
    public string Theme { get; set; } = "light";
    public bool DarkMode { get; set; } = false;
    public string PrimaryColor { get; set; } = "blue";
    public string Language { get; set; } = "en";
    public bool CompactMode { get; set; } = false;
    public bool ShowSidebar { get; set; } = true;
    public int SidebarWidth { get; set; } = 240;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class AccessLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
