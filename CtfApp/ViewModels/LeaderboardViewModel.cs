using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtfApp.Data;

namespace CtfApp.ViewModels;

public partial class LeaderboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    public ObservableCollection<LeaderboardEntry> Leaderboard { get; } = new();

    public LeaderboardViewModel()
    {
        // Конструктор без параметров для дизайнера
    }

    public LeaderboardViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        using var db = new AppDbContext();
        
        // Получаем всех пользователей (кроме админов), сортируем по очкам
        var users = db.Users
            .Where(u => !u.IsAdmin)  // Исключаем админов
            .OrderByDescending(u => u.Score)
            .ThenBy(u => u.Username)  // При равных очках сортируем по имени
            .ToList();
        
        Leaderboard.Clear();
        int position = 1;
        
        foreach (var user in users)
        {
            // Считаем количество решенных задач
            var solvedCount = db.UserTasks.Count(ut => ut.UserId == user.Id);
            
            Leaderboard.Add(new LeaderboardEntry
            {
                Position = position++,
                Username = user.Username,
                Score = user.Score,
                SolvedTasks = solvedCount,
                IsCurrentUser = Session.CurrentUser?.Id == user.Id
            });
        }
    }

    [RelayCommand]
    public void BackToDashboard()
    {
        _mainViewModel?.SwitchToDashboardCommand.Execute(null);
    }
}

// Модель для записи в лидерборде
public partial class LeaderboardEntry : ObservableObject
{
    [ObservableProperty] private int _position;
    [ObservableProperty] private string _username = "";
    [ObservableProperty] private int _score;
    [ObservableProperty] private int _solvedTasks;
    [ObservableProperty] private bool _isCurrentUser;
    
    public string PositionColor => Position switch
    {
        1 => "#f9e2af",  // Золотой
        2 => "#a6adc8",  // Серебряный
        3 => "#fab387",  // Бронзовый
        _ => "#585b70"   // Обычный
    };
}
