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
    public IActionResult GetUserTrainignWeek(int id = 1, string date = "2023-04-04")
    {
        return Ok(_trainingService.GetUserTrainignWeek(id, date));
    }

    [HttpPost("create-Training")]
    public async Task<IActionResult> CreateTraining(int userId, string date, int type, [FromBody] TrainingParams _data)
    {
        var data = await _trainingService.CreateTrainingAsync(userId, date, type, _data.Definitions, _data.Responses);
        return Ok(data);
    }
    
    [HttpPut("update-Training")]
    public IActionResult updateTrainign(string treninkId, int type, [FromBody] TrainingParams _data)
    {
        _trainingService.UpdateTraining(treninkId, type, _data.Definitions, _data.Responses);
        return Ok();
    }
    
    [HttpDelete("delete-Training")]
    public IActionResult DeleteTrainign(string treninkId)
    {
        return Ok(_trainingService.DeleteTraining(treninkId));
    }
    
    [HttpPut("update-Training-Response/treninkResponseId/{treninkResponseId}/response/{response}")]
    public IActionResult UpdateTrainingResponse(int treninkResponseId, int response)
    {
        _trainingService.UpdateTrainingResponse(treninkResponseId, response);
        return Ok();
    }

}