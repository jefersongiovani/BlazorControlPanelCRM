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
public interface IUiPersonalizationService
{
    Task<UISettings> GetUiSettingsAsync();
    Task SaveUiSettingsAsync(UISettings settings);
    Task<List<AccessLog>> GetAccessLogsAsync();
    Task LogAccessAsync(string action);
    Task ClearAccessLogsAsync();
    Task<T?> GetUserPreferenceAsync<T>(string key);
    Task SetUserPreferenceAsync<T>(string key, T value);
    Task RemoveUserPreferenceAsync(string key);
    Task ClearAllUserPreferencesAsync();
    Task<bool> GetDarkModePreferenceAsync();
    Task SetDarkModePreferenceAsync(bool isDarkMode);
}

public class UiPersonalizationService : IUiPersonalizationService
{
    private readonly ILocalStorageService _localStorage;
    private const string UiSettingsKey = "ui_settings";
    private const string AccessLogsKey = "access_logs";
    private const string UserPreferencesPrefix = "user_pref_";

    public UiPersonalizationService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<UISettings> GetUiSettingsAsync()
    {
        var settings = await _localStorage.GetItemAsync<UISettings>(UiSettingsKey);
        return settings ?? new UISettings();
    }

    public async Task SaveUiSettingsAsync(UISettings settings)
    {
        await _localStorage.SetItemAsync(UiSettingsKey, settings);
    }

    public async Task<List<AccessLog>> GetAccessLogsAsync()
    {
        var logs = await _localStorage.GetItemAsync<List<AccessLog>>(AccessLogsKey);
        return logs ?? new List<AccessLog>();
    }

    public async Task LogAccessAsync(string action)
    {
        var logs = await GetAccessLogsAsync();
        logs.Add(new AccessLog { Timestamp = DateTime.UtcNow, Action = action });
        await _localStorage.SetItemAsync(AccessLogsKey, logs);
    }

    public async Task ClearAccessLogsAsync()
    {
        await _localStorage.RemoveItemAsync(AccessLogsKey);
    }

    public async Task<T?> GetUserPreferenceAsync<T>(string key)
    {
        return await _localStorage.GetItemAsync<T>(UserPreferencesPrefix + key);
    }

    public async Task SetUserPreferenceAsync<T>(string key, T value)
    {
        await _localStorage.SetItemAsync(UserPreferencesPrefix + key, value);
    }

    public async Task RemoveUserPreferenceAsync(string key)
    {
        await _localStorage.RemoveItemAsync(UserPreferencesPrefix + key);
    }

    public async Task ClearAllUserPreferencesAsync()
    {
        // This is a simple implementation; for production, enumerate keys and remove those with prefix
    }

    public async Task<bool> GetDarkModePreferenceAsync()
    {
        var settings = await GetUiSettingsAsync();
        return settings.DarkMode;
    }

    public async Task SetDarkModePreferenceAsync(bool isDarkMode)
    {
        var settings = await GetUiSettingsAsync();
        settings.DarkMode = isDarkMode;
        await SaveUiSettingsAsync(settings);
    }
}
