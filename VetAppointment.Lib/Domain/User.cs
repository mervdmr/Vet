

namespace VetAppointment.Lib.Domain
{
    public class User : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<Clinic> Clinics { get; set; }

    }
}
