﻿using System;

namespace CmsData
{
    public partial class OrgSchedule
    {
        public override string ToString()
        {
            var dayOfWeek = Enum.GetName(typeof(DayOfWeek), SchedDay.Value);
            var name = $"{dayOfWeek} {SchedTime.Value:H:mm tt}";

            return name;
        }

        public void Update(OrgSchedule schedule)
        {
            AttendCreditId = schedule.AttendCreditId;
            Id = schedule.Id;
            MeetingTime = schedule.MeetingTime;
            SchedDay = schedule.SchedDay;
            SchedTime = schedule.SchedTime;
        }
    }
}
