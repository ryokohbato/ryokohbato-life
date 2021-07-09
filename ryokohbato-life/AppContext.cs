using System.Net.Http;

namespace ryokohbato_life
{
  public class AppContext
  {
    public static HttpClient Client { get; private set; }

    AppContext()
    {
      Client = new HttpClient();
    }

    public static void InitializeClient()
    {
      Client = new HttpClient();
    }
  }
}
