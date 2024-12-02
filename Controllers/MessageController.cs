using System;
using System.Linq;
using System.Web.Mvc;
using Chat.Models.EF;

namespace Chat.Controllers
{
    public class MessageController : Controller
    {
        private readonly Chat_DbContext db = new Chat_DbContext();

        // GET: Message/ChatMessages/{chatId}
        public ActionResult ChatMessages(int chatId)
        {
            var messages = db.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentTime)
                .ToList();

            return PartialView("_ChatMessages", messages); // Use partial view for real-time updates
        }

        // POST: Message/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(int chatId, string messageContent, string messageType = "text")
        {
            if (string.IsNullOrEmpty(messageContent))
            {
                ModelState.AddModelError("", "Message content cannot be empty.");
                return RedirectToAction("Details", "Chat", new { id = chatId });
            }

            var senderId = Convert.ToInt32(Session["UserId"]);
            var message = new Message
            {
                SenderId = senderId,
                ChatId = chatId,
                MessageContent = messageContent,
                SentTime = DateTime.Now,
                MessageType = messageType
            };

            db.Messages.Add(message);
            db.SaveChanges();

            return RedirectToAction("Details", "Chat", new { id = chatId });
        }

        // POST: Message/MarkAsSeen
        [HttpPost]
        public ActionResult MarkAsSeen(int messageId)
        {
            var message = db.Messages.Find(messageId);
            if (message != null && message.SeenTime == null)
            {
                message.SeenTime = DateTime.Now;
                db.SaveChanges();
            }

            return new EmptyResult();
        }
    }
}
