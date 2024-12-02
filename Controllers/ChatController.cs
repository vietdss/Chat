using Chat.Models;
using Chat.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chat.Controllers
{
    [RequireLogin]

    public class ChatController : Controller
    {
        private Chat_DbContext db = new Chat_DbContext();

        public ActionResult Index()
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);
            var users = db.Users
                          .Where(u => u.UserId != currentUserId)
                          .ToList();

            return View(users);
        }
        public ActionResult UserList()
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);
            var users = db.Users
                          .Where(u => u.UserId != currentUserId)
                          .ToList();

            return View(users);

        }

        public ActionResult CreateChat(int userId)
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);
            var chat = db.Chats
                         .FirstOrDefault(c => !c.IsGroup &&
                                              c.ChatParticipants.Any(cp => cp.UserId == currentUserId) &&
                                              c.ChatParticipants.Any(cp => cp.UserId == userId));
            if (chat == null)
            {
                // Tạo cuộc trò chuyện mới
                var newChat = new Models.EF.Chat
                {
                    ChatName = "Private Chat",
                    IsGroup = false,
                    CreatedDate = DateTime.Now
                };
                db.Chats.Add(newChat);
                db.SaveChanges();

                // Thêm người tham gia
                db.ChatParticipants.Add(new ChatParticipant { ChatId = newChat.ChatId, UserId = currentUserId });
                db.ChatParticipants.Add(new ChatParticipant { ChatId = newChat.ChatId, UserId = userId });
                db.SaveChanges();

                chat = newChat;
            }

            return RedirectToAction("ChatRoom", new { id = chat.ChatId });
        }

        public ActionResult ChatRoom(int id)
        {
            var chat = db.Chats.FirstOrDefault(c => c.ChatId == id);
            if (chat == null)
            {
                return HttpNotFound();
            }
            return View(chat);
        }
        

        

    }
}