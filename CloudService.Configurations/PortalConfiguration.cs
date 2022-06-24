namespace CloudService.Configurations
{
    public class PortalConfiguration
    {
        public JwtConfiguration Jwt { get; set; }

        public CorsConfiguration Cors { get; set; }

        public UploadingConfiguration Uploading { get; set; }
    }
}
