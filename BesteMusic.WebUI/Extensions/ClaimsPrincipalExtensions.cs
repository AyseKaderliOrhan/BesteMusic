using BesteMusic.Data.Enums;
using System.Security.Claims;

namespace BesteMusic.WebUI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return Convert.ToInt32(user.Claims.FirstOrDefault(x => x.Type == "id")?.Value);
          
        }
        public static bool IsLogged(this ClaimsPrincipal user)
        {
            if (user.Claims.FirstOrDefault(x => x.Type == "id") != null)
                return true;
            else
                return false;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            if (user.Claims.FirstOrDefault(x => x.Type == "userType")?.Value == UserTypeEnum.Admin.ToString())
                return true;
            else
                return false;
        }

        public static string GetUserFirstName(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type == "firstName")?.Value;
        }

        public static string GetUserLastName(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type == "lastName")?.Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
        }
    }
}
