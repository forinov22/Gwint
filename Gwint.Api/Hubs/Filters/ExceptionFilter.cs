using Microsoft.AspNetCore.SignalR;

namespace Gwint.Api.Hubs.Filters
{
    public class ExceptionFilter(ILogger<ExceptionFilter> logger) : IHubFilter
    {
        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
        {
            logger.LogInformation($"Calling hub method '{invocationContext.HubMethodName}'");
            try
            {
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception calling '{invocationContext.HubMethodName}': {ex}", invocationContext.HubMethodName, ex);
                throw new HubException(ex.Message);
            }
        }
    }
}
