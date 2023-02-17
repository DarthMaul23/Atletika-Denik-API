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
            return (int) Convert.ToDateTime(_date).DayOfWeek;
        }
    }
}