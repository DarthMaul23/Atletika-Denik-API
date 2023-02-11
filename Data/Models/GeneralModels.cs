namespace Atletika_Denik_API.Data.Models;

public class GeneralModels
{

}

public class UserLogin
{
    public string userName { get; set; }
    public string userPassword { get; set; }
}

public class UserLoginResponse
{
    public bool loggedin { get; set; }
    public Guid? token {get; set; }
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