using Atletika_Denik_API.Data.Services;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Atletika_Denik_API.Controllers;

public class PDFController : ControllerBase
{
    private PDFService _PDFService;

    public PDFController(PDFService PDFService)
    {
        _PDFService = PDFService;
    }

    [HttpGet("get-Training-WeekPDF")]
    public IActionResult GetUserTrainignWeek(int id = 1, string date = "2022-12-04")
    {
        return _PDFService.GenerateTrainignWeekPDF(id, date);
    }

}