using FlowJudge.Common.Http.User;
using System.Security.Claims;
using System.Text.Json;

namespace FlowJudge.Common.Http.Extensions
{
    public static class JwtExtensions
    {
        public static UserContext GetUserContext(this ClaimsPrincipal user)
        {
            if (!user.TryGetUserContext(out var context) || context == null)
            {
                throw new InvalidOperationException("User context could not be retrieved from JWT claims.");
            }
            return context;
        }

        public static bool TryGetUserContext(this ClaimsPrincipal user, out UserContext? context)
        {
            var idString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = user.FindFirst(KeycloakClaimTypes.Name)?.Value;
            var username = user.FindFirst(KeycloakClaimTypes.PreferredUsername)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var locale = user.FindFirst(KeycloakClaimTypes.Locale)?.Value;
            var realmAccessSerialized = user.FindFirst(KeycloakClaimTypes.RealmAccess)?.Value;

            var isId = Guid.TryParse(idString, out var id);

            if (!isId
                || string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(locale)
                || string.IsNullOrWhiteSpace(realmAccessSerialized))
            {
                context = null;
                return false;
            }

            var realmAccess = JsonSerializer.Deserialize<RealmAccess>(realmAccessSerialized);

            context = new UserContext
            {
                Id = id,
                Name = name,
                Username = username,
                Email = email,
                Locale = locale,
                RealmRoles = realmAccess?.Roles?.Select(r => new RealmRole() { Name = r }) ?? Enumerable.Empty<RealmRole>()
            };

            return true;
        }

        private class RealmAccess
        {
            public IEnumerable<string>? Roles { get; set; }
        }
    }
}
