namespace TempSoft.CQRS.Events
{
    public class EventFilter
    {
        public string[] EventGroups { get; set; }

        public string[] EventTypes { get; set; }
    }
}