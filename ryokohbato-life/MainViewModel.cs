using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;

namespace ryokohbato_life
{
  public partial class MainViewModel : BindableBase
  {
    public DelegateCommand RegisterCommand { get; private set; }
    public DelegateCommand DeleteCommand { get; private set; }
    public DelegateCommand WatchCommand { get; private set; }

    private ThicknessConverter _thicknessConverter = new ThicknessConverter();

    public MainViewModel()
    {
      this.FontSize__Title = 18;
      this.Body__Margin = (Thickness)_thicknessConverter.ConvertFromString("16, 8");
      this.ContentsWrapper__Margin = (Thickness)_thicknessConverter.ConvertFromString("8");
      this.ItemWrapper__Margin = (Thickness)_thicknessConverter.ConvertFromString("8, 8");
      this.Item__Margin = (Thickness)_thicknessConverter.ConvertFromString("0, 4");
      this.Item__Margin__Large = (Thickness)_thicknessConverter.ConvertFromString("16, 8, 0, 8");
      this.ListedItemWrapper__Margin = (Thickness)_thicknessConverter.ConvertFromString("0, 0, 0, 8");
      this.ListedItem__Margin = (Thickness)_thicknessConverter.ConvertFromString("8, 0, 0, 0");
      this.RegisteredApplicationsItem__Size = 24;
      this.Caption__MainWindow = "設定";
      this.Title__RegisterApp = "アプリケーションの登録";
      this.Title__MachineName = "マシン名";
      this.Content__DisplayName = "アプリケーションの表示名";
      this.Content__RegisterButton = "登録";
      this.Content__DisplayAllProcesses = "全てのプロセスを表示";
      this.Content__Delete = "削除";
      this.Content__UseSystemSetting = "システム設定を使用";
      this.Text__MachineName = Environment.MachineName;
      this.IsUsingSystemSetting = true;

      RegisterCommand = new DelegateCommand(ExecuteRegisterCommand, CanExecuteRegisterCommand);
      DeleteCommand = new DelegateCommand(ExecuteDeleteCommand, CanExecuteDeleteCommand);
      WatchCommand = new DelegateCommand(StartWatching, CanStartWatching);

      RegisteredApplications = new ObservableCollection<RegisteredApplication>();
      SetProcesses();
     
      this.IsDisplayingAllProcesses = false;

      timer = new Timer();
      gyazo = new Gyazo();
      slack = new Slack();

      _registeredApplicationsName__HasMainWindow = new List<string>();
      _registeredApplicationsName__IgnoreMainWindow = new List<string>();
      _workingRegisteredApplicationsName__HasMainWindow = new List<string>();
      _workingRegisteredApplicationsName__IgnoreMainWindow = new List<string>();
      _workingRegisteredApplicationsName = new List<string>();
    }

    private int _fontSize__Title;
    public int FontSize__Title
    {
      get { return this._fontSize__Title; }
      set { this.SetProperty(ref this._fontSize__Title, value); }
    }

    private Thickness _contentsWrapper__Margin;
    public Thickness ContentsWrapper__Margin
    {
      get { return this._contentsWrapper__Margin; }
      set { this.SetProperty(ref this._contentsWrapper__Margin, value); }
    }

    private Thickness _body__Margin;
    public Thickness Body__Margin
    {
      get { return this._body__Margin; }
      set { this.SetProperty(ref this._body__Margin, value); }
    }

    private Thickness _itemWrapper__Margin;
    public Thickness ItemWrapper__Margin
    {
      get { return this._itemWrapper__Margin; }
      set { this.SetProperty(ref this._itemWrapper__Margin, value); }
    }

    private Thickness _item__Margin;
    public Thickness Item__Margin
    {
      get { return this._item__Margin; }
      set { this.SetProperty(ref this._item__Margin, value); }
    }

    private Thickness _item__Margin__Large;
    public Thickness Item__Margin__Large
    {
      get { return this._item__Margin__Large; }
      set { this.SetProperty(ref this._item__Margin__Large, value); }
    }

    private Thickness _listedItemWrapper__Margin;
    public Thickness ListedItemWrapper__Margin
    {
      get { return this._listedItemWrapper__Margin; }
      set { this.SetProperty(ref this._listedItemWrapper__Margin, value); }
    }

    private Thickness _listedItem__Margin;
    public Thickness ListedItem__Margin
    {
      get { return this._listedItem__Margin; }
      set { this.SetProperty(ref this._listedItem__Margin, value); }
    }

    private int _registeredApplicationsItem__Size;
    public int RegisteredApplicationsItem__Size
    {
      get { return this._registeredApplicationsItem__Size; }
      set { this.SetProperty(ref this._registeredApplicationsItem__Size, value); }
    }

    private string _caption__MainWindow;
    public string Caption__MainWindow
    {
      get { return this._caption__MainWindow; }
      set { this.SetProperty(ref this._caption__MainWindow, value); }
    }

    private string _title__RegisterApp;
    public string Title__RegisterApp
    {
      get { return this._title__RegisterApp; }
      set { this.SetProperty(ref this._title__RegisterApp, value); }
    }

    private string _title__MachineName;
    public string Title__MachineName
    {
      get { return this._title__MachineName; }
      set { this.SetProperty(ref this._title__MachineName, value); }
    }

    private string _content__DisplayName;
    public string Content__DisplayName
    {
      get { return this._content__DisplayName; }
      set { this.SetProperty(ref this._content__DisplayName, value); }
    }

    private string _content__RegisterButton;
    public string Content__RegisterButton
    {
      get { return this._content__RegisterButton; }
      set { this.SetProperty(ref this._content__RegisterButton, value); }
    }

    private string _content__DisplayAllProcesses;
    public string Content__DisplayAllProcesses
    {
      get { return _content__DisplayAllProcesses; }
      set { this.SetProperty(ref this._content__DisplayAllProcesses, value); }
    }

    private string _content__Delete;
    public string Content__Delete
    {
      get { return _content__Delete; }
      set { this.SetProperty(ref this._content__Delete, value); }
    }

    private string _content__UseSystemSetting;
    public string Content__UseSystemSetting
    {
      get { return _content__UseSystemSetting; }
      set { this.SetProperty(ref this._content__UseSystemSetting, value); }
    }

    private string _text__MachineName;
    public string Text__MachineName
    {
      get { return _text__MachineName; }
      set { this.SetProperty(ref this._text__MachineName, value); }
    }

    private bool _isUsingSystemSetting;
    public bool IsUsingSystemSetting
    {
      get { return _isUsingSystemSetting; }
      set {
        this.SetProperty(ref this._isUsingSystemSetting, value);
        if (value)
        {
          this.Text__MachineName = Environment.MachineName;
        }
      }
    }

    private ObservableCollection<string> _processesName;
    public ObservableCollection<string> ProcessesName
    {
      get { return this._processesName; }
      set { this.SetProperty(ref this._processesName, value); }
    }

    private ObservableCollection<Process> _allProcesses;
    public ObservableCollection<Process> AllProcesses
    {
      get { return this._allProcesses; }
      set { this.SetProperty(ref this._allProcesses, value); }
    }

    private void SetProcesses()
    {
      if (IsDisplayingAllProcesses)
      {
        ProcessesName = new ObservableCollection<string>(
          Process.GetProcesses()
            .Select(x => x.ProcessName)
            .Distinct()
            .Except(RegisteredApplications.Select(x => x.ProcessName).ToList())
          );
        AllProcesses = new ObservableCollection<Process>(Process.GetProcesses());
      }
      else
      {
        ProcessesName = new ObservableCollection<string>(
          Process.GetProcesses()
            .Where(x => x.MainWindowHandle.ToInt32() != 0)
            .Select(x => x.ProcessName)
            .Distinct()
            .Except(RegisteredApplications.Select(x => x.ProcessName).ToList())
        );
        AllProcesses = new ObservableCollection<Process>(Process.GetProcesses());
      }
    }

    private string _selectedProcessName;
    public string SelectedProcessName
    {
      get { return _selectedProcessName; }
      set
      {
        this.SetProperty(ref this._selectedProcessName, value);
        this.SelectedProcessDisplayName = this._selectedProcessName;
        this.RegisterCommand.RaiseCanExecuteChanged();
      }
    }

    private string _selectedProcessDisplayName;
    public string SelectedProcessDisplayName
    {
      get { return _selectedProcessDisplayName; }
      set
      {
        this.SetProperty(ref this._selectedProcessDisplayName, value);
        this.RegisterCommand.RaiseCanExecuteChanged();
      }
    }

    public ObservableCollection<RegisteredApplication> RegisteredApplications { get; set; }

    private bool _isDisplayingAllProcesses;
    public bool IsDisplayingAllProcesses
    {
      get { return this._isDisplayingAllProcesses; }
      set
      {
        this.SetProperty(ref this._isDisplayingAllProcesses, value);
        SetProcesses();
      }
    }
  }
}
