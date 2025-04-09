using Identity.Application.Interfaces;
using Identity.Core;
using Marten;

namespace Identity.Infrastructure.Services;

public class SigninRequestService(IDocumentSession documentSession) : ISigninRequestService
{
    public async Task SaveAsync(SigninRequest request)
    {
        documentSession.Store(request);
        await documentSession.SaveChangesAsync();
    }

    public async Task<SigninRequest?> GetAsync(Guid id)
    {
        return await documentSession.LoadAsync<SigninRequest>(id);
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