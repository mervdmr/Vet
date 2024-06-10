using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetAppointment.Lib.App.Helper;
using VetAppointment.Lib.App.Model;
using VetAppointment.Lib.Domain;
using VetAppointment.Lib.Infra.UnitOfWork;

namespace VetAppointment.Lib.App.Validation
{
    public static class Validator
    {
        public static ReturnModel ValidateAppointment(this Appointment appointment)
        {
            bool isSuccess = true;
            StringBuilder stringBuilder = new StringBuilder();
            if (appointment.AppointmentDate == DateTime.MinValue)
            {
                stringBuilder.AppendLine("Lütfen randevu tarihi seçiniz.");
                isSuccess = false;
            }
            if (appointment.StartTime == TimeSpan.MinValue || appointment.StartTime == TimeSpan.Zero)
            {
                stringBuilder.AppendLine("Lütfen randevu saatini seçiniz.");
                isSuccess = false;
            }
            if (appointment.ClinicId == 0)
            {
                stringBuilder.AppendLine("Lütfen klinik bilgisini seçiniz.");
                isSuccess = false;
            }
            if (!appointment.Pet.Name.HasValue())
            {
                stringBuilder.AppendLine("Lütfen hayvan bilgisini giriniz.");
                isSuccess = false;
            }
            ReturnModel returnModel = new ReturnModel();
            returnModel.IsSuccess = isSuccess;
            returnModel.Message = stringBuilder.ToString();
            return returnModel;

        }

        public static ReturnModel ValidateRegister(this RegisterModel user, IUnitOfWork? unitOfWork = default)
        {
            bool isSuccess = true;
            StringBuilder stringBuilder = new StringBuilder();
            if (!user.Name.HasValue())
            {
                stringBuilder.AppendLine("Lütfen isminizi giriniz.");
                isSuccess = false;
            }
            if (!user.Email.HasValue())
            {
                stringBuilder.AppendLine("Lütfen Email adresinizi giriniz.");
                isSuccess = false;
            }
            if (!user.Phone.HasValue())
            {
                stringBuilder.AppendLine("Lütfen telefon numaranızı giriniz.");
                isSuccess = false;
            }
            if (!user.Password.HasValue())
            {
                stringBuilder.AppendLine("Lütfen şifrenizi numaranızı giriniz.");
                isSuccess = false;
            }
            if (user.UserId > 0 && user.RoleId <= 0)
            {
                stringBuilder.AppendLine("Lütfen Kullanıcı rolü belirleyiniz.");
                isSuccess = false;
            }
            if (unitOfWork is not default(IUnitOfWork?) && user.UserId <= 0)
            {
                var existingUser = unitOfWork.GetRepository<User>().AnyAsync(x => x.Email.Equals(user.Email) || x.Phone.Equals(user.Phone)).GetAwaiter().GetResult();
                if (existingUser)
                {
                    isSuccess = false;
                    stringBuilder.AppendLine("Girdiğiniz bilgilerle sistemde kullanıcı kayıtlıdır.");
                }
            }
            ReturnModel returnModel = new ReturnModel();
            returnModel.IsSuccess = isSuccess;
            returnModel.Message = stringBuilder.ToString();
            return returnModel;

        }
    }
}
