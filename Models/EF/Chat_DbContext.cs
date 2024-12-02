using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Chat.Models.EF
{
    public partial class Chat_DbContext : DbContext
    {
        public Chat_DbContext()
            : base("name=Chat_DbContext")
        {
        }

        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<ChatParticipant> ChatParticipants { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.ReceiverId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages1)
                .WithRequired(e => e.User1)
                .HasForeignKey(e => e.SenderId)
                .WillCascadeOnDelete(false);
        }
    }
}
