using ElectronNET.API;
using ElectronNET.API.Entities;
using MudBlazor.Services;
using PSCenter.Components;
using PSCenter.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddElectron();
builder.Services.AddSingleton<PowerShellService>();

builder.UseElectron(args, async () =>
{
    var options = new BrowserWindowOptions
    {
        Width = 1280,
        Height = 820,
        MinWidth = 960,
        MinHeight = 640,
        Show = false,
        IsRunningBlazor = true,
        BackgroundColor = "#0f172a"
    };

    if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
    {
        options.AutoHideMenuBar = true;
    }

    var window = await Electron.WindowManager.CreateWindowAsync(options);
    window.OnReadyToShow += () => window.Show();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
