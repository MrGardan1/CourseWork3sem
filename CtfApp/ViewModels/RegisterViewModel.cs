using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtfApp.Data;

namespace CtfApp.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainVm;

    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _passwordConfirm = "";
    [ObservableProperty] private string _statusMessage = "Create new account";
    [ObservableProperty] private string _statusColor = "#a6adc8";

    public RegisterViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;
    }

    [RelayCommand]
    private void Register()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(PasswordConfirm))
        {
            StatusMessage = "All fields are required";
            StatusColor = "#f9e2af";
            return;
        }

        if (Password != PasswordConfirm)
        {
            StatusMessage = "Passwords do not match";
            StatusColor = "#f38ba8";
            PasswordConfirm = "";
            return;
        }

        if (Password.Length < 4)
        {
            StatusMessage = "Password must be at least 4 characters";
            StatusColor = "#f9e2af";
            return;
        }

        using var db = new AppDbContext();

        if (db.Users.Any(u => u.Username == Username))
        {
            StatusMessage = "Username already exists";
            StatusColor = "#f38ba8";
            Username = "";
            return;
        }

        try
        {
            var newUser = new User
            {
                Username = Username,
                Password = Password,
                Score = 0,
                IsAdmin = false
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            StatusMessage = "Registration successful! Redirecting to login...";
            StatusColor = "#a6e3a1";

            System.Threading.Tasks.Task.Delay(1500).ContinueWith(_ =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>
                {
                    _mainVm.SwitchToLogin();
                });
            });
        }
        catch
        {
            StatusMessage = "Database error. Try again.";
            StatusColor = "#f38ba8";
        }
    }

    [RelayCommand]
    private void BackToLogin()
    {
        _mainVm.SwitchToLogin();
    }
}
