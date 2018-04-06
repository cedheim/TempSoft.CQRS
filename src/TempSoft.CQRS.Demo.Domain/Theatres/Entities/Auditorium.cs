namespace TempSoft.CQRS.Demo.Domain.Theatres.Entities
{
    public class Auditorium
    {
        private Auditorium() { }

        public Auditorium(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Name { get; set; }

        public string Id { get; set; }
    }
}