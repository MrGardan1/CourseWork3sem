using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtfApp.Data;
using Avalonia.Controls.ApplicationLifetimes;

namespace CtfApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _statusMessage = "Ready for connection...";
    [ObservableProperty] private string _statusColor = "#585b70";
    [ObservableProperty] private bool _isLoggedIn = false;
    [ObservableProperty] private int _currentScore = 0;
    [ObservableProperty] private string _currentScreen = "Login";
    [ObservableProperty] private RegisterViewModel _registerViewModel;

    private int _failedAttempts = 0;
    private const int MaxAttempts = 5;

    public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();

    public MainWindowViewModel()
    {
        RegisterViewModel = new RegisterViewModel(this);
    }

    [RelayCommand]
    public void SwitchToLogin()
    {
        CurrentScreen = "Login";
        StatusMessage = "Back to login";
        StatusColor = "#a6adc8";
        Username = "";
        Password = "";
    }

    [RelayCommand]
    public void SwitchToRegister()
    {
        CurrentScreen = "Register";
    }

    [RelayCommand]
    private void Login()
    {
        if (_failedAttempts >= MaxAttempts)
        {
            StatusMessage = "SECURITY LOCKOUT: Too many attempts.";
            StatusColor = "#f38ba8";
            return;
        }

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "Warning: Empty fields";
            StatusColor = "#f9e2af";
            return;
        }

        using var db = new AppDbContext();
        var user = db.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

        if (user != null)
        {
            StatusMessage = "ACCESS GRANTED";
            StatusColor = "#a6e3a1";
            IsLoggedIn = true;
            CurrentScore = user.Score;
            CurrentScreen = "Dashboard";
            _failedAttempts = 0;

            Tasks.Clear();
            foreach (var task in db.Tasks)
            {
                Tasks.Add(new TaskItemViewModel(task, this));
            }
        }
        else
        {
            _failedAttempts++;
            int remaining = MaxAttempts - _failedAttempts;
            Password = "";
            StatusColor = "#f38ba8";

            if (remaining > 0)
            {
                StatusMessage = $"ACCESS DENIED. Attempts left: {remaining}";
            }
            else
            {
                StatusMessage = "SYSTEM LOCKDOWN INITIATED...";
                System.Threading.Tasks.Task.Delay(1500).ContinueWith(_ =>
                {
                    if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        Avalonia.Threading.Dispatcher.UIThread.Invoke(() => desktop.Shutdown());
                    }
                });
            }
        }
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
    }
}
