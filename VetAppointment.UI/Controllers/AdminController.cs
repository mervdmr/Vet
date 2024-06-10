using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetAppointment.Lib.App.Model;
using VetAppointment.Lib.Domain;
using VetAppointment.Lib.Infra.AuthFilters;
using VetAppointment.Lib.Infra.UnitOfWork;
using VetAppointment.Lib.App.Validation;
using VetAppointment.Lib.App.Helper;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
namespace VetAppointment.UI.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IAppContext _appContext;

        public AdminController(IUnitOfWork unitOfWork, IAppContext appContext)
        {
            _unitOfWork = unitOfWork;
            _appContext = appContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers(bool onlyVet = false)
        {
            var isAjaxReq = HttpContext.Request.IsAjaxRequest();
            var users = _unitOfWork.GetRepository<User>().Where(include: source => source.Include(y => y.UserRoles).ThenInclude(y => y.Role), disableTracking: true);
            if (onlyVet)
            {
                var role = await _unitOfWork.GetRepository<Role>().GetAsync(x => x.Name == "Vet");
                users = users.Where(x => x.UserRoles.Any(y => y.RoleId == role.Id));
            }
            if (isAjaxReq)
            {
                return Json(await users.ToListAsync());
            }
            return View(await users.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _unitOfWork.GetRepository<User>().GetAsync(x => x.Id == id);
            if (user == null)
            {
                return Json(ReturnModel.FailureResponse("Kullanıcı bulunamadı"));
            }
            await _unitOfWork.GetRepository<User>().Remove(user);
            await _unitOfWork.SaveChangeAsync();

            return Json(ReturnModel.SuccessResponse("Kullanıcı başarıyla silindi"));
        }

        [HttpGet]
        public async Task<IActionResult> AddUser(int userId)
        {
            if (userId != 0)
            {
                var user = await _unitOfWork.GetRepository<User>().GetAsync(x => x.Id == userId);
                ViewBag.User = user;
            }
            var roleList = await _unitOfWork.GetRepository<Role>().GetListAsync();
            return View(roleList);
        }
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] RegisterModel registerModel)
        {
            var validator = registerModel.ValidateRegister(_unitOfWork);
            if (!validator.IsSuccess)
            {
                return Json(validator);
            }
            User user = null;
            if (registerModel.UserId > 0)
            {
                user = await _unitOfWork.GetRepository<User>().Where(x => x.Id == registerModel.UserId, source => source.Include(y => y.UserRoles)).FirstOrDefaultAsync();
                if (user == null)
                {
                    return Json(ReturnModel.FailureResponse("Kullanıcı bulunamadı"));
                }
                user.UserRoles.ForEach(x =>
                {
                    x.RoleId = registerModel.RoleId;
                });
            }
            else
            {
                user = new User();
                user.UserRoles = new List<UserRole>
                {
                    new UserRole{RoleId = registerModel.RoleId}
                };
            }

            user.Name = registerModel.Name;
            user.Email = registerModel.Email;
            user.Surname = registerModel.Surname;
            user.Phone = registerModel.Phone;
            user.Password = registerModel.Password;



            await _unitOfWork.GetRepository<User>().Update(user);
            await _unitOfWork.SaveChangeAsync();
            var res = ReturnModel.SuccessResponse("Kullanıcı başarıyla eklendi..", pageRefresh: true);
            return Json(res);
        }

        [HttpPost]
        public async Task<IActionResult> AddSpecies([FromBody] Species species)
        {

            Species data = null;
            if (species.Id > 0)
            {
                data = await _unitOfWork.GetRepository<Species>().Where(x => x.Id == species.Id).FirstOrDefaultAsync();
                if (data == null)
                {
                    return Json(ReturnModel.FailureResponse("Tür bulunamadı"));
                }
            }
            else
            {
                data = new Species();
            }

            data.Name = species.Name;
            await _unitOfWork.GetRepository<Species>().Update(data);
            await _unitOfWork.SaveChangeAsync();
            var res = ReturnModel.SuccessResponse("Tür başarıyla eklendi..", pageRefresh: true);
            return Json(res);
        }

        [HttpGet]
        public async Task<IActionResult> AddSpecies(int speciesId)
        {
            Species species = new Species();
            if (speciesId != 0)
            {
                species = await _unitOfWork.GetRepository<Species>().GetAsync(x => x.Id == speciesId);
            }
            return View(species);
        }


        [HttpGet]
        public async Task<IActionResult> GetSpecies()
        {
            var reuslt = await _unitOfWork.GetRepository<Species>().GetListAsync();
            return View(reuslt);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSpecies(int id)
        {
            var species = await _unitOfWork.GetRepository<Species>().GetAsync(x => x.Id == id);
            if (species == null)
            {
                return Json(ReturnModel.FailureResponse("Tür bulunamadı"));
            }
            await _unitOfWork.GetRepository<Species>().Remove(species);
            await _unitOfWork.SaveChangeAsync();

            return Json(ReturnModel.SuccessResponse("Tür başarıyla silindi"));
        }

    }
}
