using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetAppointment.Lib.App.Model;
using VetAppointment.Lib.Domain;
using VetAppointment.Lib.Infra.UnitOfWork;

namespace VetAppointment.UI.Controllers
{
    public class RegionController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IAppContext _appContext;

        public RegionController(IUnitOfWork unitOfWork, IAppContext appContext)
        {
            _unitOfWork = unitOfWork;
            _appContext = appContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _unitOfWork.GetRepository<City>().Where(disableTracking: true).ToListAsync();
            return Json(cities);
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts(int cityId)
        {
            var distcits = await _unitOfWork.GetRepository<District>().Where(x => x.CityId == cityId, disableTracking: true).ToListAsync();

            return Json(distcits);
        }
    }
}
