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

    [HttpPost("create-New-Activity")]
    public IActionResult CreateNewActivity(NewTag tag,[FromBody] List<NewTagUserSettings> details)
    {
        _activityService.CreateNewActivity(tag, details);
        return Ok();
    }

}