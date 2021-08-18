using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ryokohbato_life.ryokohbato_scheduler
{
  public class Scheduler
  {
    private static string[] _scopes = { SheetsService.Scope.Spreadsheets };
    private static string _applicationName = "ryokohbato-scheduler";

    // Google Sheetsに登録された予定から、指定された日数先までのものを出力
    public static List<Schedule> GetSchedule(int days)
    {
      return GetSchedule(days, DateTime.Now, $"{DateTime.Now.Year}-{DateTime.Now.Month.ToString().PadLeft(2, '0')}", "secrets/client_secret.json");
    }

    public static List<Schedule> GetSchedule(int days, DateTime date, string sheetName, string secretFilePath)
    {
      UserCredential userCredential;

      // プログラムのディレクトリ以下にsecrets/client_secret.jsonファイルが必要
      using (var stream = new FileStream(secretFilePath, FileMode.Open, FileAccess.Read))
      {
        string credentialFilePath = AppDomain.CurrentDomain.BaseDirectory;
        credentialFilePath = Path.Combine(credentialFilePath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

        userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
          GoogleClientSecrets.FromStream(stream).Secrets,
          _scopes,
          "user",
          CancellationToken.None,
          new FileDataStore(credentialFilePath, true)).Result;
      }

      var service = new SheetsService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = userCredential,
        ApplicationName = _applicationName,
      });

      var request = service.Spreadsheets.Values.Get(SecretData.Spreadsheet.SpreadsheetId, $"{sheetName}!A2:E20");
      request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMATTEDVALUE;
      var results = request.Execute();

      List<Schedule> schedules = new List<Schedule>();
      
      foreach(var result in results.Values)
      {
        DateTime startDateTime = new DateTime(1900, 1, 1, 0, 0, 0);
        DateTime endDateTime = new DateTime(1900, 1, 1, 0, 0, 0);

        // タイトルで予定の有無を判別
        if (result[0].ToString() == string.Empty)
        {
          continue;
        }

        var dates = result[2].ToString().Split('-');
        var times = result[3].ToString().Split('-');

        if (dates.Length == 1)
        {
          string[] startDate__Splitted = dates[0].Split('/');

          if (startDate__Splitted.Length != 2)
          {
            continue;
          }

          if (!int.TryParse(startDate__Splitted[0], out int startDate__Month))
          {
            continue;
          }

          if (!int.TryParse(startDate__Splitted[1], out int startDate__Day))
          {
            continue;
          }

          string[] startTime__Splitted = times[0].Split(':');

          if (!int.TryParse(startTime__Splitted[0], out int startTime__Hour))
          {
            continue;
          }

          if (!int.TryParse(startTime__Splitted[1], out int startTime__Minute))
          {
            continue;
          }

          startDateTime = new DateTime(date.Year, startDate__Month, startDate__Day, startTime__Hour, startTime__Minute, 0);

          endDateTime = startDateTime;
        }
        else if (dates.Length == 2)
        {
          string[] startDate__Splitted = dates[0].Split('/');

          if (startDate__Splitted.Length != 2)
          {
            continue;
          }

          if (!int.TryParse(startDate__Splitted[0], out int startDate__Month))
          {
            continue;
          }

          if (!int.TryParse(startDate__Splitted[1], out int startDate__Day))
          {
            continue;
          }

          string[] startTime__Splitted = times[0].Split(':');

          if (!int.TryParse(startTime__Splitted[0], out int startTime__Hour))
          {
            continue;
          }

          if (!int.TryParse(startTime__Splitted[1], out int startTime__Minute))
          {
            continue;
          }

          startDateTime = new DateTime(date.Year, startDate__Month, startDate__Day, startTime__Hour, startTime__Minute, 0);

          string[] endDate__Splitted = dates[1].Split('/');

          if (endDate__Splitted.Length != 2)
          {
            continue;
          }

          if (!int.TryParse(endDate__Splitted[0], out int endDate__Month))
          {
            continue;
          }

          if (!int.TryParse(endDate__Splitted[1], out int endDate__Day))
          {
            continue;
          }

          string[] endTime__Splitted = times[1].Split(':');

          if (!int.TryParse(endTime__Splitted[0], out int endTime__Hour))
          {
            continue;
          }

          if (!int.TryParse(endTime__Splitted[1], out int endTime__Minute))
          {
            continue;
          }

          if (startDate__Month <= endDate__Month)
          {
            endDateTime = new DateTime(date.Year, endDate__Month, endDate__Day, endTime__Hour, endTime__Minute, 0);
          }
          else
          {
            endDateTime = new DateTime(date.Year + 1, endDate__Month, endDate__Day, endTime__Hour, endTime__Minute, 0);
          }
        }

        if (DateTime.Compare(endDateTime, date) < 0)
        {
          continue;
        }

        DateTime lastTimeDate = date.AddDays(days);
        lastTimeDate = new DateTime(lastTimeDate.Year, lastTimeDate.Month, lastTimeDate.Day, 23, 59, 59);

        if (DateTime.Compare(startDateTime, lastTimeDate) > 0)
        {
          continue;
        }

        schedules.Add(new Schedule()
        {
          Title = result[0].ToString(),
          IsVisible = result[1].ToString() == "TRUE" ? true : false,
          Dates = dates,
          Times = times,
          // 予定の詳細は省略可能
          Detail = result.Count == 5 ? result[4].ToString() : string.Empty,
        });
      }

      return schedules;
    }
  }
}
