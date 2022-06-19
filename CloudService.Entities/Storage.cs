using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudService.Entities
{
    public class Storage : Entity
    {
        public string Name { get; set; }

        public long Capacity { get; set; }

        public long UsedCapacity { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
