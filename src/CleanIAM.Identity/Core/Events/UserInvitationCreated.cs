namespace CleanIAM.Identity.Core.Events;

/// <summary>
/// Event that is published when a user invitation is created and sent via email.
/// </summary>
/// <param name="InvitaionId">Invitation request id</param>
/// <param name="Id">Id of the user</param>
/// <param name="email">Email of the user</param>
/// <param name="FirstName">First name of the user</param>
/// <param name="LastName">Last name of the user</param>
public record UserInvitationCreated(Guid InvitaionId, Guid Id, string email, string FirstName, string LastName);