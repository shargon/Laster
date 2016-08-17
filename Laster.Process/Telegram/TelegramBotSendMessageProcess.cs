using Laster.Core.Classes;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
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
        [DefaultValue("")]
        public string ApiKey { get; set; }
        [DefaultValue(ParseMode.Html)]
        public ParseMode MessageMode { get; set; }

        /// <summary>
        /// Mensaje Fijo a enviar
        /// </summary>
        [Description("Only send this message")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
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
                _Bot.Value.SendMessage(FixedMessage, MessageMode, _Bot.Value.AllowedChats);
                return data;
            }

            if (data != null)
                foreach (object o in data)
                {
                    if (o == null) continue;
                    _Bot.Value.SendMessage(o.ToString(), MessageMode, _Bot.Value.AllowedChats);
                }

            return data;
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
                _Bot.Free(this, TelegramBot.ReleaseCreateTelegramBotClient);
                _Bot = null;
            }

            base.OnStop();
        }
    }
}