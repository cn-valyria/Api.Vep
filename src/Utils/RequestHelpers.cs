using System.Linq;
using Microsoft.AspNetCore.Http;

namespace api.Utils;

public static class RequestHelpers
{
    public static string GetJwtBearerToken(this HttpRequest request)
    {
        var authHeader = request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
            return null;
        
        var authHeaderWords = authHeader.Split(" ");
        if (authHeaderWords.Length != 2)
            return null;

        return authHeaderWords.Last();
    }
}