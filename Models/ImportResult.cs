namespace Models;

public class ImportResult
{
    public ImportStatus Status { get; set; }
    public List<string> Errors { get; set; } = new();
}