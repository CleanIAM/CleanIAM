namespace ManagementPortal.Api.Controllers.Models.Requests.User;

public class UpdateUserSimpleRequest
{
    /// <summary>
    /// First name of the user
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// last name of the user
    /// </summary>
    public required string LastName { get; set; }
}