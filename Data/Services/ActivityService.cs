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

    // This method gets a list of activities based on a user, page number, items per page and a search term
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

    // Get activity description by tag association ID
    public TagDetail GetActivityDescription(string tagAssociationId)
    {
        using (var context = _activitiesContext)
        {
            var query = context.Tag_Details
                        .Join(context.Tag_Association, td => td.tagAsocId, ta => ta.id, (td, ta) => new { td, ta })
                        .Join(context.Tag, j => j.ta.tagId, t => t.id, (j, t) => new { j, t })
                        .Where(jt => jt.j.ta.id == tagAssociationId)
                        .Select(jt => new
                        {
                            id = jt.j.td.id,
                            name = jt.t.name,
                            color = jt.t.color,
                            description = jt.t.description
                        });


            if (query.ToList().Count == 0)
            {
                return null;
            }

            var data = query.ToList().ElementAt(0);

            return new TagDetail() { id = data.id, name = data.name, color = data.color, description = data.description };
        }
    }

    // Get activity detail by tag ID
    public EditTag GetActivityDetail(string tagId)
    {
        using (var context = _activitiesContext)
        {
            var queryTag = context.Tag
                .Where(tag => tag.id == tagId)
                .Select(tag => new
                {
                    id = tag.id,
                    name = tag.name,
                    color = tag.color,
                    description = tag.description
                })
                .FirstOrDefault();

            var queryTagSettings = context.Tag_User_Settings
                                    .Join(context.Tag_Association,
                                        tagUserSettings => tagUserSettings.tagAsocId,
                                        tagAssociation => tagAssociation.id,
                                        (tagUserSettings, tagAssociation) => new
                                        {
                                            TagUserId = tagAssociation.userId,
                                            TagUserAsocId = tagUserSettings.tagAsocId,
                                            Repetition = tagUserSettings.repetition,
                                            WeekDay = tagUserSettings.weekDay,
                                            Col = tagUserSettings.col,
                                            DateFrom = tagUserSettings.dateFrom,
                                            DateTo = tagUserSettings.dateTo,
                                            TagId = tagAssociation.tagId
                                        })
                                    .Where(tagUser => tagUser.TagId == tagId)
                                    .Select(tagUser => new
                                    {
                                        id = tagUser.TagUserId,
                                        asocId = tagUser.TagUserAsocId,
                                        repetition = tagUser.Repetition,
                                        weekDay = tagUser.WeekDay,
                                        col = tagUser.Col,
                                        dateFrom = tagUser.DateFrom,
                                        dateTo = tagUser.DateTo,
                                    })
                                    .ToList();


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

    // Create a new activity with a tag, user settings, and activities
    public async Task CreateNewActivity(NewTag tag, List<NewTagUserSettings> details, List<ActivityDefinitionCreate> activities)
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
                    Definition = activity.Definition,
                    Order = activity.Order
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

    // Get activity definition by tag association ID, user ID, and date
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
                        .OrderBy(joined => joined.def.Order)
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
    
    // Set activity response by response ID and new response value
    public void setActivityResponse(string responseId, int newResponse)
    {
        using (var context = _activitiesContext)
        {
            var record = context.Tag_Activities_User_Responses.First(x => x.Id == responseId);
            if (record != null)
            {
                record.Response = newResponse;
                context.SaveChanges();
            }
        }
    }
}