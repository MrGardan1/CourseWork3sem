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
    [ObservableProperty] private string _statusMessage = "";
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
            StatusMessage = "Fill all fields";
            StatusColor = "#f9e2af";
            return;
        }

        if (Password != PasswordConfirm)
        {
            StatusMessage = "Passwords don't match";
            StatusColor = "#f38ba8";
            return;
        }

        using var db = new AppDbContext();
        
        if (db.Users.Any(u => u.Username == Username))
        {
            StatusMessage = "Username already exists";
            StatusColor = "#f38ba8";
            return;
        }

        var newUser = new User
        {
            Username = Username,
            Password = PasswordHelper.HashPassword(Password),
            IsAdmin = false,
            Score = 0
        };

        db.Users.Add(newUser);
        db.SaveChanges();

        StatusMessage = "Account created successfully!";
        StatusColor = "#a6e3a1";

        // Переход на логин через 1.5 секунды
        System.Threading.Tasks.Task.Delay(1500).ContinueWith(_ =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>
            {
                _mainVm.SwitchToLoginCommand.Execute(null);
            });
        });
    }

    [RelayCommand]
    private void BackToLogin()
    {
        _mainVm.SwitchToLogin();
    }
}
