using Laster.Core.Helpers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using IO = System.IO;

namespace Laster.Process.Telegram
{
    public class TelegramBot : TelegramBotClient
    {
        List<long> _AllowedChats = new List<long>();

        /// <summary>
        /// Chats permitidos
        /// </summary>
        public long[] AllowedChats { get { return _AllowedChats.ToArray(); } }

        public TelegramBot(string token) : base(token) { }

        public void AllowedChatsAdd(params long[] ids)
        {
            if (ids == null) return;
            foreach (long id in ids)
            {
                if (!_AllowedChats.Contains(id))
                    _AllowedChats.Add(id);
            }
        }
        public void AllowedChatsRemove(long id)
        {
            _AllowedChats.Remove(id);
        }
        public void AllowedChatsSave(string file)
        {
            string data = SerializationHelper.SerializeToJson(_AllowedChats.ToArray(), false, false);
            IO.File.WriteAllText(file, data, Encoding.UTF8);
        }
        public bool AllowedChatsContains(long id)
        {
            return _AllowedChats.Contains(id);
        }
        public void AllowedChatsClear()
        {
            _AllowedChats.Clear();
        }
        public void SendMessage(string message, ParseMode mode, IReplyMarkup keyboard, params long[] chatIds)
        {
            if (string.IsNullOrEmpty(message)) return;

            foreach (long chat in chatIds)
                try
                {
                    Task t = SendTextMessageAsync(chat, message, false, false, 0, keyboard, mode);
                    t.Wait();

                    if (t.Exception != null) throw (t.Exception);
                }
                catch (ApiRequestException)
                {
                    if (mode != ParseMode.Default)
                    {
                        Task t = SendTextMessageAsync(chat, message, false, false, 0, keyboard, ParseMode.Default);
                        t.Wait();
                        if (t.Exception != null) throw (t.Exception);

                        //if (t.Exception != null) throw (t.Exception);
                        mode = ParseMode.Default;
                    }
                }
        }
        public static void ReleaseCreateTelegramBotClient(TelegramBotClient stop) { stop.StopReceiving(); }
    }
}