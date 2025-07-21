using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.IO;

namespace CrashHaxlor;

public partial class CrashHaxlor : Window
{
    private string[] _crashInfo;
    private string _appPath = Directory.GetCurrentDirectory();
    private string? _logPath;

    public CrashHaxlor()
    {
        InitializeComponent();
        _crashInfo = new[] { string.Join("", Environment.GetCommandLineArgs()[1..]) };

        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (_crashInfo[0] == "") {
            _crashInfo[0] = "There seems to be no information provided with this crash.";
        }
        string info = $"To prevent leaving you in total confusion, here's some information I've gathered:\n\n{_crashInfo[0]}";
        InfoText.Text = info;

        
        string logsDir = Path.Combine(_appPath, "logs");
        if (!Directory.Exists(logsDir))
        {
            Directory.CreateDirectory(logsDir);
        }

        string dateNTime = DateTime.Now.ToString("yyyy-MM-dd - HH-mm-ss");
        string logName = $"{dateNTime} - crashlog.txt";
        _logPath = Path.Combine(logsDir, logName);

        File.WriteAllLines(_logPath, new[]
        {
            $"Engine crash at {DateTime.Now}",
            "",
            _crashInfo[0]
        });
        
        ReportText.Text = $"If your game keeps crashing with the same error, please report it at our GitHub with the log file included at: https://github.com/uh-and-who/VK\n\nA log file \"{logName}\" has been created at\n{logsDir}";
    }

    private void RelaunchClicked(object? sender, RoutedEventArgs e)
    {
        var exeName = OperatingSystem.IsWindows() ? "OurpleEngine.exe" : "OurpleEngine";
        var currentDirectory = AppContext.BaseDirectory;
        var enginePath = Path.Combine(currentDirectory, exeName);

        try { 
            Process.Start(new ProcessStartInfo
            {
                FileName = enginePath,
                WorkingDirectory = currentDirectory
            });
        } catch (Exception ex) {
            Console.WriteLine($"Failed to launch the game with the following error: {ex}");
            if (_logPath != null) {
                File.AppendAllText(_logPath, $"\nFailed to relaunch the game:\n{ex}");
            }
        }
        Close();
    }

    private void ExitClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
