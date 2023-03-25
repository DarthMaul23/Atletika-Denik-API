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
    }

    public Error CreateTraining(int userId, string date, int type, List<TrainingDefinition> definition, List<TrainingResponse> response)
    {
        using (var context = _trainingContext)
        {
            if(!Enumerable.Range(1,4).Contains(type)){
                return error.GetError("Ex03");
            }else 
            if (context.Asociace_Treninku.FirstOrDefault(a => a.user_id == userId && a.date == date && a.type == type) != null)
            {
                return error.GetError("Ex02");
            }else{
                int id_trenink = context.Trenink.Select(t => t.id).Max() + 1;
                int id_response = context.Trenink_user_response.Select(r => r.id).Max() + 1;
                context.Trenink_user_response.Add(new Trenink_user_response()
                {
                    id = id_response,
                    definition = JsonSerializer.Serialize(response)
                });
                context.Asociace_Treninku.Add(new Asociace_Treninku()
                {
                    id = context.Asociace_Treninku.Select(a => a.id).Max() + 1,
                    date = date,
                    user_id = userId,
                    trenink_id = id_trenink,
                    response_id = id_response,
                    type = type
                });
                context.Trenink.Add(new Trenink()
                {
                    id = id_trenink,
                    definition = JsonSerializer.Serialize(definition)
                });
                context.SaveChanges();
                return error.GetError("S01");
            }
        }
    }
    
    public void UpdateTraining(int treninkId, int type, List<TrainingDefinition> definition, List<TrainingResponse> response)
    {
        using (var context = _trainingContext)
        {
            var trainingAsociaction = context.Asociace_Treninku.FirstOrDefault(a => a.trenink_id == treninkId && a.type == type);
                
            var trainingDefinition = context.Trenink.FirstOrDefault(t => t.id == trainingAsociaction.trenink_id);
            trainingDefinition.definition = JsonSerializer.Serialize(definition);
            context.Entry(trainingDefinition).State = EntityState.Modified;
            context.Update(trainingDefinition);

            var trainingResponse = context.Trenink_user_response.FirstOrDefault(r => r.id == trainingAsociaction.response_id);
            trainingResponse.definition = JsonSerializer.Serialize(response);
            context.Entry(trainingResponse).State = EntityState.Modified;
            context.Update(trainingResponse);

            context.SaveChanges();
        }
    }
    
    public Error DeleteTraining(int treninkId)
    {
        using (var context = _trainingContext)
        {
            var trainingAsociaction = context.Asociace_Treninku.FirstOrDefault(a => a.trenink_id == treninkId);
            if (trainingAsociaction == null)
            {
                return error.GetError("Ex04");
            }
            else
            {
                context.Entry(trainingAsociaction).State = EntityState.Deleted;
                context.Remove(trainingAsociaction);

                var trainingDefinition =
                    context.Trenink.FirstOrDefault(t => t.id == trainingAsociaction.trenink_id);
                context.Entry(trainingDefinition).State = EntityState.Deleted;
                context.Remove(trainingDefinition);

                var trainingResponse =
                    context.Trenink_user_response.FirstOrDefault(r => r.id == trainingAsociaction.response_id);
                context.Entry(trainingResponse).State = EntityState.Deleted;
                context.Remove(trainingResponse);

                context.SaveChanges();
                    
                return error.GetError("S02");
            }
        }
    }

    public void UpdateTrainingResponse(int id, List<TrainingResponse> responseJSON)
    {
        using (var context = _trainingContext)
        {
            var response = context.Trenink_user_response.FirstOrDefault(r => r.id == id);
            response.definition = JsonSerializer.Serialize(responseJSON);
            context.Entry(response).State = EntityState.Modified;
            context.Update(response);
            context.SaveChanges();
        }
    }
    
}