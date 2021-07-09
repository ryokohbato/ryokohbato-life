using ryokohbato_life.Secret;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ryokohbato_life
{
  class Gyazo
  {
    /// <summary>
    /// Gyazoにファイルをアップロードする。アップロードされた画像のURLを返す。
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public async Task<string> Post(string filePath)
    {
      MultipartFormDataContent content = new MultipartFormDataContent();

      using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
      {

        StreamContent streamContent = new StreamContent(fs);
        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
          Name = "imagedata",
          FileName = filePath,
        };
        content.Add(streamContent);

        StringContent descriptionContent = new StringContent(Path.GetFileName(filePath));
        descriptionContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
          Name = "desc",
        };
        content.Add(descriptionContent);

        AppContext.InitializeClient();
        HttpResponseMessage response = await AppContext.Client.PostAsync($"https://upload.gyazo.com/api/upload?access_token={Tokens.Gyazo.AccessToken}", content);

        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody)["url"];
      }
    }
  }
}
