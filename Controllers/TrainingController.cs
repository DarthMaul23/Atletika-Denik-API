using Atletika_Denik_API.Data.Services;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Atletika_Denik_API.Controllers;

public class TrainingController : ControllerBase
{
    private TrainingService _trainingService;

    public TrainingController(TrainingService trainingService)
    {
        _trainingService = trainingService;
    }

    [HttpGet("get-Training-Week")]
    public IActionResult GetUserTrainignWeek(int id = 1, string date = "2022-12-04")
    {
        return Ok(_trainingService.GetUserTrainignWeek(id, date));
    }

    [HttpPost("create-Training")]
    public IActionResult CreateTrainign(int userId, string date, int type, [FromBody] TrainingParams _data)
    {
        return Ok(_trainingService.CreateTraining(userId, date, type, _data.Definitions, _data.Responses));
    }
    
    [HttpPut("update-Training")]
    public IActionResult updateTrainign(int treninkId, int type, [FromBody] TrainingParams _data)
    {
        _trainingService.UpdateTraining(treninkId, type, _data.Definitions, _data.Responses);
        return Ok();
    }
    
    [HttpDelete("delete-Training")]
    public IActionResult DeleteTrainign(int treninkId)
    {
        return Ok(_trainingService.DeleteTraining(treninkId));
    }
    
    [HttpPut("update-Training-Response")]
    public IActionResult UpdateTrainingResponse(int id, [FromBody] List<TrainingResponse> response)
    {
        _trainingService.UpdateTrainingResponse(id, response);
        return Ok();
    }

}