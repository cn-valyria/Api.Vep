namespace api.Contracts;

public class AuthorizeUserRequest
{
    public string NationId { get; set; }
    public string RulerName { get; set; }
    public string UniqueCode { get; set;}
}