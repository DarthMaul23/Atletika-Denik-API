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
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BirthDate { get; set; }
    public int Sex { get; set; }
    public string Email { get; set; }
    public Adress Adress { get; set; }
    public Phone Phone { get; set; }
    public int Category { get; set; }
    public int Discipline { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
}

public class Adress
{
    public string City { get; set; }
    public string Street { get; set; }
    public string HouseNo { get; set; }
    public string ZipCode { get; set; }
}

public class Phone{
    public string UserPhone {get; set; }
    public string FathersPhone {get; set; }
    public string MothersPhone {get; set; }
}