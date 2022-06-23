namespace CloudService.Web.ViewModels.Storage
{
    public class StorageViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Capacity { get; set; }

        public string CapacityType { get; set; }

        public double UsedCapacity { get; set; }

        public string UsedCapacityType { get; set; }
    }
}
