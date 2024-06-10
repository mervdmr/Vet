using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VetAppointment.Lib.App.Model;
using VetAppointment.Lib.Domain;

namespace VetAppointment.Lib.App.Helper
{
    public interface IClaimsHelper
    {
        List<Claim> PrepareClaims(User user);
    }

    public class ClaimsHelper : IClaimsHelper
    {
        #region Fields

        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region ctor

        public ClaimsHelper(IServiceProvider serviceProvider)
        {

            _serviceProvider = serviceProvider;
        }

        #endregion

        public List<Claim> PrepareClaims(User user)
        {

            try
            {

                List<Claim> claims = new List<Claim>()
                {
                new Claim("UserId", user.Id.ToString()),
                new Claim("NameSurname", user.Name + " " + user.Surname),
                new Claim("Email", user.Email),
                new Claim("IsAdmin", user.UserRoles.Any(x => x.Role.Name == "Admin") ? "true" : "false"),
                new Claim("UserRoles", JsonConvert.SerializeObject(user.UserRoles,new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}))
                };

                return claims;
            }
            catch (Exception ex)
            {
                return new List<Claim>();
            }


        }
    }
}
