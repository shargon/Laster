using Laster.Core.Classes;
using Laster.Core.Classes.RaiseMode;
using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using IO = System.IO;

namespace Laster.Process.Telegram
{
    /// <summary>
    /// No permite que se repitan los datos
    /// </summary>
    public class TelegramBotSubscribeProcess : IDataProcess
    {
        ShareableClass<TelegramBot, string> _Bot;
        public class TelegramAction
        {
            [Description("User text input")]
            public string UserInput { get; set; }
            [Description("Event thrown")]
            public string EventName { get; set; }
            [Description("Text for send")]
            public string SendText { get; set; }

            public override string ToString()
            {
                return UserInput + " => " + EventName;
            }
        }

        public override string Title { get { return "TelegramBot - Subscribe"; } }

        [DefaultValue("")]
        [Description("File for write the chat sessions")]
        public string FileChatStore { get; set; }

        [DefaultValue("/start")]
        [Category("Authentication")]
        public string SubscribePassword { get; set; }
        [DefaultValue("")]
        [Category("Authentication")]
        public string ApiKey { get; set; }
        [DefaultValue(ParseMode.Default)]
        public ParseMode MessageMode { get; set; }
        [DefaultValue(null)]
        [Category("Authentication")]
        public string[] AvailableUsers { get; set; }

        #region Mensajes
        [DefaultValue("")]
        [Category("Messages")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string MessageNotUserAvailable { get; set; }
        [DefaultValue("")]
        [Category("Messages")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string MessageWrongPassword { get; set; }
        [DefaultValue("")]
        [Category("Messages")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string MessageUnsubscribe { get; set; }
        [DefaultValue("")]
        [Category("Messages")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string MessageAlreadySubscribed { get; set; }
        [DefaultValue("")]
        [Category("Messages")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string MessageNotSubscribed { get; set; }
        #endregion

        [Category("Raise Events")]
        public List<TelegramAction> Events { get; set; }

        public TelegramBotSubscribeProcess()
        {
            Events = new List<TelegramAction>();
            MessageMode = ParseMode.Default;
            DesignBackColor = Color.Fuchsia;

            SubscribePassword = "/start";

            MessageNotUserAvailable = "No está configurado dentro de nuestro bot, no se moleste en escribir";
            MessageWrongPassword = "¡Contraseña correcta!";
            MessageUnsubscribe = "No se le informará de mas notificaciones";
            MessageAlreadySubscribed = "Será informado en cuanto ocurra algún error. En cualquier momento puede usar la palabra clave '/stop' para parar las notificaciones";
            MessageNotSubscribed = "No autentificado, ingrese la contraseña";

            FileChatStore = "TelegramAllowedChats.json";
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data != null)
                foreach (object o in data)
                {
                    if (o == null) continue;
                    _Bot.Value.SendMessage(o.ToString(), MessageMode, _Bot.Value.AllowedChats);
                }

            return data;
        }

        protected override void OnStart()
        {
            if (string.IsNullOrEmpty(ApiKey)) return;
            if (_Bot != null) return;

            _Bot = ShareableClass<TelegramBot, string>.GetOrCreate(this, ApiKey, CreateTelegramBotClient);
            if (_Bot != null)
            {
                if (IO.File.Exists(FileChatStore))
                {
                    long[] chats = SerializationHelper.DeserializeFromJson<long[]>(IO.File.ReadAllText(FileChatStore, Encoding.UTF8), false);
                    _Bot.Value.AllowedChatsClear();
                    _Bot.Value.AllowedChatsAdd(chats);
                }

                _Bot.Value.OnMessage += C_OnMessage;
            }
        }

        void C_OnMessage(object sender, MessageEventArgs e)
        {
            bool ok = AvailableUsers == null;
            if (!ok)
                foreach (string s in AvailableUsers)
                    if (s == e.Message.From.Username) { ok = true; break; }

            if (!ok)
            {
                _Bot.Value.SendMessage(MessageNotUserAvailable, MessageMode, e.Message.Chat.Id);
                return;
            }

            string text = e.Message.Text;

            if (!string.IsNullOrEmpty(text))
            {
                if (text == SubscribePassword)
                {
                    _Bot.Value.AllowedChatsAdd(e.Message.Chat.Id);
                    _Bot.Value.AllowedChatsSave(FileChatStore);

                    //await c.g(e.Message.Chat.Id, e.Message.MessageId, "******", ParseMode.Default);
                    _Bot.Value.SendMessage(MessageWrongPassword, MessageMode, e.Message.Chat.Id);
                }
                else
                {
                    if (_Bot.Value.AllowedChatsContains(e.Message.Chat.Id))
                    {
                        if (text.ToLowerInvariant() == "/stop")
                        {
                            _Bot.Value.AllowedChatsRemove(e.Message.Chat.Id);
                            _Bot.Value.AllowedChatsSave(FileChatStore);

                            _Bot.Value.SendMessage(MessageUnsubscribe, MessageMode, e.Message.Chat.Id);
                            return;
                        }
                    }
                }

                if (Events != null && _Bot.Value.AllowedChatsContains(e.Message.Chat.Id))
                    foreach (TelegramAction ev in Events)
                        if (text == ev.UserInput)
                        {
                            if (!string.IsNullOrEmpty(ev.EventName))
                                DataInputEventListener.RaiseEvent(this, ev.EventName);
                            if (!string.IsNullOrEmpty(ev.SendText))
                                _Bot.Value.SendMessage(ev.SendText, MessageMode, e.Message.Chat.Id);

                            return;
                        }
            }

            if (_Bot.Value.AllowedChatsContains(e.Message.Chat.Id))
                _Bot.Value.SendMessage(MessageAlreadySubscribed, MessageMode, e.Message.Chat.Id);
            else
            {
                _Bot.Value.SendMessage(MessageNotSubscribed, MessageMode, e.Message.Chat.Id);
            }
        }

        protected override void OnStop()
        {
            if (_Bot != null && _Bot.Free(this, TelegramBot.ReleaseCreateTelegramBotClient))
            {
                _Bot.Value.OnMessage -= C_OnMessage;
                _Bot = null;
            }
        }

        public static TelegramBot CreateTelegramBotClient(string apiKey)
        {
            TelegramBot t = new TelegramBot(apiKey);

            Task<bool> tb = t.TestApiAsync();
            tb.Wait();

            if (tb.Exception == null && tb.Result)
            {
                t.StartReceiving();
            }
            else
            {
                tb.Dispose();
                throw (new Exception("Error creating TelegramBot (Valid api key?)"));
            }
            return t;
        }
    }
}