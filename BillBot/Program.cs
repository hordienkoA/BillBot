using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using BillBot.Commands;
using BillBot.Database;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace BillBot
{
    class Program
    {
        private static TelegramBotClient client;
        private static List<Command> commandsList;

        public static void CheckAndAddUserToDb(Telegram.Bot.Types.User user)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (db.Users.FirstOrDefault(u => u.UserId == user.Id) == null)
                    {
                        db.Users.Add(new Database.User()
                        {
                            UserId = user.Id, UserName = user.Username, FirstName = user.FirstName, IsBot = user.IsBot,
                            LanguageCode = user.LanguageCode, LastName = user.LastName
                        });
                        db.SaveChanges();
                    }
                }


            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static void CheckAndAddChatToDb(Telegram.Bot.Types.Chat chat)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (db.Chats.FirstOrDefault(c => c.ChatId == chat.Id) == null)
                    {
                        db.Chats.Add(new Database.Chat()
                        {
                            ChatId = chat.Id
                        });
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error with db access");
            }


        }
        
        public static void CheckAndAddUserStatusToDb(int userId, long chatId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    int userkey = db.Users.FirstOrDefault(u => u.UserId == userId).Id;
                    int chatkey = db.Chats.FirstOrDefault(c => c.ChatId == chatId).Id;
                    bool isInExist = db.Chats.First(c => c.Id == chatkey)
                                         .UserStatuses.FirstOrDefault(s =>
                                             s.User.Id == userkey) !=
                                     null;
                                     
                    if (!isInExist)
                    {

                        //var us = db.UserStatuses.Add(new UserStatus());
                        var us = new UserStatus();
                        us.UserId = userkey;
                        us.ChatId = chatkey;
                        db.UserStatuses.Add(us);
                        //us.Entity.User=db.Users.FirstOrDefault(u=>u.UserId==userkey);
                        //us.Entity.Chat = db.Chats.FirstOrDefault(c => c.ChatId == chatkey);

                        db.SaveChanges();
                    }

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
                
            


        
        public static void DeleteStatusByUserAndChat(Telegram.Bot.Types.User user, Telegram.Bot.Types.Chat chat)
        {

            using (var db = new ApplicationContext())
            {
                try
                {
                    var status = db.UserStatuses
                        .Where(s => s.Chat.ChatId == chat.Id)
                        .FirstOrDefault(s => s.User.UserId == user.Id);
                    db.UserStatuses.Remove(status);
                    db.SaveChanges();
                }
                catch
                {
                    Console.WriteLine("Error with deleting statys");
                }
            }
        }


        public static void AddMemberByUserAndChat(Telegram.Bot.Types.User[] messageNewChatMembers, Telegram.Bot.Types.Chat messageChat)
        {
            try
            {
                foreach (var VARIABLE in messageNewChatMembers)
                {
                    CheckAndAddUserStatusToDb(VARIABLE.Id, messageChat.Id);
                }
            }
            catch
            {
                Console.WriteLine("Error with database");
            }
        }

        public static Achievement CheckAndAddAchivement(UserStatus status)
        {
            switch (status.FugCount)
            {
                case 1:
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        var acvivement = db.Achievements.FirstOrDefault(a => a.Name == "firstAchivement");
                        status.StatusWithAchievements.Add(new StatusWithAchievement() {AchievementId = acvivement.Id});
                        return acvivement;
                    }


                default:

                    return null;
            }
        }


        static void Main(string[] args)
        {
            client=new TelegramBotClient(BotConfiguration.token);
            client.OnMessage += BotMessageReceived;
            client.StartReceiving();
            while (true)
            {
                
            }
            client.StopReceiving();
            
        }

        public static async void BotMessageReceived(object? sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message.From != null && message.Chat != null)
            {
                CheckAndAddUserToDb(message.From);
                CheckAndAddChatToDb(message.Chat);
                CheckAndAddUserStatusToDb(message.From.Id,message.Chat.Id);
            }

            if (message.Type == MessageType.ChatMemberLeft)
            {
                DeleteStatusByUserAndChat(message.LeftChatMember,message.Chat);
            }
            else if (message.Type == MessageType.ChatMembersAdded)
            {
                AddMemberByUserAndChat(message.NewChatMembers,message.Chat);
            }

            if (message.Type == MessageType.Text)
            {
                if (message.Text.Contains("Билл", StringComparison.OrdinalIgnoreCase))
                {
                    var substr = message.Text.Substring(message.Text.IndexOf("Билл",StringComparison.OrdinalIgnoreCase));
                    if (substr.Contains("Когда", StringComparison.OrdinalIgnoreCase))
                    {
                        Random gen=new Random();
                        DateTime random_Date = DateTime.Today.AddDays(gen.Next(0, 10000));
                        await client.SendTextMessageAsync(message.Chat.Id, random_Date.ToString("dd.MM.yyyy"));
                    }
                    if (substr.Contains("Кто", StringComparison.OrdinalIgnoreCase))
                    {
                        using(ApplicationContext db=new ApplicationContext())
                        {
                            Random rnd = new Random();
                            var users = db.UserStatuses.Where(s => s.Chat.ChatId == message.Chat.Id).Select(s => s.User).ToList();
                            Database.User user = users.ElementAt(rnd.Next(0, users.Count()));
                            string nick = user.UserName == null
                    ? $"<a href=\"tg://user?id={user.UserId}\">{user.FirstName}</a>"
                    : $"@{user.UserName}";
                            await client.SendTextMessageAsync(message.Chat.Id, nick,ParseMode.Html);
                        }

                    }
                    if (substr.Contains("Число", StringComparison.OrdinalIgnoreCase))
                    {
                        var rndSub = System.Text.RegularExpressions.Regex.Split(message.Text.Substring(message.Text.IndexOf("Число", StringComparison.OrdinalIgnoreCase)), @"\s+");
                        Random rnd = new Random();
                        try {
                            await client.SendTextMessageAsync(message.Chat.Id, rnd.Next(int.Parse(rndSub[2]), int.Parse(rndSub[4])).ToString());
                        }
                        catch
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "недопустимый промежуток");
                        }
                    }
                    if (substr.Contains("где", StringComparison.OrdinalIgnoreCase))
                    {
                        Random rnd = new Random();
                        var latitude = (float)(rnd.NextDouble() * (90.0 - (-90.0)) + (-90.0));
                        var longtitude = -(float)(rnd.NextDouble() * (180.0 - (-180.0)) + (-180.0));
                        
                        await client.SendLocationAsync(message.Chat.Id,latitude, longtitude);
                    }


                        if (message.Text.Contains("slaves", StringComparison.OrdinalIgnoreCase))
                    {

                        GetSlaves gs = new GetSlaves(new ApplicationContext());
                        await gs.Execute(message, client);

                    }
                     /*if (message.Text.Contains("кто", StringComparison.OrdinalIgnoreCase))
                    {
                        Slave slave = new Slave(new ApplicationContext());
                        await slave.Execute(message, client);
                    }*/
                     
                }
                
            }
        }
    }
}