namespace Atletika_Denik_API.Data.Models;

public class TrainingWeek
{
    public int User_Id { get; set; }
    public string? Date { get; set; }
    public List<TrainingDefinition>? Definition { get; set; }
}

public class TrainingWeek_data
{
    public int User_Id { get; set; }
    public string? Date { get; set; }
    public string? Definition { get; set; }
}

public class TrainingDefinition
{
    public string? col1 { get; set; }
    public string? col2 { get; set; }
    public string? col3 { get; set; }
    public string? col4 { get; set; }
}