using System.Windows;

namespace ryokohbato_life
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void StartWatching(object sender, RoutedEventArgs e)
    {
      (this.DataContext as MainViewModel).WatchCommand.Execute(null);
    }
  }
}
