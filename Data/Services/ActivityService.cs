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
    private Atletika_Denik_API.Data.Models.ActivitiesContext _activitiesContext;
    private DataTransformation dataTrans = new DataTransformation();
    Errors error = new Errors();
    public ActivityService(Atletika_Denik_API.Data.Models.ActivitiesContext activiesContext)
    {
        _activitiesContext = activiesContext;
    }

    public ReturnItems GetListOfActivities(int userId, int pageNo, int itemsPerPage, string searchTerm)
    {
        using (var context = _activitiesContext)
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

    public TagDetail GetActivityDescription(string tagDetailId)
    {
        using (var context = _activitiesContext)
        {
            var query = (from tagDetail in context.Tag_Details
                         where tagDetail.id == tagDetailId
                         join tagAsoc in context.Tag_Association on tagDetail.tagAsocId equals tagAsoc.id
                         join tag in context.Tag on tagAsoc.tagId equals tag.id
                         select new
                         {
                             id = tagDetail.id,
                             name = tag.name,
                             color = tag.color,
                             description = tag.description
                         });

            var data = query.ToList().ElementAt(0);

            return new TagDetail() { id = tagDetailId, name = data.name, color = data.color, description = data.description };
        }
    }

    public EditTag GetActivityDetail(string tagId)
    {
        using (var context = _activitiesContext)
        {
            var queryTag = (from tag in context.Tag
                            where tag.id == tagId
                            select new
                            {
                                id = tag.id,
                                name = tag.name,
                                color = tag.color,
                                description = tag.description
                            }).First();

            var queryTagSettings = from tagUserSettings in context.Tag_User_Settings
                                   join tagAssociation in context.Tag_Association on tagUserSettings.tagAsocId equals tagAssociation.id
                                   where tagAssociation.tagId == tagId
                                   select new
                                   {
                                       id = tagAssociation.userId,
                                       asocId = tagUserSettings.tagAsocId,
                                       repetition = tagUserSettings.repetition,
                                       weekDay = tagUserSettings.weekDay,
                                       col = tagUserSettings.col,
                                       dateFrom = tagUserSettings.dateFrom,
                                       dateTo = tagUserSettings.dateTo,
                                   };

            List<EditTagSettings> _settings = new List<EditTagSettings>();

            foreach (var _item in queryTagSettings.ToList())
            {
                _settings.Add(new EditTagSettings()
                {
                    id = _item.id,
                    tagAsocId = _item.asocId,
                    repetition = _item.repetition,
                    weekDay = _item.weekDay,
                    col = _item.col,
                    dateFrom = _item.dateFrom,
                    dateTo = _item.dateTo
                });
            }

            /*
            string queryString = query.ToQueryString();
            Console.WriteLine(queryString);
            */

            return new EditTag() { id = queryTag.id, name = queryTag.name, color = queryTag.color, description = queryTag.description, settings = _settings };
        }
    }

    public async Task CreateNewActivity(NewTag tag, List<NewTagUserSettings> details)
    {
        using (var context = _activitiesContext)
        {
            string _NewTagId = Guid.NewGuid().ToString();

            var _tag = new Tag()
            {
                id = _NewTagId,
                name = tag.name,
                color = tag.color + "",
                description = tag.description + ""
            };

            context.Tag.Add(_tag);

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

                List<DateTime> dates = dataTrans.GetDatesBetween(Convert.ToDateTime(detail.dateFrom), Convert.ToDateTime(detail.dateTo), detail.weekDay, detail.repetition, false);

                foreach (var date in dates)
                {
                    var _TagDetails = new Tag_Details()
                    {
                        id = Guid.NewGuid().ToString(),
                        tagAsocId = _NewTagAsocId,
                        date = date.ToString("yyyy-MM-dd"),
                        created = DateTime.Now.ToString("yyyy-MM-dd")
                    };

                    context.Tag_Details.Add(_TagDetails);
                }

                await context.SaveChangesAsync();
            }
        }
    }
    public List<ActivityDefinitionDto> GetActivityDefinitionByTagAssociationId(string tagAssociationId, int userId)
    {
        var query = from tad in _activitiesContext.Tag_Activities_Definitions
                    join taa in _activitiesContext.Tag_Activities_Association on tad.Id equals taa.ActivityDefinitionId
                    join ta in _activitiesContext.Tag_Association on taa.TagId equals ta.id
                    join td in _activitiesContext.Tag_Details on ta.id equals td.tagAsocId
                    join taur in _activitiesContext.Tag_Activities_User_Responses on td.id equals taur.TagDetailId into UserResponsesGroup
                    from ur in UserResponsesGroup.DefaultIfEmpty()
                    where taa.TagId == tagAssociationId && ta.userId == userId
                    select new { tad, ur };

        var result = query.ToList();

        if (result.Count == 0)
        {
            return null;
        }

        var activityDefinitionDtos = result.GroupBy(r => r.tad.Id)
            .Select(g => new ActivityDefinitionDto
            {
                Id = g.Key,
                Name = g.First().tad.Name,
                Definitions = g.First().tad.Definition,
                Responses = g.Where(r => r.tad.Name == g.First().tad.Name)
                .GroupBy(r => r.ur.ActivityDefinitionId)
                .Select(grp => grp.First())
                .Select(r => new UserResponseDto { Id = r.ur.Id, Response = r.ur.Response })
                .ToList() // There needs to be filter to get only one record for the definition, not all the reocrords of user responses for all the activities defintions
            })
            .ToList();

        return activityDefinitionDtos;
    }
}