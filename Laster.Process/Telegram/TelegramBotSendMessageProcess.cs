using Laster.Core.Classes;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using Laster.Process.Telegram;
using System.ComponentModel;
using System.Drawing;
using Telegram.Bot.Types.Enums;

namespace Laster.Process.Telegram
{
    /// <summary>
    /// No permite que se repitan los datos
    /// </summary>
    public class TelegramBotSendMessageProcess : IDataProcess
    {
        public override string Title { get { return "TelegramBot - SendMessage"; } }

        [Category("Authentication")]
        public string ApiKey { get; set; }
        [DefaultValue(ParseMode.Html)]
        public ParseMode MessageMode { get; set; }

        /// <summary>
        /// Mensaje Fijo a enviar
        /// </summary>
        [Description("Only send this message")]
        public string FixedMessage { get; set; }

        ShareableClass<TelegramBot, string> _Bot;

        public TelegramBotSendMessageProcess()
        {
            MessageMode = ParseMode.Html;
            DesignBackColor = Color.Fuchsia;
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (!string.IsNullOrEmpty(FixedMessage))
            {
                SendMessage(FixedMessage, MessageMode);
                return data;
            }

            foreach (object o in data)
            {
                if (o == null) continue;
                SendMessage(o.ToString(), MessageMode);
            }

            return data;
        }

        public async void SendMessage(string message, ParseMode mode)
        {
            if (_Bot == null || string.IsNullOrEmpty(message)) return;

            foreach (long chat in _Bot.Value.AllowedChats)
            {
                /*Message r =*/
                await _Bot.Value.SendTextMessageAsync(chat, message, false, false, 0, null, mode);
                //r = await _Bot.EditMessageText(chat, r.MessageId, "<b>Editado</b>", ParseMode.Html);
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            if (string.IsNullOrEmpty(ApiKey)) return;

            _Bot = ShareableClass<TelegramBot, string>.GetOrCreate(this, ApiKey, TelegramBotSubscribeProcess.CreateTelegramBotClient);
        }

        public override void OnStop()
        {
            if (_Bot != null)
            {
                _Bot.Free(this, TelegramBotSubscribeProcess.ReleaseCreateTelegramBotClient);
                _Bot = null;
            }

            base.OnStop();
        }
    }
}