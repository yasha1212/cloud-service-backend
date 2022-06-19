using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudService.Entities
{
    public class SharingRule
    {
        public string FileId { get; set; }

        public string UserId { get; set; }

        public string Access { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ExpiredOn { get; set; }

        [ForeignKey("FileId")]
        public FileInfo File { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
