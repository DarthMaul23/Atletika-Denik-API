using System.Text.Json;

namespace Atletika_Denik_API.Data.Models;

public class ErrorList
{
    public List<Error> Errors { get; set; }
}

public class Error
{
    public string type {get; set;}
    public Value value { get; set; }
}

public class Value
{
    public string langCZ { get; set; }
    public string langEN { get; set; }
}

public class Errors
{
    ErrorList errors = new ErrorList();
    string json = File.ReadAllText("Data/InternalSourceData/Errors.json");
    public Errors()
    {
        setupErrorsFromJson();
    }

    private void setupErrorsFromJson()
    {
        errors = JsonSerializer.Deserialize<ErrorList>(json);
    }

    public Error GetError(string type)
    {
        return errors.Errors.Find(e => e.type == type);
    }
    
}