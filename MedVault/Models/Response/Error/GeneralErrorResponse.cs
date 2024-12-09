namespace MedVault.Models.Response.Error;

public class GeneralErrorResponse
{
    public string message { get; set; }
    public bool success { get; set; }
    public int errorCode { get; set; }
    public string error { get; set; }
}