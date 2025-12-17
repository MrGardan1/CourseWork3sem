using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtfApp.Data;

namespace CtfApp.ViewModels;

public partial class ProfileViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    [ObservableProperty] private string _username = "";
    [ObservableProperty] private int _totalScore = 0;
    [ObservableProperty] private int _solvedTasks = 0;
    [ObservableProperty] private int _totalTasks = 0;
    [ObservableProperty] private double _completionPercent = 0;
    [ObservableProperty] private int _leaderboardPosition = 0;
    [ObservableProperty] private string _leaderboardText = "Not ranked";
    [ObservableProperty] private bool _showRank = true;


    public ObservableCollection<TaskItemViewModel> SolvedTasksList { get; } = new();

    public ProfileViewModel() {}

    public ProfileViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public void LoadProfileData()
    {
        if (Session.CurrentUser == null)
            return;

        Username = Session.CurrentUser.Username;
        TotalScore = Session.CurrentUser.Score;

        using var db = new AppDbContext();
        
        // Получаем только решенные задачи пользователя
        var solvedTaskIds = db.UserTasks
            .Where(ut => ut.UserId == Session.CurrentUser.Id)
            .Select(ut => ut.TaskId)
            .ToList();
        
        var solvedTasks = db.Tasks
            .Where(t => solvedTaskIds.Contains(t.Id))
            .ToList();
        
        // Общее количество задач
        TotalTasks = db.Tasks.Count();
        
        // Количество решенных
        SolvedTasks = solvedTasks.Count;
        
        // Процент выполнения
        CompletionPercent = TotalTasks > 0 ? (double)SolvedTasks / TotalTasks * 100 : 0;
        
        // Получаем позицию в лидерборде
        var allUsers = db.Users
            .Where(u => !u.IsAdmin)
            .OrderByDescending(u => u.Score)
            .ThenBy(u => u.Username)
            .Select(u => u.Id)
            .ToList();
        
        LeaderboardPosition = allUsers.IndexOf(Session.CurrentUser.Id) + 1;
        
        if (Session.CurrentUser.IsAdmin)
        {
            LeaderboardText = "#ADMIN";
            ShowRank = false;
        }
        else if (LeaderboardPosition > 0)
        {
            LeaderboardText = $"#{LeaderboardPosition} of {allUsers.Count}";
            ShowRank = true;
        }
        else
        {
            LeaderboardText = "Not ranked";
            ShowRank = true;
        }
        
        // Список решенных задач
        SolvedTasksList.Clear();
        foreach (var task in solvedTasks)
        {
            if (_mainViewModel != null)
            {
                SolvedTasksList.Add(new TaskItemViewModel(task, _mainViewModel));
            }
        }
    }

    [RelayCommand]
    public void BackToDashboard()
    {
        _mainViewModel?.SwitchToDashboardCommand.Execute(null);
    }

    [RelayCommand]
    public void Logout()
    {
        _mainViewModel?.SwitchToLoginCommand.Execute(null);
    }
}
