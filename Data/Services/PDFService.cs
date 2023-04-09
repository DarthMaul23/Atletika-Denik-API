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
            FileDownloadName = "trenikovy_plan.pdf",
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
            if (type == 1)
            {
                newDate = _trans.GetDayOfWeek((int)date.DayOfWeek - 1) + " " + date.Day + ". " + _trans.GetMonth(date.Month - 1) + " " + date.Year;
            }
            else
                if (type == 2)
            {
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
}