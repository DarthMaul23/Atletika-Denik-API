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

    public ReturnItems GetListOfActivities(int userId, int pageNo, int itemsPerPage, string? searchTerm)
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

    public TagDetail GetActivityDescription(string tagAsociationId)
    {
        using (var context = _activitiesContext)
        {
            var query = (from tagDetail in context.Tag_Details
                         join tagAsoc in context.Tag_Association on tagDetail.tagAsocId equals tagAsoc.id
                         join tag in context.Tag on tagAsoc.tagId equals tag.id
                         where tagAsoc.id == tagAsociationId
                         select new
                         {
                             id = tagDetail.id,
                             name = tag.name,
                             color = tag.color,
                             description = tag.description
                         });

            if (query.ToList().Count == 0)
            {
                return null;
            }

            var data = query.ToList().ElementAt(0);

            return new TagDetail() { id = data.id, name = data.name, color = data.color, description = data.description };
        }
    }

    public EditTag GetActivityDetail(string tagAsocId)
    {
        using (var context = _activitiesContext)
        {
            var queryTag = (from tag in context.Tag
                            where tag.id == context.Tag_Association.FirstOrDefault(x => x.id == tagAsocId).tagId
                            select new
                            {
                                id = tag.id,
                                name = tag.name,
                                color = tag.color,
                                description = tag.description
                            }).First();

            var queryTagSettings = from tagUserSettings in context.Tag_User_Settings
                                   join tagAssociation in context.Tag_Association on tagUserSettings.tagAsocId equals tagAssociation.id
                                   where tagAssociation.tagId == queryTag.id
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

            return new EditTag() { id = queryTag.id, name = queryTag.name, color = queryTag.color, description = queryTag.description, settings = _settings };
        }
    }

    public async Task CreateNewActivity(NewTag tag, List<NewTagUserSettings> details, List<ActivityDefinitionDto> activities)
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

            //Preparing the list of records for table Tag_Actvities_Definitions
            List<Tag_Activities_Definitions> activities_Definitions = new List<Tag_Activities_Definitions>();
            foreach (var activity in activities)
            {
                activities_Definitions.Add(new Tag_Activities_Definitions()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = activity.Name,
                    Definition = activity.Definition
                });
            }

            context.Tag_Activities_Definitions.AddRange(activities_Definitions);

            //Preparing the list of records for table Tag_Activities_Association
            List<Tag_Activities_Association> activities_Associations = new List<Tag_Activities_Association>();
            foreach (var activity in activities_Definitions)
            {
                activities_Associations.Add(new Tag_Activities_Association()
                {
                    Id = Guid.NewGuid().ToString(),
                    TagId = _NewTagId,
                    ActivityDefinitionId = activity.Id
                });
            }

            context.Tag_Activities_Association.AddRange(activities_Associations);

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

                foreach (var _date in dates)
                {
                    var _TagDetail = new Tag_Details()
                    {
                        id = Guid.NewGuid().ToString(),
                        tagAsocId = _NewTagAsocId,
                        date = _date.ToString("yyyy-MM-dd"),
                        created = DateTime.Now.ToString("yyyy-MM-dd")
                    };

                    context.Tag_Details.Add(_TagDetail);

                    List<Tag_Activities_User_Responses> activities_User_Responses = new List<Tag_Activities_User_Responses>();
                    foreach (var _item in activities_Definitions)
                    {
                        activities_User_Responses.Add(
                            new Tag_Activities_User_Responses()
                            {
                                Id = Guid.NewGuid().ToString(),
                                UserId = detail.id,
                                TagDetailId = _TagDetail.id,
                                ActivityDefinitionId = _item.Id,
                                Response = 1,
                                Date = _date.ToString("yyyy-MM-dd")
                            }
                        );
                    }

                    context.Tag_Activities_User_Responses.AddRange(activities_User_Responses);
                }

                await context.SaveChangesAsync();
            }
        }
    }

    public List<ActivityDefinitionDto> GetActivityDefinitionByTagAssociationId(string tagAssociationId, int userId, string date)
    {

        var query = _activitiesContext.Tag_Activities_User_Responses
                        .Join(_activitiesContext.Tag_Activities_Definitions,
                        res => res.ActivityDefinitionId,
                        def => def.Id,
                        (res, def) => new { res, def })
                        .Where(joined => _activitiesContext.Tag_Details
                        .Where(detail => _activitiesContext.Tag_Association
                            .Where(assoc => assoc.id == tagAssociationId && assoc.userId == userId && detail.date == date)
                            .Select(assoc => assoc.id)
                            .Contains(detail.tagAsocId))
                        .Select(detail => detail.id)
                        .Contains(joined.res.TagDetailId))
                        .Select(joined => new
                        {
                            DefinitionId = joined.def.Id,
                            ResponseId = joined.res.Id,
                            Response = joined.res.Response,
                            Name = joined.def.Name,
                            Definition = joined.def.Definition
                        });

        var result = query.ToList();

        if (result.Count == 0)
        {
            return null;
        }

        var activityDefinitionDtos = result
            .Select(g => new ActivityDefinitionDto
            {
                Id = g.DefinitionId,
                Name = g.Name,
                Definition = g.Definition,
                Response = new UserResponseDto() { Id = g.ResponseId, Response = g.Response }
            })
            .ToList();

        return activityDefinitionDtos;
    }
}