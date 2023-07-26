using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace MarketKhoone.Common.IdentityToolkit
{
    public static class IdentityExtensions
    {
        public static void AddErrorsFromResult(this ModelStateDictionary modelState, IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(string.Empty, error.Description);
            }
        }

        public static List<string> GetModelStateErrors(this ModelStateDictionary modelState)
        {
            return modelState.Keys.SelectMany(k => modelState[k].Errors)
                .Select(m => m.ErrorMessage).OrderByDescending(x => x).ToList();
        }
        public static string DumpErrors(this IdentityResult result, bool useHtmlNewLine = false)
        {
            var results = new StringBuilder();
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    var errorDescription = error.Description;
                    if (string.IsNullOrWhiteSpace(errorDescription))
                    {
                        continue;
                    }

                    if (!useHtmlNewLine)
                    {
                        results.AppendLine(errorDescription);
                    }
                    else
                    {
                        results.Append(errorDescription).AppendLine("<br/>");
                    }
                }
            }

            return results.ToString();
        }
        /// <summary>
        /// Finds the first claimType/s value of the given ClaimsIdentity.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity?.FindFirst(claimType)?.Value;
        }

        /// <summary>
        /// Finds the first claimType's value of the given IIdentity
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string GetUserClaimValue(this IIdentity identity, string claimType)
        {
            var identity1 = identity as ClaimsIdentity;
            return identity1?.FindFirstValue(claimType);
        }

        public static long? GetUserId(this IIdentity identity)
        {
            var userIdValue = identity?.GetUserClaimValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdValue))
            {
                return null;
            }

            if (long.TryParse(userIdValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var userId))
            {
                return userId;
            }

            return null;
        }


        public static long GetLoggedInUserId(this IIdentity identity)
        {
            var userIdValue = identity.GetUserClaimValue(ClaimTypes.NameIdentifier);
            return long.Parse(userIdValue, NumberStyles.Number, CultureInfo.InvariantCulture);
        }
    }
}
