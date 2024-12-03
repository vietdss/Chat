using Chat.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chat.Models.ViewModel
{
    public class ChatViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Models.EF.Chat> Groups { get; set; }
    }
}