using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ryokohbato_life
{
  public class BindableBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
      if (Equals(field, value)) { return false; }
      field = value;
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); return true;
    }
  }
}
