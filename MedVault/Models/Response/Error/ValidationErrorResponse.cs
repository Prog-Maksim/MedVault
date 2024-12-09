namespace MedVault.Models.Response.Error;

public class ValidationErrorResponse
{
    public string type { get; set; }
    public string title { get; set; }
    public int status { get; set; }
    public Dictionary<string, string[]> errors { get; set; }
    public string traceId { get; set; }
}