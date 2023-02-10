using Microsoft.EntityFrameworkCore;

namespace Atletika_Denik_API.Data.ViewModels;

public class TrainingParams
{
    public List<TrainingDefinition>? Definitions { get; set; }
    public List<TrainingResponse>? Responses { get; set; }
}

public class Training
{
    public int User_Id { get; set; }
    public string? Date { get; set; }
    public int? DefinitionId { get; set; }
    public List<TrainingDefinition>? Definition { get; set; }
    public int? ResponseId { get; set; }
    public List<TrainingResponse>? Response { get; set; }
    public int? Type { get; set; }
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