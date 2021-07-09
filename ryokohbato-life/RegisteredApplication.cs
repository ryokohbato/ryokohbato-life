using System;

namespace ryokohbato_life
{
  public class RegisteredApplication
  {
    public string ProcessName { get; set; }
    public string DisplayName { get; set; }
    public string IconUrl { get; set; }
    public bool HasMainWindow { get; set; }
    public bool HasPosted { get; set; }
    public DateTime LaunchTime { get; set; }
  }
}
