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

public class Tag_Detail
{
    public string id { get; set; }
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

public class NewTagDetail{
    public int id {get; set;}
    public string repetition { get; set; }
    public int weekDay { get; set; }
    public int column { get; set; }
    public string dateFrom { get; set; }
    public string dateTo { get; set; }
}