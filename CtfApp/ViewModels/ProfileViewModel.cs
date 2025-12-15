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

    public ObservableCollection<TaskItemViewModel> SolvedTasksList { get; } = new();

    public ProfileViewModel()
    {
        // Конструктор без параметров для дизайнера
    }

    public ProfileViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LoadProfileData();
    }

    public void LoadProfileData()
    {
        if (Session.CurrentUser == null)
            return;

        Username = Session.CurrentUser.Username;
        TotalScore = Session.CurrentUser.Score;

        using var db = new AppDbContext();
        
        // Получаем ТОЛЬКО решенные задачи пользователя
        var solvedTaskIds = db.UserTasks
            .Where(ut => ut.UserId == Session.CurrentUser.Id)
            .Select(ut => ut.TaskId)
            .ToList();
        
        var solvedTasks = db.Tasks
            .Where(t => solvedTaskIds.Contains(t.Id))
            .ToList();
        
        SolvedTasks = solvedTasks.Count;
        
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
}
