using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ryokohbato_life.ryokohbato_scheduler;

namespace Tests
{
  public class ScheduleFormatterTests
  {
    List<Schedule> schedules;

    [SetUp]
    public void Setup()
    {
      schedules = Scheduler.GetSchedule(1, System.DateTime.Now, "Tests", "../../../secrets/client_secret.json");
    }

    [Test]
    public void Test1()
    {
      Assert.AreEqual("1/3 13:00〜1/5 19:00 非公開の予定", SchedulerFormatter.Execute(schedules)[1]);
    }

    [Test]
    public void Test2()
    {
      Assert.AreEqual("1/7 15:00〜1/9 18:00 予定3", SchedulerFormatter.Execute(schedules)[2]);
    }

    [Test]
    public void Test3()
    {
      Assert.AreEqual("1/11 9:00〜12:00 予定4", SchedulerFormatter.Execute(schedules)[3]);
    }

    [Test]
    public void Test4()
    {
      Assert.AreEqual("1/13 9:00〜1/15 9:00 予定5", SchedulerFormatter.Execute(schedules)[4]);
    }

    [Test]
    public void Test5()
    {
      Assert.AreEqual("1/18 9:00 予定6", SchedulerFormatter.Execute(schedules)[5]);
    }
  }
}
