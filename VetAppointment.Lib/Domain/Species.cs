namespace VetAppointment.Lib.Domain
{
    public class Species : BaseEntity<int>
    {
        public string Name { get; set; }
        public List<Pet> Pets { get; set; }
    }
}
