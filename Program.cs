/*
 * Project: Blazor Control Panel
 * Date: 2025-08-08
 * Author: J Schneider - j.g@live.com
 */

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorControlPanel;
using BlazorControlPanel.Services;
using MudBlazor.Services;
using Blazored.LocalStorage;

// <summary>
// Application entry point and dependency injection configuration for the Blazor Control Panel.
// Configures services, components, and application startup settings for the WebAssembly host.
// </summary>
// <remarks>
// This file sets up the entire application including:
// - Root component registration
// - HTTP client configuration
// - MudBlazor UI framework setup
// - Local storage services
// - Business service registrations
// - Application startup and hosting
// </remarks>
var builder = WebAssemblyHostBuilder.CreateDefault(args);
try
{
// Configure root components
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for static hosting
    builder.Services.AddScoped(sp => new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        //Timeout = TimeSpan.FromSeconds(30)
    });

//Add MudBlazor services with optimized configuration
    builder.Services.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomLeft;
        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.NewestOnTop = false;
        config.SnackbarConfiguration.ShowCloseIcon = true;
        config.SnackbarConfiguration.VisibleStateDuration = 6000;
        config.SnackbarConfiguration.HideTransitionDuration = 500;
        config.SnackbarConfiguration.ShowTransitionDuration = 500;
        config.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
    });

// Add LocalStorage services with optimized configuration
    builder.Services.AddBlazoredLocalStorage(config =>
    {
        config.JsonSerializerOptions.WriteIndented = false;
        config.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Add custom services
    builder.Services.AddScoped<IUiPersonalizationService, UiPersonalizationService>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<IStaffService, StaffService>();
    builder.Services.AddScoped<IEstimateService, EstimateService>();
    builder.Services.AddScoped<IInvoiceService, InvoiceService>();
    builder.Services.AddScoped<ILeadService, LeadService>();
    builder.Services.AddScoped<IProjectService, ProjectService>();
    builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// Build and configure the application
    await builder.Build().RunAsync();

// Log application start for static hosting
    Console.WriteLine("BlazorControlPanel starting...");
    
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

