using System.Security.Claims;
using Identity.Core.Requests;
using SharedKernel.Infrastructure;

namespace Identity.Application.Interfaces;

public interface ISigninRequestService
{
    /// <summary>
    /// Save the request to the storage
    /// </summary>
    /// <param name="request">New request to be saved</param>
    /// <returns></returns>
    Task SaveAsync(SigninRequest request);

    /// <summary>
    /// Get the request from the storage
    /// </summary>
    /// <param name="id">ID the request</param>
    /// <returns>Request if found, null if not</returns>
    Task<SigninRequest?> GetAsync(Guid id);

    /// <summary>
    /// Ger signin request from the claims of a currently authenticated user
    /// </summary>
    /// <param name="user">User claims</param>
    /// <remarks>This should be used in the later seps of the flow like MFA or email verification</remarks>
    /// <returns>Request if user authenticated</returns>
    Task<Result<SigninRequest>> GetFromClaimsAsync(ClaimsPrincipal user);

    /// <summary>
    /// Delete the request from the storage
    /// </summary>
    /// <param name="id">ID of the request to delete</param>
    /// <returns>Deleted request or null if no request to delete was found</returns>
    Task<SigninRequest?> DeleteAsync(Guid id);

    /// <summary>
    /// Update the request in the storage
    /// </summary>
    /// <param name="request">Updated request</param>
    /// <returns>Updated request, null if no request to update was found</returns>
    Task<SigninRequest?> UpdateAsync(SigninRequest request);

    /// <summary>
    /// Create oidc query string from the request
    /// </summary>
    /// <param name="request">Request to make query string from</param>
    /// <returns>Query object</returns>
    Dictionary<string, object?> CreateOidcQueryObject(SigninRequest request);
}