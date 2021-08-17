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
    public static List<Schedule> GetSchedule(int days, DateTime date, string spreadsheetId)
    {
      UserCredential userCredential;

      // プログラムのディレクトリ以下にsecrets/client_secret.jsonファイルが必要
      using (var stream = new FileStream("secrets/client_secret.json", FileMode.Open, FileAccess.Read))
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

      var request = service.Spreadsheets.Values.Get(spreadsheetId, "2021-08!A2:E20");
      request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMATTEDVALUE;
      var results = request.Execute();

      List<Schedule> schedules = new List<Schedule>();
      
      foreach(var result in results.Values)
      {
        // タイトルで予定の有無を判別
        if (result[0].ToString() == string.Empty)
        {
          continue;
        }

        var dates = result[2].ToString().Split('-');
        var times = result[3].ToString().Split('-');
        
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
