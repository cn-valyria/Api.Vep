using System.Threading.Tasks;
using api.Contracts;
using Repository;
using Repository.DTO;

namespace api.Auth;

public class AuthenticationProvider : IAuthenticationProvider
{
    private readonly IAuthorizationRepository _authorizationRepository;
    
    public AuthenticationProvider(IAuthorizationRepository authorizationRepository)
    {
        _authorizationRepository = authorizationRepository;
    }

    public async Task<bool> IsAuthenticated(AuthorizeUserRequest request)
    {
        if (!RequestIsValid(request))
            return false;

        var authorizedUser = await GetAuthorizedAccount(request);
        return authorizedUser != null;
    }

    public async Task<AuthorizedAccount> GetAuthorizedAccount(AuthorizeUserRequest request)
    {
        AuthorizedAccount authorizedUser = null;
        if (!RequestIsValid(request))
            return authorizedUser;

        if (!string.IsNullOrEmpty(request.NationId) && int.TryParse(request.NationId, out var nationId))
            authorizedUser = await _authorizationRepository.Authorize(nationId, request.UniqueCode);
        else
            authorizedUser = await _authorizationRepository.Authorize(request.RulerName, request.UniqueCode);

        return authorizedUser;
    }

    private bool RequestIsValid(AuthorizeUserRequest request)
    {
        if (string.IsNullOrEmpty(request.UniqueCode))
            return false;
        if (string.IsNullOrEmpty(request.NationId) && string.IsNullOrEmpty(request.RulerName))
            return false;

        return true;
    }
}

public interface IAuthenticationProvider
{
    Task<bool> IsAuthenticated(AuthorizeUserRequest account);
    Task<AuthorizedAccount> GetAuthorizedAccount(AuthorizeUserRequest request);
}