
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultCtx = await next(); // after the request
            
            if(!resultCtx.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultCtx.HttpContext.User.GetUserId();
            var repo = resultCtx.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}