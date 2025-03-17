using Identity.Core;
using Identity.Infrastructure;
using Marten;

namespace Identity.Application.Services;

public class SigninRequestService(IDocumentSession documentSession): ISigninRequestService
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
        if(requestToDelete == null)
        {
            return null;
        }
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
    
    public Dictionary<string, object> CreateOIDCQueryObject(SigninRequest request)
    {
        return new Dictionary<string, object>
        {
            { "client_id", request.oidcRequest.ClientId },
            { "redirect_uri", request.oidcRequest.RedirectUri },
            { "response_type", request.oidcRequest.ResponseType },
            { "scope", request.oidcRequest.Scope },
            { "state", request.oidcRequest.State },
            { "code_challenge", request.oidcRequest.CodeChallenge },
            { "code_challenge_method", request.oidcRequest.CodeChallengeMethod }
        };
    }
}