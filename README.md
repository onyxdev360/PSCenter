# PSCenter

PSCenter is a desktop administration shell built with **Electron.NET**, **ASP.NET Core / Blazor**, and **MudBlazor**.

The initial vertical slice demonstrates a controlled PowerShell integration:

1. Open **PowerShell > Script Demo**.
2. Click **Run PowerShell Script**.
3. PSCenter executes the bundled `Scripts/Demo.ps1` file.
4. Standard output, standard error, exit code, executable, and duration are shown in the UI.

The demo is intentionally read-only and does not expose arbitrary command execution.

## Technology

- .NET 8
- ElectronNET.Core 0.5.1
- ElectronNET.Core.AspNet 0.5.1
- MudBlazor 9.10.0
- Blazor Interactive Server rendering
- PowerShell 7 (`pwsh`) when available, with Windows PowerShell fallback on Windows

## Prerequisites

Install:

- .NET 8 SDK or later
- Node.js 22.x or later
- PowerShell 7 is recommended; Windows PowerShell is supported as a fallback on Windows

Verify:

```powershell
dotnet --version
node --version
pwsh --version
```

## Run

Clone the repository and run:

```powershell
dotnet restore
dotnet run
```

ElectronNET.Core can launch from the .NET application directly. The Electron desktop window is created during startup.

## Project structure

```text
PSCenter/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   └── Pages/
│       ├── Home.razor
│       ├── PowerShellDemo.razor
│       └── ComingSoon.razor
├── Scripts/
│   └── Demo.ps1
├── Services/
│   └── PowerShellService.cs
├── wwwroot/
│   └── app.css
├── Program.cs
└── PSCenter.csproj
```

## PowerShell execution model

`PowerShellService` is the boundary between the UI and PowerShell.

It:

- resolves `pwsh` first and falls back to `powershell.exe` on Windows;
- launches PowerShell without a shell window;
- uses `-NoProfile` and `-NonInteractive`;
- executes a known script file rather than user-supplied command text;
- captures stdout and stderr independently;
- returns an explicit exit code and execution duration.

This is the pattern to extend for administrative features. Prefer purpose-built methods such as `UnlockUserAsync(username)` over a generic `ExecuteAsync(commandText)` endpoint.

## Next logical features

- Active Directory user search
- account unlock workflow
- machine/network diagnostics
- reusable confirmation dialogs for state-changing operations
- structured PowerShell JSON output rather than presentation-formatted text
- privilege/elevation checks
- audit logging
