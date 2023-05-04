using System.Data;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json;
using Atletika_Denik_API.Data.Models;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using TrainingDefinition = Atletika_Denik_API.Data.ViewModels.TrainingDefinition;

namespace Atletika_Denik_API.Data.Services;

public class TrainingService
{
    private Atletika_Denik_API.Data.Models.TrainingContext _trainingContext;

    private Atletika_Denik_API.Data.Models.ActivitiesContext _tagContext = new ActivitiesContext();

    DataTransformation dataTrans = new DataTransformation();

    Errors error = new Errors();
    public TrainingService(Atletika_Denik_API.Data.Models.TrainingContext context)
    {
        _trainingContext = context;
    }

    public List<ViewModels.TrainingDay> GetUserTrainignWeek(int _id, string _date)
    {
        using (var context = _trainingContext)
        {
            DateTime date = _date == null || _date == "" ? DateTime.Now : DateTime.Parse(_date);
            var list = new List<ViewModels.TrainingDay>();

            var query = (from users in context.Users.Where(u => u.id == _id)
                         join asociace_treninku in context.Training_Association.Where(a =>
                                 a.userId == _id && (DateTime)(object)a.date >= date.AddDays(-((int)date.DayOfWeek + 1)) &&
                                 (DateTime)(object)a.date <= date.AddDays(+(7 - (int)date.DayOfWeek))) on users.id equals
                             asociace_treninku.userId
                         join trenink in context.Training_Definition on asociace_treninku.trainingId equals trenink.trainingId
                         join trenink_user_response in context.Training_User_Response on
                             new { n1 = trenink.trainingId, n2 = trenink.rowId } equals
                             new { n1 = trenink_user_response.trainingId, n2 = trenink_user_response.rowId }
                         orderby asociace_treninku.date
                         select new
                         {
                             userId = users.id,
                             dayOfWeek = new DataTransformation().GetDayOfWeekFromDate(asociace_treninku.date),
                             date = asociace_treninku.date,
                             treninkId = asociace_treninku.trainingId,
                             definitionId = trenink.id,
                             rowId = trenink.rowId,
                             col1 = trenink.col1,
                             col2 = trenink.col2,
                             col3 = trenink.col3,
                             col4 = trenink.col4,
                             responseId = trenink_user_response.id,
                             responseRowId = trenink_user_response.rowId,
                             response = trenink_user_response.response
                         }).ToArray();

            var dates = dataTrans.GetDatesBetween(dataTrans.GetMondayOfWeek(date), dataTrans.GetMondayOfWeek(date).AddDays(4), 1, "daily", false);

            foreach (var item in dates)
            {

                List<ViewModels.Training_Definition> definitionsList = new List<ViewModels.Training_Definition>();
                List<ViewModels.Training_User_Response> responsesList = new List<ViewModels.Training_User_Response>();
                var _testDate = item;
                foreach (var _item in query.Where(x => x.date == item.ToString("yyyy-MM-dd")).ToArray())
                {
                    definitionsList.Add(new Training_Definition()
                    {
                        id = _item.definitionId,
                        rowId = _item.rowId,
                        col1 = _item.col1,
                        col2 = _item.col2,
                        col3 = _item.col3,
                        col4 = _item.col4
                    });
                }

                foreach (var _item in query.Where(x => x.date == item.ToString("yyyy-MM-dd")).ToArray())
                {
                    responsesList.Add(new ViewModels.Training_User_Response()
                    {
                        id = _item.responseId,
                        rowId = _item.responseRowId,
                        response = _item.response
                    });
                }

                list.Add(new ViewModels.TrainingDay()
                {
                    userId = _id,
                    dayOfWeek = (int)item.DayOfWeek,
                    date = item.ToString("yyyy-MM-dd"),
                    type = 0,
                    activity = GetActivities(_id, item.ToString("yyyy-MM-dd")),
                    training = new Training1()
                    {
                        trainingId = query.FirstOrDefault(x => x.date == item.ToString("yyyy-MM-dd"))?.treninkId,
                        definition = definitionsList,
                        response = responsesList
                    },
                });
            }
            return list;
        }
    }

    public async Task<Error> CreateTrainingAsync(int userId, string date, int type, List<New_Training_Definition> definition, List<New_Training_User_Response> response)
    {
        using (var context = _trainingContext)
        {
            if (!Enumerable.Range(1, 4).Contains(type))
            {
                return error.GetError("Ex03");
            }
            else
            if (context.Training_Association.FirstOrDefault(a => a.userId == userId && a.date == date) != null)
            {
                return error.GetError("Ex02");
            }
            else
            {
                string _trainingId = Guid.NewGuid().ToString();

                int id_response = context.Training_User_Response.FirstOrDefault() == null ? 0 : context.Training_User_Response.Select(r => r.id).Max();
                var responses = new List<Training_User_Response>();
                foreach (var item in response)
                {
                    responses.Add(new Training_User_Response()
                    {
                        id = id_response + (response.IndexOf(item) + 1),
                        trainingId = _trainingId,
                        rowId = (response.IndexOf(item) + 1),
                        response = item.response
                    });
                }

                await context.Training_User_Response.AddRangeAsync(responses);
                await context.SaveChangesAsync();

                int id_training_association = context.Training_Association.FirstOrDefault() == null ? 0 : context.Training_Association.Select(a => a.id).Max() + 1;
                var _associationItem = new Training_Association()
                {
                    id = id_training_association,
                    userId = userId,
                    trainingId = _trainingId,
                    date = date,
                };

                context.Training_Association.Add(_associationItem);
                context.SaveChanges();

                int trainingNo = context.Training_Definition.FirstOrDefault() == null ? 0 : context.Training_Definition.Select(a => a.id).Max();

                var definitions = new List<Training_Definition>();

                foreach (var item in definition)
                {
                    definitions.Add(new Training_Definition()
                    {
                        id = trainingNo + (definition.IndexOf(item) + 1),
                        trainingId = _trainingId,
                        rowId = (definition.IndexOf(item) + 1),
                        col1 = item.col1,
                        col2 = item.col2,
                        col3 = item.col3,
                        col4 = item.col4
                    });
                }

                await context.Training_Definition.AddRangeAsync(definitions);
                await context.SaveChangesAsync();

                return error.GetError("S01");
            }
        }
    }

    public void UpdateTraining(string _trainingId, int type, List<New_Training_Definition> definition, List<New_Training_User_Response> response)
    {
        using (var context = _trainingContext)
        {
            var trainingAsociaction = context.Training_Association.FirstOrDefault(a => a.trainingId == _trainingId);
            if (trainingAsociaction != null)
            {
                var _training = context.Training_Definition.Where(a => a.trainingId == _trainingId);

                foreach (var _item in _training)
                {
                    context.Entry(_item).State = EntityState.Deleted;
                    context.Training_Definition.Remove(_item);
                }

                int trainingNo = context.Training_Definition.Select(a => a.id).Max();

                foreach (var item in definition)
                {
                    context.Training_Definition.Add(new Training_Definition()
                    {
                        id = trainingNo + (definition.IndexOf(item) + 1),
                        trainingId = _trainingId,
                        rowId = (definition.IndexOf(item) + 1),
                        col1 = item.col1,
                        col2 = item.col2,
                        col3 = item.col3,
                        col4 = item.col4
                    });
                }

                var _trainingResponse = context.Training_User_Response.Where(a => a.trainingId == _trainingId);

                foreach (var _item in _trainingResponse)
                {
                    context.Entry(_item).State = EntityState.Deleted;
                    context.Training_User_Response.Remove(_item);
                }

                int id_response = context.Training_User_Response.Select(r => r.id).Max();

                foreach (var item in response)
                {
                    context.Training_User_Response.Add(new Training_User_Response()
                    {
                        id = id_response + (response.IndexOf(item) + 1),
                        trainingId = _trainingId,
                        rowId = (response.IndexOf(item) + 1),
                        response = item.response
                    });
                }

                context.SaveChanges();
            }
        }
    }

    public Error DeleteTraining(string _trainingId)
    {
        using (var context = _trainingContext)
        {
            var trainingAsociaction = context.Training_Association.FirstOrDefault(a => a.trainingId == _trainingId);
            if (trainingAsociaction == null)
            {
                return error.GetError("Ex04");
            }
            else
            {
                context.Entry(trainingAsociaction).State = EntityState.Deleted;
                context.Remove(trainingAsociaction);

                var _training = context.Training_Definition.Where(a => a.trainingId == _trainingId);

                foreach (var _item in _training)
                {
                    context.Entry(_item).State = EntityState.Deleted;
                    context.Training_Definition.Remove(_item);
                }

                var _trainingResponse = context.Training_User_Response.Where(r => r.trainingId == _trainingId);

                foreach (var _item in _trainingResponse)
                {
                    context.Entry(_item).State = EntityState.Deleted;
                    context.Training_User_Response.Remove(_item);
                }

                context.SaveChanges();

                return error.GetError("S02");
            }
        }
    }

    public void UpdateTrainingResponse(string _trainingId, List<Training_User_Response> response)
    {
        using (var context = _trainingContext)
        {
            var _trainingResponse = context.Training_User_Response.Where(a => a.trainingId == _trainingId);

            foreach (var _item in _trainingResponse)
            {
                context.Entry(_item).State = EntityState.Deleted;
                context.Training_User_Response.Remove(_item);
            }

            int id_response = context.Training_User_Response.Select(r => r.id).Max();

            foreach (var item in response)
            {
                context.Training_User_Response.Add(new Training_User_Response()
                {
                    id = id_response + (response.IndexOf(item) + 1),
                    trainingId = _trainingId,
                    rowId = (response.IndexOf(item) + 1),
                    response = item.response
                });
            }
            context.SaveChanges();
        }
    }

    private List<Activity2> GetActivities(int _userId, string _trainingDate)
    {
        using (var context = new ActivitiesContext())
        {
            // Get Tag_Association entries for the user
            var tagAssociations = context.Tag_Association
                .Where(ta => ta.userId == _userId)
                .ToList();

            // Get Tag_Details for the specified training date
            var tagDetails = context.Tag_Details
                .Where(td => td.date == _trainingDate)
                .ToList();

            // Get Tag_User_Response for the specified Tag_Details
            var tagUserResponses = context.Tag_User_Response
                .Where(tur => tagDetails.Select(td => td.id).Contains(tur.id))
                .ToList();

            // Combine the data
            var result = new List<Activity2>();

            foreach (var ta in tagAssociations)
            {
                var tag = context.Tag.FirstOrDefault(t => t.id == ta.tagId);

                if (tag != null)
                {
                    var detail = tagDetails.FirstOrDefault(td => td.tagAsocId == ta.id);

                    if (detail != null)
                    {
                        var response = tagUserResponses.FirstOrDefault(tur => tur.id == detail.id);

                        result.Add(new Activity2
                        {
                            tagAsociationId = ta.id,
                            name = tag.name,
                            color = tag.color,
                            description = tag.description,
                            response = response?.response ?? 0
                        });
                    }
                }
            }

            return result;
        }
    }

}