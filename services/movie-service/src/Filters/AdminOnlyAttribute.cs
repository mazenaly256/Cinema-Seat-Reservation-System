using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace movie_service.Filters;

public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var role = context.HttpContext.Request.Headers["X-User-Role"].ToString();

        if (role != "Admin")
        {
            context.Result = new ObjectResult("Unauthorized to access the endpoint, access is allowed for admins only.")
            {
                StatusCode = 403
            };
        }
    }
}
