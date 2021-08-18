using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ryokohbato_life.ryokohbato_scheduler;

namespace ryokohbato_life
{
  public class Program
  {
    private static Slack _slack = new Slack();
    public static async Task Main(string[] args)
    {
      await PostScheduleTask(SchedulerFormatter.Execute(Scheduler.GetSchedule(5)));
    }

    // 引数として与えられたリストを改行区切りでSlackに投稿
    private async static Task<bool> PostScheduleTask (List<string> responses)
    {
      return await _slack.PostJsonMessageAsync(@"
      {
        'channel': 'ryokohbato-dev-log-zatsu',
        'blocks': [
          {
            'type': 'section',
            'text': {
              'type': 'mrkdwn',
              'text': '*直近の予定*\n" + string.Join('\n', responses) + @"'
            },
          },  
        ],
      }", SecretData.Slack.BotUserOAuthToken);
    }
  }
}
