namespace Atletika_Denik_API.Data.ViewModels;

public class ReturnItems{
    public List<Tag> tags { get; set; }
    public int noRecords { get; set; }
}

public class Tag_Association
{
    public int id { get; set; }
    public string tagId { get; set; }
    public int userId { get; set; }
}

public class Tag
{
    public string id { get; set; }
    public string name { get; set; }
    public string color { get; set; }
    public string description { get; set; }
}