namespace CloudService.Entities
{
    public abstract class Entity : IEntity<string>
    {
        public string Id { get; set; }
    }
}
