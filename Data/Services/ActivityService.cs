using System.Net.Mime;
using System.Text.Json;
using Atletika_Denik_API.Data.Models;
using Atletika_Denik_API.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using TrainingDefinition = Atletika_Denik_API.Data.ViewModels.TrainingDefinition;

namespace Atletika_Denik_API.Data.Services;

public class ActivityService
{
    private Atletika_Denik_API.Data.Models.ActivitiesContext _activiesContext;
    Errors error = new Errors();
    public ActivityService(Atletika_Denik_API.Data.Models.ActivitiesContext activiesContext)
    {
        _activiesContext = activiesContext;
    }

    public ReturnItems GetListOfActivities(int userId, int pageNo, int itemsPerPage, string searchTerm)
    {
        using (var context = _activiesContext)
        {
            var query = context.Tag
                .Where(tag => tag.name.Contains(searchTerm == null? "" : searchTerm))
                .OrderBy(tag => tag.id)
                .Skip((pageNo - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            var _noRecords = context.Tag.Where(tag => tag.name.Contains(searchTerm == null? "" : searchTerm)).Count();

            return new ReturnItems(){ tags = query, noRecords = _noRecords};
        }
    }

}