using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtfApp.Data;

namespace CtfApp.ViewModels;

public partial class TaskItemViewModel : ObservableObject
{
    private readonly CtfTask _task;
    private readonly MainWindowViewModel _mainVm;

    [ObservableProperty] private string _userInput = "";
    [ObservableProperty] private bool _isSolved = false;
    [ObservableProperty] private string _statusMessage = "";
    [ObservableProperty] private string _statusColor = "#585b70";
    [ObservableProperty] private int _attemptCount = 0;
    [ObservableProperty] private bool _showHintButton = false;
    [ObservableProperty] private bool _hintRevealed = false;
    [ObservableProperty] private bool _showSolution = false;

    public int Id => _task.Id;
    public string Title => _task.Title;
    public string Description => _task.Description;
    public int Points => _task.Points;
    public string Category => _task.Category;
    public string Difficulty => _task.Difficulty;
    public string Hint => _task.Hint;
    public string Solution => _task.Solution;

    public string DifficultyColor => _task.Difficulty switch
    {
        "Easy" => "#a6e3a1",
        "Medium" => "#f9e2af",
        "Hard" => "#f38ba8",
        _ => "#89b4fa"
    };

    public string PlaceholderText => IsSolved ? "Already solved!" : "Enter flag...";
    public string ButtonText => IsSolved ? "SOLVED ✓" : "SUBMIT";
    public string ButtonColor => IsSolved ? "#a6e3a1" : "#89b4fa";

    public TaskItemViewModel(CtfTask task, MainWindowViewModel mainVm)
    {
        _task = task;
        _mainVm = mainVm;

        // Проверяем решена ли задача
        if (Session.CurrentUser != null)
        {
            using var db = new AppDbContext();
            IsSolved = db.UserTasks.Any(ut => 
                ut.UserId == Session.CurrentUser.Id && 
                ut.TaskId == _task.Id);

            if (IsSolved)
            {
                StatusMessage = "Task completed!";
                StatusColor = "#a6e3a1";
                ShowSolution = true;  // Показываем решение если уже решено
            }
        }
    }

    [RelayCommand]
    private void Submit()
    {
        if (IsSolved || Session.CurrentUser == null)
            return;

        if (string.IsNullOrWhiteSpace(UserInput))
        {
            StatusMessage = "Please enter a flag";
            StatusColor = "#f9e2af";
            return;
        }

        if (UserInput.Trim().ToLower() == _task.Flag.ToLower())
        {
            // Правильный ответ
            StatusMessage = $"Correct! +{_task.Points} points";
            StatusColor = "#a6e3a1";
            IsSolved = true;
            ShowSolution = true;
            ShowHintButton = false;

            // Сохраняем в БД
            using var db = new AppDbContext();
            db.UserTasks.Add(new UserTask 
            { 
                UserId = Session.CurrentUser.Id, 
                TaskId = _task.Id 
            });
            db.SaveChanges();

            // Обновляем очки
            _mainVm.AddScore(_task.Points);
            
            // Обновляем профиль и лидерборд
            _mainVm.ProfileViewModel?.LoadProfileData();
            _mainVm.LeaderboardViewModel?.LoadLeaderboard();
        }
        else {
            // Неправильный ответ
            if (AttemptCount < 3)
            {
                AttemptCount++;
                StatusMessage = $"Wrong flag! Attempt {AttemptCount}/3";
                StatusColor = "#f38ba8";
            }
            else
            {
                // После 3 попыток больше не увеличиваем счётчик
                StatusMessage = "Wrong flag! Use the hint button.";
                StatusColor = "#f38ba8";
            }
            
            UserInput = "";

            // После 3 попыток показываем кнопку подсказки
            if (AttemptCount >= 3)
            {
                ShowHintButton = true;
            }
        }
    }

    [RelayCommand]
    private void ShowHint()
    {
        HintRevealed = true;
        StatusMessage = "Hint revealed below";
        StatusColor = "#f9e2af";
    }
}
