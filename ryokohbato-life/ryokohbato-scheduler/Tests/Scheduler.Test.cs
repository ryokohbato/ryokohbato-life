using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ryokohbato_life.ryokohbato_scheduler;

namespace Tests
{
  public class Tests
  {
    List<Schedule> schedules;

    bool SchedulesAreEqual(Schedule schedule1, Schedule schedule2)
    {
      return
        schedule1.Title == schedule2.Title
        && schedule1.IsVisible == schedule2.IsVisible
        && schedule1.Dates.SequenceEqual(schedule2.Dates)
        && schedule1.Times.SequenceEqual(schedule2.Times)
        && schedule1.Detail == schedule2.Detail;
    }

    [SetUp]
    public void Setup()
    {
      schedules = Scheduler.GetSchedule(1, System.DateTime.Now, "Tests", "../../../secrets/client_secret.json");
    }

    [Test]
    public void Test1()
    {
      Assert.IsTrue(SchedulesAreEqual(schedules[0], new Schedule()
      {
        Title = "予定1",
        IsVisible = true,
        Dates = new string[] {"1/1"},
        Times = new string[] {"8:00", "12:00"},
        Detail = string.Empty,
      }));
    }

    [Test]
    public void Test2()
    {
      Assert.IsTrue(SchedulesAreEqual(schedules[1], new Schedule()
      {
        Title = "予定2",
        IsVisible = false,
        Dates = new string[] {"1/3", "1/5"},
        Times = new string[] {"13:00", "19:00"},
        Detail = "3日間のイベント"
      }));
    }
  }
}
