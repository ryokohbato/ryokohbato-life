using System;
using System.Collections.Generic;
using System.Linq;

namespace ryokohbato_life.ryokohbato_scheduler
{
  public class SchedulerFormatter
  {
    public static List<string> Execute(List<Schedule> schedules)
    {
      return schedules.Select(x =>
      {
        if (!x.IsVisible)
        {
          x.Title = "非公開の予定";
        }

        if (x.Dates.Length == 1)
        {
          if (x.Times.Length == 1)
          {
            return $"{x.Dates[0]} {x.Times[0]} {x.Title}";
          }
          else if (x.Times.Length == 2)
          {
            return $"{x.Dates[0]} {x.Times[0]}〜{x.Times[1]} {x.Title}";
          }
        }
        else if (x.Dates.Length == 2)
        {
          if (x.Times.Length == 1)
          {
            return $"{x.Dates[0]} {x.Times[0]}〜{x.Dates[1]} {x.Times[0]} {x.Title}";
          }
          else if (x.Times.Length == 2)
          {
            return $"{x.Dates[0]} {x.Times[0]}〜{x.Dates[1]} {x.Times[1]} {x.Title}";
          }
        }

        return string.Empty;
      }).ToList();
    }
  }
}