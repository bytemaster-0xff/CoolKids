using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.Models
{
    [DataContract(Name = "time")]
    public class Time
    {
        public Time(String time)
        {
            var timeParts = time.Split(':');
            Minutes = Convert.ToInt32(timeParts[0]) * 60 + Convert.ToInt32(timeParts[1]);
        }

        public Time(DateTime dateTime)
        {
            Minutes = dateTime.Hour * 60 + dateTime.Minute;
        }

        [DataMember(Name = "minutes")]
        public int Minutes { get; set; }


        [IgnoreDataMember]

        public String Value
        {
            get
            {
                return String.Format("{0:00}:{1:00}", Minutes / 60, Minutes % 60);
            }
        }

        public bool IsAfter(Time time)
        {
            return Minutes > time.Minutes;
        }

        public bool IsBefore(Time time)
        {
            return Minutes < time.Minutes;
        }

        public bool IsBeforeOrEquals(Time time)
        {
            return Minutes <= time.Minutes;
        }

        public bool IsSame(Time time)
        {
            return Minutes == time.Minutes;
        }

        public TimeSpan ToTimeSpan()
        {
            return TimeSpan.FromMinutes(Minutes);
        }

        [IgnoreDataMember]
        public String Display
        {
            get
            {
                int hours = Minutes / 60;
                int minutes = Minutes % 60;

                if (hours == 0)
                    return String.Format(String.Format("12:{0:00} AM", minutes));

                if (hours == 12)
                    return String.Format(String.Format("12:{0:00} PM", minutes));

                if (hours < 12)
                    return String.Format(String.Format("{0}:{1:00} AM", hours, minutes));

                return String.Format(String.Format("{0}:{1:00} PM", hours - 12, minutes));
            }
        }
        public override int GetHashCode()
        {
            return Minutes;
        }
    }
}
