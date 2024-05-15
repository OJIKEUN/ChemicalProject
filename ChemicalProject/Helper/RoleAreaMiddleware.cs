using System.Security.Claims;

namespace ChemicalProject.Helper
{
    public class RoleAreaMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAreaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var windowsAccount = context.User.Identity.Name;
            if (!string.IsNullOrEmpty(windowsAccount))
            {
                var user = await userService.GetUserByWindowsAccount(windowsAccount);

                if (user != null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, user.Role.Name),
                };

                    if (user.Area != null)
                    {
                        claims.Add(new Claim("Area", user.Area.Name));
                    }

                    var appIdentity = new ClaimsIdentity(claims);
                    context.User.AddIdentity(appIdentity);
                }
            }

            await _next(context);
        }
    }
}
