using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CtfApp.Data;
using Avalonia.Media;

namespace CtfApp.ViewModels;

public partial class TaskItemViewModel : ObservableObject
{
    private readonly CtfTask _task;
    private readonly MainWindowViewModel _mainVm;

    [ObservableProperty] private string _userInput = "";
    [ObservableProperty] private bool _isSolved = false;
    [ObservableProperty] private IBrush _buttonColor = SolidColorBrush.Parse("#a6e3a1");    [ObservableProperty] private string _buttonText = "SUBMIT";
    [ObservableProperty] private string _placeholderText = "Enter flag...";

    public string Title => _task.Title;
    public string Description => _task.Description;
    public int Points => _task.Points;

    public TaskItemViewModel(CtfTask task, MainWindowViewModel mainVm)
    {
        _task = task;
        _mainVm = mainVm;
    }

    [RelayCommand]
    private void Submit()
    {
        if (IsSolved) return;
        if (string.IsNullOrWhiteSpace(UserInput)) return;

        if (UserInput == _task.Flag)
        {
            IsSolved = true;
            ButtonText = "SOLVED ✓";
            ButtonColor = SolidColorBrush.Parse("#f9e2af");            UserInput = _task.Flag;
            _mainVm.AddScore(Points);
        }
        else
        {
            UserInput = "";
            PlaceholderText = "❌ Wrong Flag! Try again...";
        }
    }
}
