namespace Chat.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Message
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Message()
        {
            Attachments = new HashSet<Attachment>();
        }

        public int MessageId { get; set; }

        public int SenderId { get; set; }

        public int? ReceiverId { get; set; }

        public int? ChatId { get; set; }

        [Required]
        public string MessageContent { get; set; }

        public DateTime? SentTime { get; set; }

        public DateTime? SeenTime { get; set; }

        [StringLength(50)]
        public string MessageType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attachment> Attachments { get; set; }

        public virtual Chat Chat { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
