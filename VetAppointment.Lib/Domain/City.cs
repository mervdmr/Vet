namespace VetAppointment.Lib.Domain
{
    public class City : BaseEntity<int>
    {
        public string Name { get; set; }
        public List<District> Districts { get; set; }

    }
}
