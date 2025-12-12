using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using CtfApp.ViewModels;
using CtfApp.Views;

namespace CtfApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        using (var db = new CtfApp.Data.AppDbContext())
        {
            db.Database.EnsureCreated();
            
            if (!System.Linq.Enumerable.Any(db.Users))
            {
                db.Users.Add(new CtfApp.Data.User { Username = "admin", Password = "123", IsAdmin = true });
                db.Tasks.Add(new CtfApp.Data.CtfTask { Title = "Test", Description = "Test", Flag = "flag{1}", Points = 10 });
                db.SaveChanges();
            }
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }


    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}