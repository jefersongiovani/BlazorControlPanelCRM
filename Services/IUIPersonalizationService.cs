/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

using BlazorControlPanel.Models;
using Blazored.LocalStorage;

namespace BlazorControlPanel.Services;

/// <summary>
/// Interface defining UI personalization and user preference management operations.
/// Provides contract for managing user interface settings, themes, and access logging.
/// </summary>
/// <remarks>
/// Defines the service layer contract for user experience customization including
/// theme management, layout preferences, and user activity tracking.
/// </remarks>
public interface IUIPersonalizationService
{
    Task<UISettings> GetUISettingsAsync();
    Task SaveUISettingsAsync(UISettings settings);
    Task<List<AccessLog>> GetAccessLogsAsync();
    Task LogAccessAsync(string action, string details = "");
    Task ClearAccessLogsAsync();
    Task<T?> GetUserPreferenceAsync<T>(string key);
    Task SetUserPreferenceAsync<T>(string key, T value);
    Task RemoveUserPreferenceAsync(string key);
    Task ClearAllUserPreferencesAsync();
}

public class UIPersonalizationService : IUIPersonalizationService
{
    private readonly ILocalStorageService _localStorage;
    private const string UI_SETTINGS_KEY = "ui_settings";
    private const string ACCESS_LOGS_KEY = "access_logs";
    private const string USER_PREFERENCES_PREFIX = "user_pref_";

    public UIPersonalizationService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<UISettings> GetUISettingsAsync()
    {
        try
        {
            var settings = await _localStorage.GetItemAsync<UISettings>(UI_SETTINGS_KEY);
            return settings ?? new UISettings();
        }
        catch
        {
            return new UISettings();
        }
    }

    public async Task SaveUISettingsAsync(UISettings settings)
    {
        settings.LastUpdated = DateTime.UtcNow;
        await _localStorage.SetItemAsync(UI_SETTINGS_KEY, settings);
    }

    public async Task<List<AccessLog>> GetAccessLogsAsync()
    {
        try
        {
            var logs = await _localStorage.GetItemAsync<List<AccessLog>>(ACCESS_LOGS_KEY);
            return logs ?? new List<AccessLog>();
        }
        catch
        {
            return new List<AccessLog>();
        }
    }

    public async Task LogAccessAsync(string action, string details = "")
    {
        var logs = await GetAccessLogsAsync();
        var log = new AccessLog
        {
            Action = action,
            Details = details,
            Timestamp = DateTime.UtcNow
        };

        logs.Add(log);

        // Keep only last 1000 logs to prevent storage bloat
        if (logs.Count > 1000)
        {
            logs = logs.OrderByDescending(l => l.Timestamp).Take(1000).ToList();
        }

        await _localStorage.SetItemAsync(ACCESS_LOGS_KEY, logs);
    }

    public async Task ClearAccessLogsAsync()
    {
        await _localStorage.RemoveItemAsync(ACCESS_LOGS_KEY);
    }

    public async Task<T?> GetUserPreferenceAsync<T>(string key)
    {
        try
        {
            return await _localStorage.GetItemAsync<T>($"{USER_PREFERENCES_PREFIX}{key}");
        }
        catch
        {
            return default(T);
        }
    }

    public async Task SetUserPreferenceAsync<T>(string key, T value)
    {
        await _localStorage.SetItemAsync($"{USER_PREFERENCES_PREFIX}{key}", value);
    }

    public async Task RemoveUserPreferenceAsync(string key)
    {
        await _localStorage.RemoveItemAsync($"{USER_PREFERENCES_PREFIX}{key}");
    }

    public async Task ClearAllUserPreferencesAsync()
    {
        var keys = await _localStorage.KeysAsync();
        var prefKeys = keys.Where(k => k.StartsWith(USER_PREFERENCES_PREFIX));

        foreach (var key in prefKeys)
        {
            await _localStorage.RemoveItemAsync(key);
        }
    }
}
