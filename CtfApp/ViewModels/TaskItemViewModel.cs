using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtfApp.Data;
using Avalonia.Media;
using System.Linq;

namespace CtfApp.ViewModels;

public partial class TaskItemViewModel : ObservableObject
{
    private readonly CtfTask _task;
    private readonly MainWindowViewModel _mainVm;

    [ObservableProperty] private string _userInput = "";
    [ObservableProperty] private bool _isSolved = false;
    [ObservableProperty] private IBrush _buttonColor = SolidColorBrush.Parse("#a6e3a1");
    [ObservableProperty] private string _buttonText = "SUBMIT";
    [ObservableProperty] private string _placeholderText = "Enter flag...";

    public string Title => _task.Title;
    public string Description => _task.Description;
    public int Points => _task.Points;

    public TaskItemViewModel(CtfTask task, MainWindowViewModel mainVm)
    {
        _task = task;
        _mainVm = mainVm;
        
        // ⬇️ ДОБАВИЛИ: Проверяем решена ли задача при создании
        CheckIfSolved();
    }

    private void CheckIfSolved()
    {
        if (Session.CurrentUser == null) return;

        using var db = new AppDbContext();
        var solved = db.UserTasks.Any(ut => 
            ut.UserId == Session.CurrentUser.Id && 
            ut.TaskId == _task.Id);

        if (solved)
        {
            IsSolved = true;
            ButtonText = "SOLVED ✓";
            ButtonColor = SolidColorBrush.Parse("#f9e2af");
            PlaceholderText = "Task completed!";
        }
    }

    [RelayCommand]
    private void Submit()
    {
        if (IsSolved) return;
        if (string.IsNullOrWhiteSpace(UserInput)) return;

        if (UserInput == _task.Flag)
        {
            // ✅ Правильный флаг
            IsSolved = true;
            ButtonText = "SOLVED ✓";
            ButtonColor = SolidColorBrush.Parse("#f9e2af");
            UserInput = _task.Flag;
            
            // ⬇️ ДОБАВИЛИ: Добавляем очки
            _mainVm.AddScore(Points);

            // ⬇️ ДОБАВИЛИ: Сохраняем в БД
            if (Session.CurrentUser != null)
            {
                using var db = new AppDbContext();
                
                // Проверяем что ещё не сохранено (на всякий случай)
                var alreadySolved = db.UserTasks.Any(ut => 
                    ut.UserId == Session.CurrentUser.Id && 
                    ut.TaskId == _task.Id);

                if (!alreadySolved)
                {
                    var userTask = new UserTask
                    {
                        UserId = Session.CurrentUser.Id,
                        TaskId = _task.Id
                    };
                    db.UserTasks.Add(userTask);
                    db.SaveChanges();
                }
            }

            // ⬇️ ДОБАВИЛИ: Обновляем профиль
            _mainVm.ProfileViewModel?.LoadProfileData();
        }
        else
        {
            // ❌ Неправильный флаг
            UserInput = "";
            PlaceholderText = "❌ Wrong Flag! Try again...";
        }
    }
}
