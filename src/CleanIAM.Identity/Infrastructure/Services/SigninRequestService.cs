using System.Net;
using System.Security.Claims;
using CleanIAM.Identity.Application.Interfaces;
using CleanIAM.Identity.Core.Requests;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;

namespace CleanIAM.Identity.Infrastructure.Services;

public class SigninRequestService(IDocumentSession documentSession) : ISigninRequestService
{
    public async Task SaveAsync(SigninRequest request)
    {
        //Only one request per user is allowed
        var oldRequest = await documentSession.Query<SigninRequest>()
            .FirstOrDefaultAsync(r => r.UserId == request.UserId);
        if (oldRequest is not null)
            documentSession.Delete(oldRequest);
        
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