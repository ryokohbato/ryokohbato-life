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

    // Google Sheetsに登録された予定を出力
    public static List<string> GetSchedule()
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

      var request = service.Spreadsheets.Values.Get(SecretData.Spreadsheet.SpreadsheetId, "2021-08!A2:D20");
      request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMATTEDVALUE;
      var result = request.Execute();

      List<string> responses = new List<string>();
      
      foreach(var s in result.Values)
      {
        if (s[0].ToString() == string.Empty)
        {
          continue;
        }

        if (s[1].ToString() == "FALSE")
        {
          s[0] = "非公開の予定";
        }
        var dates = s[2].ToString().Split('-');
        var times = s[3].ToString().Split('-');

        if (dates.Length == 1)
        {
          if (times.Length == 1)
          {
            responses.Add($"{dates[0]} {times[0]}に{s[0]}があります。");
          }
          else if (times.Length == 2)
          {
            responses.Add($"{dates[0]} {times[0]}〜{times[1]}に{s[0]}があります。");
          }
        }
        else if (dates.Length == 2)
        {
          if (times.Length == 1)
          {
            responses.Add($"{dates[0]} {times[0]}〜{dates[1]} {times[0]}に{s[0]}があります。");
          }
          else if (times.Length == 2)
          {
            responses.Add($"{dates[0]} {times[0]}〜{dates[1]} {times[1]}に{s[0]}があります。");
          }
        }
      }

      return responses;
    }
  }
}
