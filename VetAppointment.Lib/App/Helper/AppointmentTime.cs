using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetAppointment.Lib.App.Helper
{
    public class AppointmentTime
    {
        public static List<TimeSpan> AvailableTimes()
        {
            List<TimeSpan> timeSpans = new List<TimeSpan>();
            for (int i = 9; i < 18; i++)
            {
                timeSpans.Add(new TimeSpan(i, 0, 0));
                timeSpans.Add(new TimeSpan(i, 30, 0));
            }
            return timeSpans;
        }

    }
}
