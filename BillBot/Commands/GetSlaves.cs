using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BillBot.Commands
{
    public class GetSlaves:Command
    {
        private readonly ApplicationContext context;
        
        public override string Name { get; } = @"/get_slaves";

        public GetSlaves(ApplicationContext db)
        {
            context = db;
        }
        public async override Task Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            StringBuilder sb=new StringBuilder();
            sb.Append("<b>Список Slaves:</b>"+"\n\n");
            foreach (var status in context.UserStatuses.Where(s=>s.Chat.ChatId==chatId).OrderByDescending(s => s.FugCount))
            {
                sb.Append("<i>"+status.User.FirstName + " " + status.User.LastName +"</i>"+"<pre>       </pre>"+BotConfiguration.PrideFlag+"  "+$"{status.FugCount} раз"+"\n");
                

            }
            await client.SendTextMessageAsync(chatId, sb.ToString(), ParseMode.Html);
        }

        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
            {
                return false;
            }

            return message.Text.Contains(this.Name);
        }
    }
}