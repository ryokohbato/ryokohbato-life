using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ryokohbato_life
{
  public partial class MainViewModel : BindableBase
  {
    public const string UnknownExeIconURL = "https://i.gyazo.com/84ae907ec3b9b42d38edd85d7b81e7bd.png";

    Timer timer;
    Gyazo gyazo;
    Slack slack;

    private List<string> _registeredApplicationsName__HasMainWindow { get; set; }
    private List<string> _registeredApplicationsName__IgnoreMainWindow { get; set; }
    private List<string> _workingRegisteredApplicationsName__HasMainWindow { get; set; }
    private List<string> _workingRegisteredApplicationsName__IgnoreMainWindow { get; set; }
    private List<string> _workingRegisteredApplicationsName { get; set; }

    // アプリケーションの登録を実行
    public void ExecuteRegisterCommand(object parameter)
    {
      Process selectedProcess = AllProcesses.Where(x => x.ProcessName == (string)parameter).ToList()[0];

      RegisteredApplication item = new RegisteredApplication
      {
        ProcessName = selectedProcess.ProcessName,
        DisplayName = this.SelectedProcessDisplayName,
        HasMainWindow = !this.IsDisplayingAllProcesses,
        HasPosted = false,
        LaunchTime = DateTime.Now,
      };

      try
      {
        // 登録するプロセスの実行ファイルから、アイコン画像を取得し、一時的にファイルに保存する。
        List<char> InvalidCharacters = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).ToList();

        string fileName = selectedProcess.ProcessName;
        foreach (char c in InvalidCharacters)
        {
          fileName.Replace(c, '_');
        }

        string distDir = $"{Environment.GetEnvironmentVariable("temp").TrimEnd('\\')}\\ryokohbato-life";

        string distPath = $"{distDir}\\{fileName}.png";

        if (!Directory.Exists(distDir))
        {
          Directory.CreateDirectory(distDir);
        }

        Icon.ExtractAssociatedIcon(
          selectedProcess.MainModule.FileName).ToBitmap().Save(distPath,
          System.Drawing.Imaging.ImageFormat.Png
        );

        // Gyazoにアップロード
        var gyazoUploadTask = Task.Run(() =>
        {
          return gyazo.Post(distPath);
        });

        string gyazoUrl = gyazoUploadTask.Result;
        item.IconUrl = gyazoUrl;

        // 一時保存したアイコン画像を消去
        File.Delete(distPath);
      }
      catch (Exception)
      {
        Debug.WriteLine("out");
        item.IconUrl = UnknownExeIconURL;
      }

      RegisteredApplications.Add(item);
      SetProcesses();

      this.SelectedProcessDisplayName = string.Empty;
    }

    public bool CanExecuteRegisterCommand()
    {
      return this.SelectedProcessName != null && !string.IsNullOrEmpty(this.SelectedProcessDisplayName);
    }

    // 登録されたアプリケーションの登録を削除
    public void ExecuteDeleteCommand(object parameter)
    {
      RegisteredApplication deleteTarget = RegisteredApplications.Where(x => x.ProcessName == (string)parameter).ToList()[0];
      // 対象のアプリケーションが起動中の場合は、アプリケーションの終了メッセージを送信する。
      if (deleteTarget.HasPosted)
      {
        _ = Task.Run(() =>
        {
          PostFinishMessageAsync(deleteTarget);
        });
        deleteTarget.HasPosted = false;
      }
      RegisteredApplications.RemoveAt(RegisteredApplications.IndexOf(deleteTarget));
      SetProcesses();
    }

    public bool CanExecuteDeleteCommand() => true;

    private void StartWatching(object _)
    {
      // 3秒間隔で実行
      timer.Interval = 3000;
      timer.Elapsed += CheckEvent;
      timer.AutoReset = true;

      timer.Enabled = true;
    }

    private bool CanStartWatching() => true;

    private void CheckEvent(object source, ElapsedEventArgs e)
    {
      // 起動プロセスの一覧にも定期的に更新をかける。
      SetProcesses();

      _registeredApplicationsName__HasMainWindow
        = RegisteredApplications
          .Where(x => x.HasMainWindow)
          .Select(x => x.ProcessName)
          .ToList();

      _registeredApplicationsName__IgnoreMainWindow
        = RegisteredApplications
          .Where(x => !x.HasMainWindow)
          .Select(x => x.ProcessName)
          .ToList();

      _workingRegisteredApplicationsName__HasMainWindow
        = this.AllProcesses
          .Where(x => x.MainWindowHandle.ToInt32() != 0)
          .Select(x => x.ProcessName)
          .Intersect(_registeredApplicationsName__HasMainWindow)
          .ToList();

      _workingRegisteredApplicationsName__IgnoreMainWindow
        = this.AllProcesses
          .Select(x => x.ProcessName)
          .Intersect(_registeredApplicationsName__IgnoreMainWindow)
          .ToList();

      _workingRegisteredApplicationsName
        = _workingRegisteredApplicationsName__HasMainWindow
          .Union(_workingRegisteredApplicationsName__IgnoreMainWindow)
          .ToList();

      foreach (var app in RegisteredApplications)
      {
        if (app.HasPosted)
        {
          if (!_workingRegisteredApplicationsName.Contains(app.ProcessName))
          {
            _ = Task.Run(() =>
            {
              PostFinishMessageAsync(app);
            });
            app.HasPosted = false;
          }
        }
        else
        {
          if (_workingRegisteredApplicationsName.Contains(app.ProcessName))
          {
            _ = Task.Run(() =>
            {
              PostLaunchMessageAsync(app);
            });
            app.HasPosted = true;
          }
        }
      }
    }

    private Task PostLaunchMessageAsync(RegisteredApplication application)
    {
      if (application.IconUrl != null)
      {
        return slack.PostJsonMessageAsync(@"
        {
          'channel': 'ryokohbato-dev-log-zatsu',
          'blocks': [
            {
              'type': 'section',
              'text': {
                'type': 'mrkdwn',
                'text': '*ヘッダー*\n" + application.DisplayName + @"を起動しました。\n端末名: " + this.Text__MachineName + @"'
              },
              'accessory': {
                'type': 'image',
                'image_url': '" + application.IconUrl + @"',
                'alt_text': '" + application.DisplayName + @"'
              }
            },  
          ],
        }");
      }

      return slack.PostJsonMessageAsync(@"
        {
          'channel': 'ryokohbato-dev-log-zatsu',
          'blocks': [
            {
              'type': 'section',
              'text': {
                'type': 'mrkdwn',
                'text': '*ヘッダー*\n" + application.DisplayName + @"を起動しました。\n端末名: " + this.Text__MachineName + @"'
              },
            },  
          ],
        }");
    }

    private Task PostFinishMessageAsync(RegisteredApplication application)
    {
      if (application.IconUrl != null)
      {
        return slack.PostJsonMessageAsync(@"
        {
          'channel': 'ryokohbato-dev-log-zatsu',
          'blocks': [
            {
              'type': 'section',
              'text': {
                'type': 'mrkdwn',
                'text': '*ヘッダー*\n" + application.DisplayName + @"を終了しました。\n端末名: " + this.Text__MachineName + @"\n起動時間: "
                + GetCustomTimespan(DateTime.Now.Subtract(application.LaunchTime)) + @"'
              },
              'accessory': {
                'type': 'image',
                'image_url': '" + application.IconUrl + @"',
                'alt_text': '" + application.DisplayName + @"'
              }
            },  
          ],
        }");
      }

      return slack.PostJsonMessageAsync(@"
        {
          'channel': 'ryokohbato-dev-log-zatsu',
          'blocks': [
            {
              'type': 'section',
              'text': {
                'type': 'mrkdwn',
                'text': '*ヘッダー*\n" + application.DisplayName + @"を終了しました。\n端末名: " + this.Text__MachineName + @"'
              },
            },  
          ],
        }");
    }

    private string GetCustomTimespan(TimeSpan timespan)
    {
      int h = timespan.Days * 24 + timespan.Hours;
      int m = timespan.Minutes;
      int s = timespan.Seconds;

      if (h != 0)
        return $"{h}時間{m}分{s}秒";
      else if (m != 0)
        return $"{m}分{s}秒";
      else
        return $"{s}秒";
    }
  }
}
