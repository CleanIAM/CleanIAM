using System.Net;
using System.Security.Claims;
using Identity.Application.Interfaces;
using Identity.Core.Requests;
using Marten;
using SharedKernel.Infrastructure.Utils;

namespace Identity.Infrastructure.Services;

public class SigninRequestService(IDocumentSession documentSession) : ISigninRequestService
{
    public async Task SaveAsync(SigninRequest request)
    {
        //TODO: Only one per user should be allowed
        documentSession.Store(request);
        await documentSession.SaveChangesAsync();
    }

    public async Task<SigninRequest?> GetAsync(Guid id)
    {
        return await documentSession.LoadAsync<SigninRequest>(id);
    }

    public async Task<Result<SigninRequest>> GetFromClaimsAsync(ClaimsPrincipal user)
    {
        if (!(user.Identity is { IsAuthenticated: true }))
            return Result.Error("User not authenticated", HttpStatusCode.Unauthorized);

        var requestIdClaim = user.Claims.FirstOrDefault(x => x.Type == IdentityConstants.SigninRequestClaimName);
        if (requestIdClaim == null)
            return Result.Error("Request ID claim not found", HttpStatusCode.BadRequest);

        if (!Guid.TryParse(requestIdClaim.Value, out var requestId))
            return Result.Error("Request ID claim is not valid", HttpStatusCode.BadRequest);

        var request = await GetAsync(requestId);
        if (request == null)
            return Result.Error("Request not found", HttpStatusCode.NotFound);

        return Result.Ok(request);
    }


    public async Task<SigninRequest?> DeleteAsync(Guid id)
    {
        var requestToDelete = await GetAsync(id);
        if (requestToDelete == null)
            return null;
        documentSession.Delete(requestToDelete);

        await documentSession.SaveChangesAsync();
        return requestToDelete;
    }

    public async Task<SigninRequest?> UpdateAsync(SigninRequest request)
    {
        documentSession.Update(request);
        await documentSession.SaveChangesAsync();

        return request;
    }

    public Dictionary<string, object?> CreateOidcQueryObject(SigninRequest request)
    {
        return new Dictionary<string, object?>
        {
            { "client_id", request.OidcRequest.ClientId },
            { "redirect_uri", request.OidcRequest.RedirectUri },
            { "response_type", request.OidcRequest.ResponseType },
            { "scope", request.OidcRequest.Scope },
            { "state", request.OidcRequest.State },
            { "code_challenge", request.OidcRequest.CodeChallenge },
            { "code_challenge_method", request.OidcRequest.CodeChallengeMethod }
        };
    }
}