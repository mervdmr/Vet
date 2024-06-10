using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VetAppointment.Lib.Domain;

namespace VetAppointment.Lib.App.Model
{
    public interface IAppContext
    {
        int UserId { get; }
        string NameSurname { get; }
        bool IsSuperAdmin { get; }
        bool IsVet { get; }
        public string Email { get; }
        public List<UserRole> UserRoles { get; }

    }

    public class AppContext : IAppContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnumerable<Claim> claims;
        public AppContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext?.User?.Claims != null)
            {
                claims = _httpContextAccessor.HttpContext.User.Claims ?? new List<Claim>();

            }
        }

        public int UserId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "UserId").Value);
                }
                catch (Exception)
                {

                    return -1;
                }
              
            }
        }

        public string NameSurname
        {
            get
            {
                try
                {
                    return claims.FirstOrDefault(x => x.Type == "NameSurname").Value;
                }
                catch (Exception)
                {

                    return string.Empty;
                }
               
            }
        }

        public bool IsSuperAdmin
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(claims.FirstOrDefault(x => x.Type == "IsAdmin").Value);
                }
                catch (Exception)
                {

                    return false;
                }
            
            }
        }

        public string Email
        {
            get
            {
                try
                {
                    return claims.FirstOrDefault(x => x.Type == "Email").Value;
                }
                catch (Exception)
                {

                    return string.Empty;
                }
            
            }
        }

        public List<UserRole> UserRoles
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<UserRole>>(claims.FirstOrDefault(x => x.Type == "UserRoles").Value);
                }
                catch (Exception)
                {

                    return new List<UserRole>();
                }
               
            }
        }

        public bool IsVet
        {
            get
            {
                try
                {
                    return UserRoles.Any(x => x.Role.Name.Equals("Vet", StringComparison.OrdinalIgnoreCase));
                }
                catch (Exception)
                {

                    return false;
                }
               
            }
        }
    }
}
