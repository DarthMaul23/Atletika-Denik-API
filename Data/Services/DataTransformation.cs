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
    }
}