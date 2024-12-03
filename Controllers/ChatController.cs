using Chat.Models;
using Chat.Models.EF;
using Chat.Models.ViewModel;
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

            // Lấy danh sách người dùng (ngoại trừ người dùng hiện tại)
            var users = db.Users
                          .Where(u => u.UserId != currentUserId)
                          .ToList();

            // Lấy danh sách nhóm chat mà người dùng hiện tại tham gia
            var groups = db.ChatParticipants
                           .Where(cp => cp.UserId == currentUserId)
                           .Select(cp => cp.Chat)
                           .Where(c => c.IsGroup) // Chỉ lấy nhóm chat
                           .ToList();

            // Tạo ViewModel
            var viewModel = new ChatViewModel
            {
                Users = users,
                Groups = groups
            };

            return View(viewModel);
        }
        public ActionResult UserList()
        {
            int currentUserId = Convert.ToInt32(Session["UserId"]);
            var users = db.Users
                          .Where(u => u.UserId != currentUserId)
                          .ToList();

            return View(users);

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