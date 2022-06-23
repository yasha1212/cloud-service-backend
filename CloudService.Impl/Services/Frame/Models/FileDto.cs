namespace CloudService.Impl.Services.FrameService.Models
{
    public class FileDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool? IsOpened { get; set; }

        public double Size { get; set; }

        public string SizeType { get; set; }
    }
}
