using Microsoft.Win32;
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

    public async Task CreateNewActivity(NewTag tag, List<NewTagUserSettings> details)
    {
        using (var context = _activiesContext)
        {
            string _NewTagId = Guid.NewGuid().ToString();

            var _tag = new Tag()
            {
                id = _NewTagId,
                name = tag.name,
                color = tag.color+"",
                description = tag.description+""
            };

            context.Tag.Add(_tag);
            context.SaveChanges();

            foreach (var detail in details)
            {
                string _NewTagAsocId = Guid.NewGuid().ToString();
                var _associationItem = new Tag_Association()
                {
                    id = _NewTagAsocId,
                    tagId = _NewTagId,
                    userId = detail.id
                };

                context.Tag_Association.Add(_associationItem);
                context.SaveChanges();


                var _TagUserSettingsItem = new Tag_User_Settings()
                {
                    id = Guid.NewGuid().ToString(),
                    tagAsocId = _NewTagAsocId,
                    repetition = detail.repetition,
                    weekDay = detail.weekDay,
                    col = detail.column,
                    dateFrom = detail.dateFrom,
                    dateTo = detail.dateTo
                };

                context.Tag_User_Settings.Add(_TagUserSettingsItem);
                context.SaveChanges();

                List<DateTime> dates = GetDatesBetween(Convert.ToDateTime(detail.dateFrom), Convert.ToDateTime(detail.dateTo), detail.weekDay, GetInterval(detail.repetition), false);

                foreach (var date in dates)
                {
                    var _TagDetails = new Tag_Details()
                    {
                        id = Guid.NewGuid().ToString(),
                        tagAsocId = _NewTagAsocId,
                        date = date.ToOADate().ToString(),
                        created = DateTime.Now.ToString()
                    };

                    context.Tag_Details.Add(_TagDetails);
                    context.SaveChanges();
                }

                await context.SaveChangesAsync();

            }
        }
    }

    private List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate, int targetDayIndex, int interval, bool weekend)
    {
        List<DateTime> dates = new List<DateTime>();
        DateTime currentDate = startDate;
        DayOfWeek targetDay = (DayOfWeek)(targetDayIndex % 7);

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
}