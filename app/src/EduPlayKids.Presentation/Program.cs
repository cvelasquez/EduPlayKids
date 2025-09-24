namespace EduPlayKids.App;

/// <summary>
/// Main entry point for the EduPlayKids application.
/// </summary>
public static class Program
{
    /// <summary>
    /// Main method - entry point for the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    public static void Main(string[] args)
    {
        var app = MauiProgram.CreateMauiApp();

        // Start the application
        var appInstance = new App();
        Microsoft.Maui.Controls.Application.Current = appInstance;

        // Run the application
        appInstance.MainPage = new AppShell();
    }
}