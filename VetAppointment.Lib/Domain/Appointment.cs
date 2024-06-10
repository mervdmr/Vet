namespace VetAppointment.Lib.Domain
{
    public class Appointment : BaseEntity<int>
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }
        public int PetId { get; set; }
        public Pet Pet { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
    }
}
