namespace Chat.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Attachment
    {
        public int AttachmentId { get; set; }

        public int MessageId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        public string FileUrl { get; set; }

        [StringLength(50)]
        public string FileType { get; set; }

        public DateTime? UploadedTime { get; set; }

        public virtual Message Message { get; set; }
    }
}
