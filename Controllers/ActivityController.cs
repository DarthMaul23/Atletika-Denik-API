using Atletika_Denik_API.Data.Services;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atletika_Denik_API.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityService _activityService;

        public ActivityController(ActivityService activityService)
        {
            _activityService = activityService;
        }

        
        [HttpPost("create-New-Activity")]
        public async Task<IActionResult> CreateNewActivityAsync([FromBody] CreateTag _newTag)
        {
            await _activityService.CreateNewActivity(_newTag.tag, _newTag.details, _newTag.activities);
            return Ok();
        }

        [HttpGet("activities")]
        public IActionResult GetActivityList(int userId = 0, int pageNo = 1, int itemsPerPage = 50, string? search = "")
        {
            return Ok(_activityService.GetListOfActivities(userId, pageNo, itemsPerPage, search));
        }

        [HttpGet("activities/{tagAsociationId}/description")]
        public IActionResult GetActivityDescription(string tagAsociationId)
        {
            return Ok(_activityService.GetActivityDescription(tagAsociationId));
        }

        [HttpGet("activities/{tagId}/detail")]
        public IActionResult GetActivityDetail(string tagId)
        {
            return Ok(_activityService.GetActivityDetail(tagId));
        }

        [HttpGet("activities/definition/tagAsociaiton/{tagAsociationId}/user/{userId}/date/{date}")]
        public IActionResult GetActivityDefinitionByTagAssociationId(string tagAsociationId = "1a2b3c4d-5e6f-7a8b-9c1d-2e3f4a5b6c7d", int userId = 1, string date = "")
        {
            var result = _activityService.GetActivityDefinitionByTagAssociationId(tagAsociationId, userId, date);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("activities/setResponse/responseId/{responseId}/responseValue/{responseValue}")]
        public IActionResult SetActivityResponse(string responseId, int responseValue)
        {
            _activityService.setActivityResponse(responseId, responseValue);
            return Ok();
        }
    }
}
