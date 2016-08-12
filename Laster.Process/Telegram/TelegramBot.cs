using Laster.Core.Helpers;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
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
    }
}