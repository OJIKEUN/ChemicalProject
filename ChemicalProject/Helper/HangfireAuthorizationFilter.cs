using Hangfire.Dashboard;

namespace ChemicalProject.Helper
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var allowedRoles = new[] { "UserAdmin" };

            return httpContext.User.Identity.IsAuthenticated &&
                   allowedRoles.Any(role => httpContext.User.IsInRole(role));
        }
    }
}