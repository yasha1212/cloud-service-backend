using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudService.Entities
{
    public class FileInfo : Entity
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public bool IsProtected { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public string ExtensionId { get; set; }

        public string StorageId { get; set; }

        public string FolderId { get; set; }

        [ForeignKey("ExtensionId")]
        public Extension Extension { get; set; }

        [ForeignKey("StorageId")]
        public Storage Storage { get; set; }

        [ForeignKey("FolderId")]
        public FolderInfo Folder { get; set; }
    }
}
