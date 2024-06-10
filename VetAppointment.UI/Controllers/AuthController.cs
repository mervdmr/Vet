using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VetAppointment.Lib.App.Model;
using VetAppointment.Lib.Domain;
using VetAppointment.Lib.Infra.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using VetAppointment.Lib.App.Helper;
using VetAppointment.Lib.App.Validation;

namespace VetAppointment.UI.Controllers
{
    public class AuthController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IClaimsHelper _claimsHelper;
        public AuthController(IUnitOfWork unitOfWork, IClaimsHelper claimsHelper)
        {
            _unitOfWork = unitOfWork;
            _claimsHelper = claimsHelper;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model, string redirectUrl = null)
        {
            if (model == null)
            {
                var result = ReturnModel.FailureResponse("Model Boş geçilemez.");
                return Json(result);
            }
            var user = await _unitOfWork.GetRepository<User>().Where(x => x.Email == model.Email && x.Password == model.Password,
                include: source => source.Include(x => x.UserRoles).ThenInclude(y => y.Role)).FirstOrDefaultAsync();
            if (user == null)
            {
                var result = ReturnModel.FailureResponse("Girdiğiniz bilgilere ait kullanıcı bulunamadı.");
                return Json(result);
            }
            List<Claim> claims = _claimsHelper.PrepareClaims(user);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
            };
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
CookieAuthenticationDefaults.AuthenticationScheme,
new ClaimsPrincipal(claimsIdentity),
authProperties);

            ReturnModel returnModel = new ReturnModel();
            returnModel.IsSuccess = true;
            returnModel.Message = "Giriş başarılı. Yönlendiriliyorsunuz..";
            var loginResult = ReturnModel.SuccessResponse("Giriş başarılı. Yönlendiriliyorsunuz", redirectUrl: redirectUrl.HasValue() ? redirectUrl : "/Home/Index");

            return Json(loginResult);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {

            var validator = registerModel.ValidateRegister(_unitOfWork);
            if (!validator.IsSuccess)
            {
                return Json(validator);
            }
            var role = await _unitOfWork.GetRepository<Role>().GetAsync(x => x.Name == "User");
            User user = new User()
            {
                Name = registerModel.Name,
                Email = registerModel.Email,
                Surname = registerModel.Surname,
                Phone = registerModel.Phone,
                Password = registerModel.Password,
                UserRoles = new List<UserRole>
                {
                    new UserRole() {RoleId =  role.Id}
                }
            };


            await _unitOfWork.GetRepository<User>().InsertAsync(user);
            await _unitOfWork.SaveChangeAsync();
            var res = ReturnModel.SuccessResponse("Giriş başarılı, yönlendiriliyorsunuz..", redirectUrl: "Login");
            return Json(res);
        }




        public async Task<IActionResult> Logout()
        {

            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Login");
        }
    }
}
