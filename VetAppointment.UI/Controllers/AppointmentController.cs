using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using VetAppointment.Lib.App.Helper;
using VetAppointment.Lib.App.Model;
using VetAppointment.Lib.App.Validation;
using VetAppointment.Lib.Domain;
using VetAppointment.Lib.Infra.AuthFilters;
using VetAppointment.Lib.Infra.UnitOfWork;

namespace VetAppointment.UI.Controllers
{
    [CustomAuthorize]
    public class AppointmentController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IAppContext _appContext;

        public AppointmentController(IUnitOfWork unitOfWork, IAppContext appContext)
        {
            _unitOfWork = unitOfWork;
            _appContext = appContext;
        }
        [HttpGet]
        public async Task<IActionResult> AddAppointment()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                return Json(ReturnModel.FailureResponse("Model boş gönderilemez"));
            }

            var validation = appointment.ValidateAppointment();
            if (!validation.IsSuccess)
            {
                return Json(validation);
            }
            appointment.UserId = _appContext.UserId;
            await _unitOfWork.GetRepository<Appointment>().InsertAsync(appointment);
            await _unitOfWork.SaveChangeAsync();

            return Json(ReturnModel.SuccessResponse("Randevu Başarıyla oluşturuldu"));
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailability(int clinicId, DateTime date)
        {

            var appointments = await _unitOfWork.GetRepository<Appointment>().Where(x => x.ClinicId == clinicId && x.AppointmentDate.Date == date.Date, disableTracking: true).ToListAsync();
            var availableTimes = AppointmentTime.AvailableTimes();
            if (appointments != null && appointments.Count > 0)
            {
                var appointmentTimes = appointments.Select(x => x.StartTime).ToList();
                availableTimes = availableTimes.Except(appointmentTimes).ToList();
            }

            if (date.Date == DateTime.Now.Date)
            {
                DateTime now = DateTime.Now;
                DateTime startOfDay = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                TimeSpan timePassedToday = now - startOfDay;
                double totalMinutesToday = timePassedToday.TotalMinutes;
                availableTimes = availableTimes.Where(x => x.TotalMinutes > totalMinutesToday).ToList();
            }
            else if (date.Date < DateTime.Now.Date)
            {
                availableTimes.Clear();
            }
            return Json(availableTimes);
        }

        [HttpGet]
        public async Task<IActionResult> GetPetTypes()
        {
            var petTypes = await _unitOfWork.GetRepository<Species>().GetListAsync();
            return Json(petTypes);
        }

        [CustomAuthorize("Vet")]
        public async Task<IActionResult> GetAppointmentsByVet()
        {
            var clinicIds = _unitOfWork.GetRepository<Clinic>().Where(x => x.UserId == _appContext.UserId).Select(x => x.Id).ToArray();
            IQueryable<Appointment> result = _unitOfWork.GetRepository<Appointment>().Where(x => clinicIds.Contains(x.ClinicId), include: source => source.Include(y => y.Pet).ThenInclude(y => y.Species).Include(y => y.Clinic).Include(y => y.User));

            return View(await result.ToListAsync());
        }

        public async Task<IActionResult> GetMyAppointments()
        {
            IQueryable<Appointment> result = _unitOfWork.GetRepository<Appointment>().Where(include: source => source.Include(y => y.Pet).ThenInclude(y => y.Species).Include(y => y.Clinic).Include(y => y.User));

            if (!_appContext.IsSuperAdmin)
            {
                result = result.Where(x => x.UserId == _appContext.UserId);
            }

            return View(await result.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _unitOfWork.GetRepository<Appointment>().GetAsync(x => x.Id == id);
            if (appointment == null)
            {
                var res = ReturnModel.FailureResponse("Randevu Bulunamadı.");
            }
            if (_appContext.UserId != appointment.UserId && !_appContext.IsSuperAdmin && !_appContext.IsVet)
            {
                var res = ReturnModel.FailureResponse("Sizin olmayan bir randevuda işlem yapamazsınız");
                return Json(res);
            }

            await _unitOfWork.GetRepository<Appointment>().Remove(appointment);
            await _unitOfWork.SaveChangeAsync();


            return Json(ReturnModel.SuccessResponse("İşlem başarıyla gerçekleştirildi...", pageRefresh: true));
        }

    }
}
