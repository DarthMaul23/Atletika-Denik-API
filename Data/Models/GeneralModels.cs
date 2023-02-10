namespace Atletika_Denik_API.Data.Models;

public class GeneralModels
{
    
}

public class Users
{
    public int id { get; set; }
    public string? userName { get; set; }
    public string? userPassword { get; set; }
    public int? admin { get; set; }
}

public class UsersCreate
{
    public string? userName { get; set; }
    public string? userPassword { get; set; }
    public int? admin { get; set; }
}