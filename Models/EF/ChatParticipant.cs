namespace Chat.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChatParticipant
    {
        [Key]
        public int ParticipantId { get; set; }

        public int ChatId { get; set; }

        public int UserId { get; set; }

        public DateTime? JoinedDate { get; set; }

        [StringLength(50)]
        public string Role { get; set; }

        public virtual Chat Chat { get; set; }

        public virtual User User { get; set; }
    }
}
