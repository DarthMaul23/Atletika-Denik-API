using Atletika_Denik_API.Data.Services;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Atletika_Denik_API.Controllers;

public class ActivityController : ControllerBase
{
    private ActivityService _activityService;

    public ActivityController(ActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpGet("get-Activity-List")]
    public IActionResult GetActivityList(int userId = 0, int pageNo = 1, int itemsPerPage = 50, string search = "")
    {
        return Ok(_activityService.GetListOfActivities(userId, pageNo, itemsPerPage, search));
    }

    [HttpGet("get-Activity-Detail")]
    public IActionResult GetActivityDetail(string tagId)
    {
        return Ok(_activityService.GetActivityDetal(tagId));
    }

    [HttpPost("create-New-Activity")]
    public async Task<IActionResult> CreateNewActivityAsync(NewTag tag,[FromBody] List<NewTagUserSettings> details)
    {
        await _activityService.CreateNewActivity(tag, details);
        return Ok();
    }

}