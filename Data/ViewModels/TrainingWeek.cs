namespace Atletika_Denik_API.Data.ViewModels;

public class TrainingParams
{
    public List<New_Training_Definition>? Definitions { get; set; }
    public List<New_Training_User_Response>? Responses { get; set; }
}

public class Training_Association
{
    public int id { get; set; }
    public int? userId { get; set; }
    public string? trainingId { get; set; }
    public string? date { get; set; }
}

public class Training
{
    public string TrainingId { get; set; }
    public int User_Id { get; set; }
    public int? DayOfWeek { get; set; }
    public string? Date { get; set; }
    public List<Tag> Activity { get; set; }
    public List<Training_Definition>? Definition { get; set; }
    public List<Training_User_Response>? Response { get; set; }
    public int? Type { get; set; }
}

public class TrainingDay
{
    public int userId { get; set; }
    public string date { get; set; }
    public int dayOfWeek { get; set; }
    public int type { get; set; }
    public List<Activity2> activity { get; set; }
    public Training1 training { get; set; }

}

public class Activity1
{
    public string trainingId { get; set; }
    public List<Tag> activity { get; set; }
    public List<Tag_User_Response> activityResponse { get; set; }
}

public class Activity2{
    public string trainingId { get; set; }
    public string name { get; set; }
    public string color { get; set; }
    public string description { get; set; }
    public int response { get; set; }


}

public class Training1
{
    public string trainingId { get; set; }
    public List<Training_Definition> definition { get; set; }
    public List<Training_User_Response> response { get; set; }
}

public class Training_data
{
    public int User_Id { get; set; }
    public string? Date { get; set; }
    public int? Definition_Id { get; set; }
    public string? Definition { get; set; }
    public int? Response_Id { get; set; }
    public string? Response { get; set; }

}

public class Training_Definition
{
    public int id { get; set; }
    public string? trainingId { get; set; }
    public int rowId { get; set; }
    public string? col1 { get; set; }
    public string? col2 { get; set; }
    public string? col3 { get; set; }
    public string? col4 { get; set; }
}

public class Training_User_Response
{
    public int id { get; set; }
    public string? trainingId { get; set; }
    public int rowId { get; set; }
    public int response { get; set; }

}

public class TrainingDefinition
{
    public int id { get; set; }
    public string? col1 { get; set; }
    public string? col2 { get; set; }
    public string? col3 { get; set; }
    public string? col4 { get; set; }
}

public class TrainingResponse
{
    public int type { get; set; }
}

public class User
{
    public int id { get; set; }
    public string? userName { get; set; }
}

public class Users
{
    public int id { get; set; }
    public string? userName { get; set; }
    public string? userPassword { get; set; }
    public int? admin { get; set; }
}

public class UserList
{
    public int id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string yearOfBirth { get; set; }
    public string category { get; set; }
    public string sex { get; set; }
    public string trener { get; set; }
}

public class Asociace_Trener_Uzivatel
{
    public int id { get; set; }
    public int? trener_id { get; set; }
    public int? user_id { get; set; }
}

public class Asociace_Treninku
{
    public int id { get; set; }
    public int? user_id { get; set; }
    public int? trenink_id { get; set; }
    public string? date { get; set; }
    public int? response_id { get; set; }
    public int? type { get; set; }
}

public class Trenink
{
    public int id { get; set; }
    public string? definition { get; set; }
}

public class Trenink_user_response
{
    public int id { get; set; }
    public string? definition { get; set; }
}

public class New_Training_Definition
{
    public int id { get; set; }
    public string? col1 { get; set; }
    public string? col2 { get; set; }
    public string? col3 { get; set; }
    public string? col4 { get; set; }
}

public class New_Training_User_Response
{
    public int response { get; set; }
}