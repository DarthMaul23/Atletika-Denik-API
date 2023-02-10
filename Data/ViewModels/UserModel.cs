namespace Atletika_Denik_API.Data.ViewModels;

public class User_Info
{
    public int UserId { get; set; }
    public string Info { get; set; }
}

public class UserInfo
{
    public int UserId { get; set; }
    public Info Info { get; set; }
}

public class Info
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string City { get; set; }
    public string Adress { get; set; }
    public string Zip { get; set; }
    public string BirthDate { get; set; }
}