using System;
using System.Collections.Generic;
using System.Linq;
using Magnum;
using MonitorWang.Core.Interfaces;

namespace MonitorWang.Core.Schedulers
{
    public class TwentyFourSevenTimerConfig
    {
        public string Sunday { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Weekdays { get; set; }
        public string Weekend { get; set; }
        public string Everyday { get; set; }
    }

    public class TwentyFourSevenTimer
    {
        public Dictionary<DayOfWeek, List<TimeSpan>> Alarms { get; set; }
        public DateTime LastAlarmTriggered { get; set; }

        protected readonly INow myTimeReference;
        protected readonly TwentyFourSevenTimerConfig myConfig;
               
        public TwentyFourSevenTimer(TwentyFourSevenTimerConfig config)
            : this(config, new RealNow())
        {

        }

        public TwentyFourSevenTimer(TwentyFourSevenTimerConfig config, INow time)
        {
            myTimeReference = time;            
            myConfig = config;

            Alarms = ParseConfig();
            LastAlarmTriggered = myTimeReference.Now();
        }

        public IEnumerable<DateTime> Triggered()
        {
            return Triggered(TimeSpan.Zero);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grace">Not sure I want to use/expose this yet</param>
        /// <returns></returns>
        private IEnumerable<DateTime> Triggered(TimeSpan grace)
        {
            var now = myTimeReference.Now();

            var triggered = (from alarmTime in Alarms[now.DayOfWeek]
                    let dtAlarm = (new DateTime(now.Year, now.Month, now.Day) + alarmTime)
                    where (dtAlarm > LastAlarmTriggered) && (dtAlarm <= now)
                    orderby dtAlarm.TimeOfDay descending 
                    select dtAlarm).ToList();

            if (triggered.Count > 0)
                LastAlarmTriggered = triggered.First();

            return triggered;
        }

        protected Dictionary<DayOfWeek, List<TimeSpan>> ParseConfig()
        {
            var alarms = new Dictionary<DayOfWeek, List<TimeSpan>>
                             {
                                 {DayOfWeek.Sunday, new List<TimeSpan>()},
                                 {DayOfWeek.Monday, new List<TimeSpan>()},
                                 {DayOfWeek.Tuesday, new List<TimeSpan>()},
                                 {DayOfWeek.Wednesday, new List<TimeSpan>()},
                                 {DayOfWeek.Thursday, new List<TimeSpan>()},
                                 {DayOfWeek.Friday, new List<TimeSpan>()},
                                 {DayOfWeek.Saturday, new List<TimeSpan>()}
                             };

            // parse the individual days
            alarms[DayOfWeek.Sunday].AddRange(ParseDay(DayOfWeek.Sunday));
            alarms[DayOfWeek.Monday].AddRange(ParseDay(DayOfWeek.Monday));
            alarms[DayOfWeek.Tuesday].AddRange(ParseDay(DayOfWeek.Tuesday));
            alarms[DayOfWeek.Wednesday].AddRange(ParseDay(DayOfWeek.Wednesday));
            alarms[DayOfWeek.Thursday].AddRange(ParseDay(DayOfWeek.Thursday));
            alarms[DayOfWeek.Friday].AddRange(ParseDay(DayOfWeek.Friday));
            alarms[DayOfWeek.Saturday].AddRange(ParseDay(DayOfWeek.Saturday));

            // Everyday
            var everyday = ParseEveryday();
            alarms[DayOfWeek.Sunday].AddRange(everyday);
            alarms[DayOfWeek.Monday].AddRange(everyday);
            alarms[DayOfWeek.Tuesday].AddRange(everyday);
            alarms[DayOfWeek.Wednesday].AddRange(everyday);
            alarms[DayOfWeek.Thursday].AddRange(everyday);
            alarms[DayOfWeek.Friday].AddRange(everyday);
            alarms[DayOfWeek.Saturday].AddRange(everyday);

            // now weekend
            var weekend = ParseWeekend();
            alarms[DayOfWeek.Sunday].AddRange(weekend);
            alarms[DayOfWeek.Saturday].AddRange(weekend);

            // now weekdays
            var weekdays = ParseWeekdays();
            alarms[DayOfWeek.Monday].AddRange(weekdays);
            alarms[DayOfWeek.Tuesday].AddRange(weekdays);
            alarms[DayOfWeek.Wednesday].AddRange(weekdays);
            alarms[DayOfWeek.Thursday].AddRange(weekdays);
            alarms[DayOfWeek.Friday].AddRange(weekdays);

            return alarms;
        }

        /// <summary>
        /// Converts a comma separated list of times
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TimeSpan> ParseDay(DayOfWeek day)
        {
            return ParseDay(day.ToString());
        }

        /// <summary>
        /// Converts a comma separated list of times
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<TimeSpan> ParseEveryday()
        {
            return ParseDay("Everyday");
        }

        /// <summary>
        /// Converts a comma separated list of times
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<TimeSpan> ParseWeekdays()
        {
            return ParseDay("Weekdays");
        }

        /// <summary>
        /// Converts a comma separated list of times
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<TimeSpan> ParseWeekend()
        {
            return ParseDay("Weekend");
        }

        /// <summary>
        /// Converts a comma separated list of times
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        protected virtual IEnumerable<TimeSpan> ParseDay(string day)
        {
            Guard.AgainstEmpty(day);

            var pi = myConfig.GetType().GetProperty(day);
            Guard.AgainstNull(pi, string.Format("Day '{0}' does not match a configuration property", day));

            var dayAlarms = pi.GetValue(myConfig, null);

            if (dayAlarms == null)
                return new TimeSpan[0];
            if (string.IsNullOrEmpty(dayAlarms.ToString()))
                return new TimeSpan[0];

            var alarms = new List<TimeSpan>();
            var parts = dayAlarms.ToString().Split(',');

            foreach (var part in parts)
            {
                TimeSpan alarm;

                if (!TimeSpan.TryParse(part, out alarm))
                    throw new FormatException(string.Format("{0} part '{1}' is invalid", day, part));

                if (!alarms.Contains(alarm))
                    alarms.Add(alarm);
            }

            return alarms;
        }
    }
}