using BillBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BillBot.Commands
{
    class Slave:Command
    {
        public override string Name { get; } = @"/set_slave";
        private readonly Random rnd;
        private readonly ApplicationContext db;
        public Slave(ApplicationContext db)
        {
            rnd = new Random();
            this.db = db;
        }
         public override Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var users = db.UserStatuses.Where(s => s.Chat.ChatId == chatId).Select(s => s.User).ToList();
            Database.User user = users.ElementAt(rnd.Next(0, users.Count()));
            var status = db.UserStatuses.FirstOrDefault(s => s.User.Id == user.Id);
            status.FugCount++;
            db.SaveChanges();
            var newAchivement = Program.CheckAndAddAchivement(status);
            string nick = user.UserName == null
                    ? $"<a href=\"tg://user?id={user.UserId}\">{user.FirstName}</a>"
                    : $"@{user.UserName}";
            /*if (newAchivement != null)
            {
                client.SendTextMessageAsync(chatId,
                        String.Format(
                            db.Phrases.ToList().ElementAt(rnd.Next(0, db.Phrases.Count())).Text, nick), ParseMode.Html);
                return client.SendPhotoAsync(chatId, photo: newAchivement.PathToImage,
                    caption: $"<b>Вы заработали новую ачивку:</b>\n {nick}", ParseMode.Html);
            }
            else{*/
                return client.SendTextMessageAsync(chatId,
                        /*String.Format(*/
                            /*db.Phrases.ToList().ElementAt(rnd.Next(0, db.Phrases.Count())).Text,*/ nick, ParseMode.Html);
            /*}*/
        }

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
                return false;
            }

            return message.Text.Contains(this.Name);
        }
    }
}
