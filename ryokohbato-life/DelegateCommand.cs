using System;
using System.Windows.Input;

namespace ryokohbato_life
{
  public class DelegateCommand : ICommand
  {
    private readonly Action<object> _execute;
    private readonly Func<bool> _canExecute;
    public event EventHandler CanExecuteChanged;

    public DelegateCommand(Action<object> execute, Func<bool> canExecute)
    {
      if (execute == null) return;
      _execute = execute;
      _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
      return _canExecute == null ? true : _canExecute();
    }

    public void Execute(object parameter)
    {
      _execute(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
      var handler = CanExecuteChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }
  }
}
