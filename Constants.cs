using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationDashboardServiceCommon.Helper
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol", Id = "id", Name = "Name", Email = "Email", Password = "Password",Company="Compamy",Role="Role";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
            }
        }
    }
}
