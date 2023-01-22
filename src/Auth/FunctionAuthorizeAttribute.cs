using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace api.Auth;

public class FunctionAuthorizeAttribute : FunctionInvocationFilterAttribute
{
    public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
    {
        var httpRequest = GetHttpRequest(executingContext);
        var jwtToken = httpRequest.GetJwtBearerToken();
        var tokenProvider = httpRequest.HttpContext.RequestServices.GetRequiredService<ITokenProvider>();
        
        if (!tokenProvider.ValidateToken(jwtToken))
        {
            await RespondUnauthorizedAsync(httpRequest.HttpContext.Response);

            // Right now there is no way to stop the function from going further other than to throw an exception here
            throw new FunctionException($"{nameof(FunctionAuthorizeAttribute)} execution failed to validate the provided token.");
        }
    }

    private HttpRequest GetHttpRequest(FunctionExecutingContext context)
    {
        var httpRequest = context.Arguments.Values.FirstOrDefault(val => val is HttpRequest) as HttpRequest;
        return httpRequest;
    }

    private async Task RespondUnauthorizedAsync(HttpResponse response)
    {
        if (response.HasStarted)
            return;

        const string message = "Authentication failed. Please provide a valid authentication header.";

        response.ContentType = "text/plain";
        response.ContentLength = message.Length;
        response.StatusCode = (int) HttpStatusCode.Unauthorized;
        await response.WriteAsync(message);
        await response.Body.FlushAsync();
    }
}