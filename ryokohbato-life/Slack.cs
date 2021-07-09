using ryokohbato_life.Secret;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ryokohbato_life
{
  class Slack
  {
    /// <summary>
    /// 指定したメッセージを送信する。レスポンスの成功ステータスを返す。
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<bool> PostMessageAsync(string message, string target)
    {
      Dictionary<string, string> header = new Dictionary<string, string>()
      {
        { "channel", target },
        { "text", message },
      };

      return await PostAsync(JsonSerializer.Serialize(header));
    }

    /// <summary>
    /// 指定したJsonで送信する。レスポンスの成功ステータスを返す。
    /// </summary>
    /// <param name="jsonMessage"></param>
    /// <returns></returns>
    public async Task<bool> PostJsonMessageAsync(string jsonMessage)
    {
      return await PostAsync(jsonMessage);
    }

    private async Task<bool> PostAsync(string header)
    {
      StringContent headerContent
        = new StringContent(header, Encoding.UTF8, "application/json");

      AppContext.InitializeClient();
      AppContext.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.Slack.BotUserOAuthToken);
      HttpResponseMessage response = await AppContext.Client.PostAsync("https://slack.com/api/chat.postMessage", headerContent);

      response.EnsureSuccessStatusCode();

      string responseBody = await response.Content.ReadAsStringAsync();

      Dictionary<string, object> responseBody__deserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

      if (responseBody__deserialized["ok"].ToString().ToLower() == "true")
      {
        return true;
      }
      else if (responseBody__deserialized["ok"].ToString().ToLower() == "false")
      {
        return false;
      }

      throw new Exception();
    }
  }
}
