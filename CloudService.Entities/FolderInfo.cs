using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudService.Entities
{
    public class FolderInfo : Entity
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public string StorageId { get; set; }

        public string FolderId { get; set; }

        [ForeignKey("StorageId")]
        public Storage Storage { get; set; }

        [ForeignKey("FolderId")]
        public FolderInfo Folder { get; set; }

        public List<FolderInfo> ChildFolders { get; set; }

        public List<FileInfo> ChildFiles { get; set; }

        public FolderInfo()
        {
            ChildFiles = new List<FileInfo>();
            ChildFolders = new List<FolderInfo>();
        }
    }
}
