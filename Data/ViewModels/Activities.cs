using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atletika_Denik_API.Data.ViewModels;

public class ReturnItems
{
    public List<Tag> tags { get; set; }
    public int noRecords { get; set; }
}

public class CreateTag
{
    public NewTag tag { get; set; }
    public List<NewTagUserSettings> details { get; set; }
    public List<ActivityDefinitionCreate> activities { get; set; }
}

public class Tag_Association
{
    public string id { get; set; }
    public string tagId { get; set; }
    public int userId { get; set; }
}

public class Tag
{
    public string id { get; set; }
    public string name { get; set; }
    public string color { get; set; }
    public string description { get; set; }

    public ICollection<Tag_Activities_Association> TagAssociations { get; set; }
}

public class Tag_Details
{
    public string id { get; set; }
    public string tagAsocId { get; set; }
    public string date { get; set; }
    public string created { get; set; }

    public ICollection<Tag_Activities_User_Responses> UserResponses { get; set; }
}

public class Tag_User_Response
{
    public string id { get; set; }
    public int response { get; set; }
}

public class Tag_User_Settings
{
    public string id { get; set; }
    public string tagAsocId { get; set; }
    public string repetition { get; set; }
    public int weekDay { get; set; }
    public int col { get; set; }
    public string dateFrom { get; set; }
    public string dateTo { get; set; }
}

public class EditTag
{
    public string id { get; set; }
    public string name { get; set; }
    public string color { get; set; }
    public string description { get; set; }
    public List<EditTagSettings> settings { get; set; }
}

public class EditTagSettings
{
    public int id { get; set; }
    public string tagAsocId { get; set; }
    public string repetition { get; set; }
    public int weekDay { get; set; }
    public int col { get; set; }
    public string dateFrom { get; set; }
    public string dateTo { get; set; }
}

public class NewTag
{
    public string name { get; set; }
    public string color { get; set; }
    public string description { get; set; }
}

public class NewTagUserSettings
{
    public int id { get; set; }
    public string repetition { get; set; }
    public int weekDay { get; set; }
    public int column { get; set; }
    public string dateFrom { get; set; }
    public string dateTo { get; set; }
}

public class TagDetail
{
    public string id { get; set; }
    public string name { get; set; }
    public string color { get; set; }
    public string description { get; set; }
}

public class Tag_Activities_Definitions
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Definition { get; set; }
    public int Order { get; set; }
    public ICollection<Tag_Activities_Association> TagAssociations { get; set; }
    public ICollection<Tag_Activities_User_Responses> UserResponses { get; set; }
}

public class Tag_Activities_User_Responses
{
    public string Id { get; set; }
    public int UserId { get; set; }
    public string TagDetailId { get; set; }
    public string ActivityDefinitionId { get; set; }
    public int Response { get; set; }
    public string Date { get; set; }
    public Tag_Activities_Definitions ActivityDefinition { get; set; }
    public Tag_Details TagDetail { get; set; }
}

public class Tag_Activities_Association
{
    public string Id { get; set; }
    public string TagId { get; set; }
    public string ActivityDefinitionId { get; set; }
    public Tag Tag { get; set; }
    public Tag_Activities_Definitions ActivityDefinition { get; set; }
}

public class CreateActivityDefinitionRequest
{
    public Tag_Activities_Definitions Definition { get; set; }
    public List<Tag_Activities_User_Responses> Responses { get; set; }
    public List<int> UserIds { get; set; }
}

public class TagActivitiesDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Definition { get; set; }
    public int Order { get; set; }
}

public class ActivityDefinitionCreate
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Definition { get; set; }
    public int Order {get; set;}
    public UserResponseDto Response { get; set; }
}

public class ActivityDefinitionDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Definition { get; set; }
    public UserResponseDto Response { get; set; }
}

public class UserResponseDto
{
    public string Id { get; set; }
    public int Response { get; set; }
}

