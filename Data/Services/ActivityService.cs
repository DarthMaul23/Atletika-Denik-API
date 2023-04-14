using System.Net.Mime;
using System.Text.Json;
using Atletika_Denik_API.Data.Models;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using TrainingDefinition = Atletika_Denik_API.Data.ViewModels.TrainingDefinition;

namespace Atletika_Denik_API.Data.Services;

public class ActivityService
{
    private Atletika_Denik_API.Data.Models.ActivitiesContext _activiesContext;
    Errors error = new Errors();
    public ActivityService(Atletika_Denik_API.Data.Models.ActivitiesContext activiesContext)
    {
        _activiesContext = activiesContext;
    }

    public ReturnItems GetListOfActivities(int userId, int pageNo, int itemsPerPage, string searchTerm)
    {
        using (var context = _activiesContext)
        {
            var query = context.Tag
                .Where(tag => tag.name.Contains(searchTerm == null ? "" : searchTerm))
                .OrderBy(tag => tag.id)
                .Skip((pageNo - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            var _noRecords = context.Tag.Where(tag => tag.name.Contains(searchTerm == null ? "" : searchTerm)).Count();

            return new ReturnItems() { tags = query, noRecords = _noRecords };
        }
    }

    public async Task CreateNewActivity(NewTag tag, List<NewTagDetail> details)
    {
        using (var context = _activiesContext)
        {
            foreach (var detail in details)
            {

                List<DateTime> dates = GetDatesBetween(Convert.ToDateTime(detail.dateFrom), Convert.ToDateTime(detail.dateTo), detail.weekDay, GetInterval(detail.repetition));

                foreach (var date in dates)
                {

                }

                await context.SaveChangesAsync();

            }
        }
    }

    private List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate, int targetDayIndex, int interval)
    {
        List<DateTime> dates = new List<DateTime>();
        DateTime currentDate = startDate;
        DayOfWeek targetDay = (DayOfWeek)(targetDayIndex % 7);

        while (currentDate <= endDate)
        {
            if (currentDate.DayOfWeek == targetDay)
            {
                dates.Add(currentDate);
                currentDate = currentDate.AddDays(interval);
            }
            else
            {
                currentDate = currentDate.AddDays(1);
            }
        }

        return dates;
    }

    private int GetInterval(string interval)
    {
        if (interval == "daily")
        {
            return 1;
        }
        else
        if (interval == "weekly")
        {
            return 7;
        }
        else
        if (interval == "biweekly")
        {
            return 14;
        }
        else
        if (interval == "monthly")
        {
            return 30;
        }
        else
        {
            return 1;
        }
    }
}