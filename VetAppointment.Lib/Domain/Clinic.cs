namespace VetAppointment.Lib.Domain
{
    public class Clinic : BaseEntity<int>
    {

        public int UserId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}
