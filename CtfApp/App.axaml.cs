using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CtfApp.Views;
using CtfApp.ViewModels;
using System;  // ← ДОБАВЬ

namespace CtfApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Инициализация базы данных
        using (var db = new CtfApp.Data.AppDbContext())
        {
            db.Database.EnsureCreated();
            if (!System.Linq.Enumerable.Any(db.Users))
            {
                db.Users.Add(new CtfApp.Data.User 
                { 
                    Username = "admin", 
                    Password = "$2a$12$m6abb5gocDQezrc/B7.NJ.nN2GqD8VKuC596aABnF50UOw/wuaFWi",
                    IsAdmin = true, 
                    Score = 0 
                });
                db.Tasks.Add(new CtfApp.Data.CtfTask
                {
                    Title = "Test Task",
                    Description = "This is a test task",
                    Flag = "flag{1}",
                    Points = 10
                });
                db.SaveChanges();
            }
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
