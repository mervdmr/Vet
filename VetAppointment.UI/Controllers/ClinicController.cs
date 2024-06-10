using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetAppointment.Lib.App.Helper;
using VetAppointment.Lib.App.Model;
using VetAppointment.Lib.Domain;
using VetAppointment.Lib.Infra.AuthFilters;
using VetAppointment.Lib.Infra.UnitOfWork;

namespace VetAppointment.UI.Controllers
{

    public class ClinicController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppContext _appContext;

        public ClinicController(IUnitOfWork unitOfWork, IAppContext appContext)
        {
            _unitOfWork = unitOfWork;
            _appContext = appContext;
        }
        [CustomAuthorize("Vet")]
        public IActionResult AddClinic()
        {
            return View();
        }
        [HttpPost]
        [CustomAuthorize("Vet")]
        public async Task<IActionResult> AddClinic([FromBody] Clinic clinic)
        {
            if (clinic == null)
            {
                return Json(ReturnModel.FailureResponse("Model boş gönderilemez."));
            }
            if (!clinic.Name.HasValue() || clinic.DistrictId == 0)
            {
                return Json(ReturnModel.FailureResponse("Lütfen tüm bilgileri doldurunuz."));
            }
            if (clinic.UserId == 0)
            {
                clinic.UserId = _appContext.UserId;
            }
            await _unitOfWork.GetRepository<Clinic>().InsertAsync(clinic);
            await _unitOfWork.SaveChangeAsync();

            var result = ReturnModel.SuccessResponse("Klinik başarıyla eklendi.", pageRefresh: true);
            return Json(result);

        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetClinics(int districtId)
        {
            var clinics = await _unitOfWork.GetRepository<Clinic>().Where(x => x.DistrictId == districtId, disableTracking: true).ToListAsync();
            return Json(clinics);
        }

        [HttpGet]
        [CustomAuthorize("Vet")]
        public async Task<IActionResult> GetClinicsByVet()
        {
            //Eğer mevcut kullanıcı adminse, UserId bazlı filtreleme yapmaya gerek yok. Tüm klinikleri göster
            IQueryable<Clinic> clinics = _unitOfWork.GetRepository<Clinic>().Where(include: source => source.Include(y => y.User).Include(y => y.District));
            if (_appContext.IsVet)
            {
                clinics = clinics.Where(x => x.UserId == _appContext.UserId);
            }

            return View(await clinics.ToListAsync());
        }

        [HttpPost]
        [CustomAuthorize("Vet")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            try
            {
                var clinic = await _unitOfWork.GetRepository<Clinic>().GetAsync(x => x.Id == id);
                if (clinic == null)
                {
                    var res = ReturnModel.FailureResponse("Klinik bulunamadı.");
                    return Json(res);
                }
                if (_appContext.UserId != clinic.UserId && !_appContext.IsSuperAdmin)
                {
                    var res = ReturnModel.FailureResponse("Sizin olmayan bir klinikte işlem yapamazsınız");
                    return Json(res);
                }

                await _unitOfWork.GetRepository<Clinic>().Remove(clinic);
                await _unitOfWork.SaveChangeAsync();


                return Json(ReturnModel.SuccessResponse("İşlem başarıyla gerçekleştirildi...", pageRefresh: true));
            }
            catch (Exception ex)
            {

                throw;
            }

        }


    }
}
