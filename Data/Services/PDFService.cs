using System.Runtime.CompilerServices;
using System.Reflection.Metadata;
using System.Text.Json;
using Atletika_Denik_API.Data.Models;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;
using TrainingDefinition = Atletika_Denik_API.Data.ViewModels.TrainingDefinition;
using System.Globalization;

namespace Atletika_Denik_API.Data.Services;

public class PDFService
{
    private Atletika_Denik_API.Data.Models.TrainingContext _trainingContext;
    Errors error = new Errors();

    public PDFService(Atletika_Denik_API.Data.Models.TrainingContext context)
    {
        _trainingContext = context;
    }

    public FileStreamResult GenerateTrainignWeekPDF(int _id, string _date)
    {
        var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Background(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.DefaultTextStyle(x => x.FontFamily("Arial"));

                    page.Header()
                        .PaddingBottom(10)
                        .Text("Treninky uživatele [" + _trainingContext.Users.First(x => x.id == _id).userName + "] na týden od " + GetDateAsText(_date, 2) + " do " + GetDateAsText(DateTime.Parse(_date).AddDays(5).ToString("yyyy-MM-dd"), 2))
                        .SemiBold().FontSize(12).FontColor(Colors.Black);

                    page.Content()
                        .Column(x =>
                        {

                            var data = GetUserTrainignWeek(_id, _date);
                            
                            foreach (var training in data)
                            {
                                x.Spacing(20);
                                x.Item().Border(1).Table(table =>
                                {

                                    IContainer DefaultCellStyle(IContainer container, string backgroundColor)
                                    {
                                        return container
                                            .Border(1)
                                            .BorderColor(Colors.Grey.Lighten1)
                                            .Background(backgroundColor)
                                            .PaddingVertical(5)
                                            .PaddingHorizontal(10)
                                            .AlignCenter()
                                            .AlignMiddle();
                                    }

                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().ColumnSpan(4).Element(CellStyle).Text(GetDateAsText(training.Date, 1));
                                        header.Cell().Element(CellStyle).Text("A");
                                        header.Cell().Element(CellStyle).Text("B");
                                        header.Cell().Element(CellStyle).Text("C");
                                        header.Cell().Element(CellStyle).Text("D");

                                        IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3);
                                    });

                                    foreach (var item in training.Definition)
                                    {
                                        table.Cell().Element(CellStyle).Text(item.col1);
                                        table.Cell().Element(CellStyle).Text(item.col2);
                                        table.Cell().Element(CellStyle).Text(item.col3);
                                        table.Cell().Element(CellStyle).Text(item.col4);

                                        IContainer CellStyle(IContainer container) => DefaultCellStyle(container, Colors.White).ShowOnce();
                                    }
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Strana ");
                            x.CurrentPageNumber();
                        });
                });
            });

        var stream = new MemoryStream();
        document.GeneratePdf(stream);

        stream.Position = 0;

        var result = new FileStreamResult(stream, "application/pdf")
        {
            FileDownloadName = "Hello.pdf",
        };

        return result;
    }

    private string GetDateAsText(string _date, int type)
    {
        string newDate = "";

        DateTime date;

        if (DateTime.TryParseExact(_date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            DataTransformation _trans = new DataTransformation();
            if (type == 1){
                newDate = _trans.GetDayOfWeek((int)date.DayOfWeek - 1) + " " + date.Day + ". " + _trans.GetMonth(date.Month - 1) + " " + date.Year;
            }else 
                if(type == 2){
                    newDate = date.Day + ". " + _trans.GetMonth(date.Month - 1) + " " + date.Year;
            }
        }
        else
        {
            Console.WriteLine("Invalid date format.");
        }
        return newDate;
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
                            userId = users.id,
                            dayOfWeek = new DataTransformation().GetDayOfWeekFromDate(asociace_treninku.date),
                            date = asociace_treninku.date,
                            treninkId = asociace_treninku.trenink_id,
                            treninkDefinice = trenink.definition,
                            responseId = asociace_treninku.response_id,
                            responseDefinice = user_response.definition,
                            type = asociace_treninku.type
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
            if (!Enumerable.Range(1, 4).Contains(type))
            {
                return error.GetError("Ex03");
            }
            else
            if (context.Asociace_Treninku.FirstOrDefault(a => a.user_id == userId && a.date == date && a.type == type) != null)
            {
                return error.GetError("Ex02");
            }
            else
            {
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