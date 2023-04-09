using System.Text.Json;
using Atletika_Denik_API.Data.Models;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using TrainingDefinition = Atletika_Denik_API.Data.ViewModels.TrainingDefinition;

namespace Atletika_Denik_API.Data.Services;

public class TrainingService
{
    private Atletika_Denik_API.Data.Models.TrainingContext _trainingContext;
    Errors error = new Errors();
    public TrainingService(Atletika_Denik_API.Data.Models.TrainingContext context)
    {
        _trainingContext = context;
    }

    public List<ViewModels.Training> GetUserTrainignWeek(int _id, string _date)
    {
        using (var context = _trainingContext)
        {
            DateTime date = _date == null || _date == "" ? DateTime.Now : DateTime.Parse(_date);
            var list = new List<ViewModels.Training>();
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
                         }).ToList();

            // Console.WriteLine("Query: " + query.ToQueryString());

            foreach (var item in query)
            {
                if (!list.Contains(list.Find(x => x.TrainingId == item.treninkId)))
                {
                    List<ViewModels.Training_Definition> definitionsList = new List<ViewModels.Training_Definition>();
                    List<ViewModels.Training_User_Response> responsesList = new List<ViewModels.Training_User_Response>();

                    foreach (var _item in query.Where(x => x.treninkId == item.treninkId).ToList())
                    {
                        definitionsList.Add(new ViewModels.Training_Definition()
                        {
                            id = _item.definitionId,
                            rowId = _item.rowId,
                            col1 = _item.col1,
                            col2 = _item.col2,
                            col3 = _item.col3,
                            col4 = _item.col4
                        });

                        responsesList.Add(new ViewModels.Training_User_Response()
                        {
                            id = _item.responseId,
                            rowId = _item.responseRowId,
                            response = _item.response
                        });
                    }

                    list.Add(new ViewModels.Training()
                    {

                        User_Id = item.userId,
                        TrainingId = item.treninkId,
                        DayOfWeek = item.dayOfWeek,
                        Date = item.date,
                        Type = 0,
                        Definition = definitionsList,
                        Response = responsesList
                    });
                }
            }
            return list;
        }
    }

    /*
    public List<ViewModels.Training> GetUserTrainignWeek(int _id, string _date)
    {
        using (var context = _trainingContext)
        {
            DateTime date = _date == null || _date == "" ? DateTime.Now : DateTime.Parse(_date);
            var list = new List<ViewModels.Training>();
            var query = from users in context.Users.Where(u => u.id == _id)
                join asociace_treninku in context.Asociace_Treninku.Where(a =>
                        a.user_id == _id && (DateTime)(object)a.date >= date.AddDays(-((int)date.DayOfWeek + 1)) &&
                        (DateTime)(object)a.date <= date.AddDays(+(7 - (int)date.DayOfWeek))) on users.id equals
                    asociace_treninku.user_id
                join trenink in context.Trenink on asociace_treninku.trenink_id equals trenink.id
                join user_response in context.Trenink_user_response on asociace_treninku.response_id equals user_response.id
                orderby asociace_treninku.date, asociace_treninku.type
                select new
                {
                    userId = users.id, dayOfWeek = new DataTransformation().GetDayOfWeekFromDate(asociace_treninku.date), date = asociace_treninku.date, treninkId = asociace_treninku.trenink_id,
                    treninkDefinice = trenink.definition, responseId = asociace_treninku.response_id,
                    responseDefinice = user_response.definition, type = asociace_treninku.type
                };

            foreach (var item in query)
            {
                List<ViewModels.TrainingDefinition> definitionsList = new List<ViewModels.TrainingDefinition>();
                foreach (var defItem in JsonSerializer.Deserialize<IList<ViewModels.TrainingDefinition>>(item.treninkDefinice))
                {
                    definitionsList.Add(new ViewModels.TrainingDefinition()
                    {
                        id = defItem.id,
                        col1 = defItem.col1,
                        col2 = defItem.col2,
                        col3 = defItem.col3,
                        col4 = defItem.col4
                    });
                }
                List<ViewModels.TrainingResponse> responsesList = new List<ViewModels.TrainingResponse>();
                foreach (ViewModels.TrainingResponse resItem in JsonSerializer.Deserialize<IList<ViewModels.TrainingResponse>>(item.responseDefinice))
                {
                    responsesList.Add(new ViewModels.TrainingResponse()
                    {
                        type = resItem.type
                    });
                }
                list.Add(new ViewModels.Training()
                {
                    User_Id = item.userId,
                    DayOfWeek = item.dayOfWeek,
                    Date = item.date,
                    Type = item.type,
                    DefinitionId = item.treninkId,
                    Definition = definitionsList,
                    ResponseId = item.responseId,
                    Response = responsesList
                });
            }
            return list;
        }
    }*/

    public Error CreateTraining(int userId, string date, int type, List<New_Training_Definition> definition, List<New_Training_User_Response> response)
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

                int id_response = context.Training_User_Response.Count() == 0 ? 0 : context.Training_User_Response.Select(r => r.id).Max();
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

                int id_training_association = context.Training_Association.Count() == 0 ? 0 : context.Training_Association.Select(a => a.id).Max() + 1;
                context.Training_Association.Add(new Training_Association()
                {
                    id = id_training_association,
                    userId = userId,
                    trainingId = _trainingId,
                    date = date,
                });

                int trainingNo =  context.Training_Definition.Count() == 0 ? 0 : context.Training_Definition.Select(a => a.id).Max();
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

                context.SaveChanges();

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

                foreach(var _item in _training){
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

                foreach(var _item in _trainingResponse){
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

                foreach(var _item in _training){
                    context.Entry(_item).State = EntityState.Deleted;
                    context.Training_Definition.Remove(_item);
                }

                var _trainingResponse = context.Training_User_Response.Where(r => r.trainingId == _trainingId);

                foreach(var _item in _trainingResponse){
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

                foreach(var _item in _trainingResponse){
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