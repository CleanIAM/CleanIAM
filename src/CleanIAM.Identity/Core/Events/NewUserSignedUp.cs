using CleanIAM.SharedKernel.Core;

namespace CleanIAM.Identity.Core.Events;
/// <summary>
/// Represents an event that is triggered when a user signed up.
/// </summary>
/// <param name="Id">Id of the signed-up user</param>
/// <param name="Email">Email of the signed-up user</param>
/// <param name="FirstName">First name of the signed-up user</param>
/// <param name="LastName">Last name of the signed-up user</param>
/// <param name="Roles">Roles of the signed-up user</param>
public record NewUserSignedUp(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);