namespace VetAppointment.Lib.Domain
{
    public class Pet : BaseEntity<int>
    {

        public string Name { get; set; }
        public Appointment Appointment { get; set; }
        public int SpeciesId { get; set; }
        public Species Species { get; set; }

    }
}
