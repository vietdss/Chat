﻿using Chat.Models;
using Chat.Models.EF;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Chat
{
    public class ChatHub : Hub
    {
        private readonly Chat_DbContext db = new Chat_DbContext();
       
        // Lưu danh sách kết nối (UserId -> ConnectionId)
        private static readonly ConcurrentDictionary<int, string> UserConnections = new ConcurrentDictionary<int, string>();

        // Sự kiện khi người dùng kết nối
        public override Task OnConnected()
        {
            int userId = int.Parse(Context.QueryString["userId"]);
            UserConnections[userId] = Context.ConnectionId; // Lưu ánh xạ userId -> ConnectionId
            return base.OnConnected();
        }


        // Sự kiện khi người dùng ngắt kết nối
        public override Task OnDisconnected(bool stopCalled)
        {
            
            int userId = GetUserIdFromContext();
            UserConnections.TryRemove(userId, out _);
            return base.OnDisconnected(stopCalled);
        }

        // Gửi tin nhắn riêng
        public void SendPrivateMessage(int receiverId, string message, int senderId, string senderName)
        {
            // Lưu tin nhắn vào cơ sở dữ liệu
            var newMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                MessageContent = message,
                SentTime = DateTime.Now,
                MessageType = "private" // Đặt loại tin nhắn là "private"
            };

            // Thêm tin nhắn vào bảng Messages
            db.Messages.Add(newMessage);
            db.SaveChanges();

            // Gửi tin nhắn đến người nhận nếu đang kết nối
            if (UserConnections.TryGetValue(receiverId, out string connectionId))
            {
                Clients.Client(connectionId).receivePrivateMessage(senderName, message, DateTime.Now);
            }
            else
            {
                // Nếu người nhận không trực tuyến, bạn có thể thêm logic thông báo hoặc lưu trạng thái chưa đọc
                Clients.Caller.receivePrivateMessage("Hệ thống", "Người nhận không trực tuyến.", DateTime.Now);
            }
        }


        // Gửi tin nhắn trong phòng chat
        public void Send(string name, string messageContent, int chatId, int senderId)
        {
            if (string.IsNullOrWhiteSpace(messageContent))
            {
                Clients.Caller.notify("Tin nhắn không được để trống.");
                return;
            }

            try
            {
                var newMessage = new Message
                {
                    SenderId = senderId,
                    ChatId = chatId,
                    MessageContent = messageContent,
                    SentTime = DateTime.Now,
                    MessageType = "text"
                };
                db.Messages.Add(newMessage);
                db.SaveChanges();

                // Gửi tin nhắn đến tất cả các client
                Clients.All.addNewMessageToPage(name, messageContent, DateTime.Now.ToString("g"));
            }
            catch (Exception ex)
            {
                Clients.Caller.notify($"Lỗi khi gửi tin nhắn: {ex.Message}");
            }
        }

        // Lấy lịch sử tin nhắn của một phòng chat
        public void GetChatHistory(int chatId)
        {
            try
            {
                var messages = db.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentTime)
            .Select(m => new
            {
                m.MessageId,
                SenderName = db.Users.Where(u => u.UserId == m.SenderId).Select(u => u.Username).FirstOrDefault(),
                m.MessageContent,
                m.SentTime
            })
            .AsEnumerable() // Chuyển dữ liệu sang bộ nhớ
            .Select(m => new
            {
                m.MessageId,
                m.SenderName,
                m.MessageContent,
                SentTime = m.SentTime != null ? m.SentTime.Value.ToString("g") : null
            })
            .ToList();

                Clients.Caller.loadChatHistory(messages); // Gửi lịch sử cho client vừa kết nối
            }
            catch (Exception ex)
            {
                Clients.Caller.notify($"Lỗi khi tải lịch sử tin nhắn: {ex.Message}");
            }
        }

        // Lấy UserId từ ngữ cảnh
        private int GetUserIdFromContext()
        {
            // Ví dụ: nếu UserId lưu trong Context.QueryString
            return int.Parse(Context.QueryString["userId"]);
        }
    }
}
