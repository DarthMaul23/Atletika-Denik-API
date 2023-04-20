using Atletika_Denik_API.Data.ViewModels;

namespace Atletika_Denik_API.Data.Services
{
    public class DataTransformation
    {
        public Info GetInfoObject(int userId, Info? info)
        {
            return new Info()
            {
                Id = userId,
                FirstName = info.FirstName,
                LastName = info.LastName,
                Email = info.Email,
                Adress = info.Adress,
                BirthDate = info.BirthDate,
                Phone = info.Phone,
                Sex = info.Sex,
                Category = info.Category,
                Discipline = info.Discipline,
                Height = info.Height,
                Weight = info.Weight
            };
        }

        internal int GetDayOfWeekFromDate(string? _date)
        {
            return (int)Convert.ToDateTime(_date).DayOfWeek;
        }

        internal string GetDayOfWeek(int _index)
        {
            List<string> dayOfWeek = new List<string>();
            dayOfWeek.Add("Pondělí");
            dayOfWeek.Add("Úterý");
            dayOfWeek.Add("Středa");
            dayOfWeek.Add("Čtvrtek");
            dayOfWeek.Add("Pátek");
            dayOfWeek.Add("Sobota");
            dayOfWeek.Add("Neděle");

            return dayOfWeek.ElementAt(_index);

        }

        internal string GetMonth(int _index)
        {
            List<string> months = new List<string>();
            months.Add("Ledna");
            months.Add("Února");
            months.Add("Března");
            months.Add("Dubna");
            months.Add("Května");
            months.Add("Června");
            months.Add("Července");
            months.Add("Srpna");
            months.Add("Září");
            months.Add("Října");
            months.Add("Listopadu");
            months.Add("Prosince");
            return months.ElementAt(_index);
        }

        public List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate, int targetDayIndex, string _interval, bool weekend)
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime currentDate = startDate;
            DayOfWeek targetDay = (DayOfWeek)(targetDayIndex % 7);
            int interval = GetInterval(_interval);

            while (currentDate <= endDate)
            {
                if (currentDate.DayOfWeek == targetDay && interval != 1)
                {
                    dates.Add(currentDate);
                    currentDate = currentDate.AddDays(interval);
                }
                else
                {
                    if (weekend)
                    {
                        dates.Add(currentDate);
                        currentDate = currentDate.AddDays(1);
                    }
                    else
                    {
                        if (currentDate.DayOfWeek != DayOfWeek.Saturday || currentDate.DayOfWeek != DayOfWeek.Sunday)
                        {
                            dates.Add(currentDate);
                            currentDate = currentDate.AddDays(1);
                        }
                    }
                }
            }

            return dates;
        }

        private int GetInterval(string interval)
        {
            switch (interval)
            {
                case "daily":
                    return 1;
                case "weekly":
                    return 7;
                case "biweekly":
                    return 14;
                case "monthly":
                    return 30;
                default:
                    return 1;
            }
        }

        public DateTime GetMondayOfWeek(DateTime inputDate)
        {
            int delta = DayOfWeek.Monday - inputDate.DayOfWeek;
            if (delta > 0)
            {
                delta -= 7;
            }
            DateTime monday = inputDate.AddDays(delta);
            return monday;
        }
    }
}