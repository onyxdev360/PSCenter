using System.Diagnostics;

namespace PSCenter.Services;

public sealed class PowerShellService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PowerShellService> _logger;

    public PowerShellService(
        IWebHostEnvironment environment,
        ILogger<PowerShellService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<PowerShellExecutionResult> RunDemoAsync(
        CancellationToken cancellationToken = default)
    {
        var scriptPath = Path.Combine(
            _environment.ContentRootPath,
            "Scripts",
            "Demo.ps1");

        if (!File.Exists(scriptPath))
        {
            return new PowerShellExecutionResult(
                Executable: string.Empty,
                ExitCode: -1,
                Output: string.Empty,
                Error: $"PowerShell demo script was not found at: {scriptPath}",
                Duration: TimeSpan.Zero);
        }

        var executable = ResolvePowerShellExecutable();
        if (executable is null)
        {
            return new PowerShellExecutionResult(
                Executable: string.Empty,
                ExitCode: -1,
                Output: string.Empty,
                Error: "Neither pwsh nor powershell.exe could be found on this system.",
                Duration: TimeSpan.Zero);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = _environment.ContentRootPath
        };

        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");

        if (OperatingSystem.IsWindows())
        {
            startInfo.ArgumentList.Add("-ExecutionPolicy");
            startInfo.ArgumentList.Add("Bypass");
        }

        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);

        using var process = new Process { StartInfo = startInfo };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "Starting PowerShell demo with {Executable}: {ScriptPath}",
                executable,
                scriptPath);

            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            var output = await outputTask;
            var error = await errorTask;

            stopwatch.Stop();

            return new PowerShellExecutionResult(
                Executable: executable,
                ExitCode: process.ExitCode,
                Output: output.TrimEnd(),
                Error: error.TrimEnd(),
                Duration: stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "PowerShell demo execution failed.");

            return new PowerShellExecutionResult(
                Executable: executable,
                ExitCode: -1,
                Output: string.Empty,
                Error: ex.Message,
                Duration: stopwatch.Elapsed);
        }
    }

    private static string? ResolvePowerShellExecutable()
    {
        var candidates = OperatingSystem.IsWindows()
            ? new[] { "pwsh.exe", "powershell.exe" }
            : new[] { "pwsh" };

        foreach (var candidate in candidates)
        {
            if (CanStart(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static bool CanStart(string executable)
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = executable,
                Arguments = "-NoLogo -NoProfile -NonInteractive -Command \"exit 0\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            });

            if (process is null)
            {
                return false;
            }

            process.WaitForExit(3000);
            return process.HasExited && process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}

public sealed record PowerShellExecutionResult(
    string Executable,
    int ExitCode,
    string Output,
    string Error,
    TimeSpan Duration);
