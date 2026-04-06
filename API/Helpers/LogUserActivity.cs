
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

            var username = resultCtx.HttpContext.User.GetUsername();
            var repo = resultCtx.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByUsernameAsync(username); // better way ?
            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}