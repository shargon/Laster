using Laster.Core.Classes;
using Laster.Core.Classes.RaiseMode;
using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
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

        [Description("File for write the chat sessions")]
        public string FileChatStore { get; set; }

        [Category("Authentication")]
        public string SubscribePassword { get; set; }
        [Category("Authentication")]
        public string ApiKey { get; set; }
        [DefaultValue(ParseMode.Html)]
        public ParseMode MessageMode { get; set; }
        [Category("Authentication")]
        public string[] AvailableUsers { get; set; }

        [Category("Messages")]
        public string MessageNotUserAvailable { get; set; }
        [Category("Messages")]
        public string MessageWrongPassword { get; set; }
        [Category("Messages")]
        public string MessageUnsubscribe { get; set; }
        [Category("Messages")]
        public string MessageAlreadySubscribed { get; set; }
        [Category("Messages")]
        public string MessageNotSubscribed { get; set; }


        [Category("Raise Events")]
        public List<TelegramAction> Events { get; set; }


        ShareableClass<TelegramBot, string> _Bot;

        public TelegramBotSubscribeProcess()
        {
            Events = new List<TelegramAction>();
            MessageMode = ParseMode.Html;
            DesignBackColor = Color.Fuchsia;
            MessageNotUserAvailable = "No está configurado dentro de nuestro bot, no se moleste en escribir";
            MessageWrongPassword = "<b>Contraseña correcta.</b>";
            MessageUnsubscribe = "No se le informará de mas notificaciones";
            MessageAlreadySubscribed = "Será informado en cuanto ocurra algún error. En cualquier momento puede usar la palabra clave <b>/stop</b> para parar las notificaciones";
            MessageNotSubscribed = "No autentificado, ingrese la contraseña";

            FileChatStore = "TelegramAllowedChats.json";
        }
        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            object obj = data.GetInternalObject();
            if (obj == null) return null;

            foreach (object o in data)
            {
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

        async void C_OnMessage(object sender, MessageEventArgs e)
        {
            bool ok = AvailableUsers == null;
            if (!ok)
                foreach (string s in AvailableUsers)
                    if (s == e.Message.From.Username) { ok = true; break; }

            if (!ok)
            {
                await _Bot.Value.SendTextMessageAsync(e.Message.Chat.Id, MessageNotUserAvailable, false, false, 0, null, MessageMode);
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
                    await _Bot.Value.SendTextMessageAsync(e.Message.Chat.Id, MessageWrongPassword, false, false, 0, null, MessageMode);
                }
                else
                {
                    if (_Bot.Value.AllowedChatsContains(e.Message.Chat.Id))
                    {
                        if (text.ToLowerInvariant() == "/stop")
                        {
                            _Bot.Value.AllowedChatsRemove(e.Message.Chat.Id);
                            _Bot.Value.AllowedChatsSave(FileChatStore);

                            await _Bot.Value.SendTextMessageAsync(e.Message.Chat.Id, MessageUnsubscribe, false, false, 0, null, MessageMode);
                            return;
                        }
                    }
                }

                if (Events != null)
                    foreach (TelegramAction ev in Events)
                        if (text == ev.UserInput)
                        {
                            if (!string.IsNullOrEmpty(ev.EventName))
                                DataInputEventListener.RaiseEvent(this, ev.EventName);
                            if (!string.IsNullOrEmpty(ev.SendText))
                                await _Bot.Value.SendTextMessageAsync(e.Message.Chat.Id, ev.SendText, false, false, 0, null, MessageMode);

                            return;
                        }
            }

            if (_Bot.Value.AllowedChatsContains(e.Message.Chat.Id))
                await _Bot.Value.SendTextMessageAsync(e.Message.Chat.Id, MessageAlreadySubscribed, false, false, 0, null, MessageMode);
            else
            {
                await _Bot.Value.SendTextMessageAsync(e.Message.Chat.Id, MessageNotSubscribed, false, false, 0, null, MessageMode);
            }
        }

        public override void OnStop()
        {
            if (_Bot != null)
            {
                _Bot.Free(this, ReleaseCreateTelegramBotClient);
                _Bot = null;
            }

            base.OnStop();
        }

        public static TelegramBot CreateTelegramBotClient(string apiKey)
        {
            TelegramBot t = new TelegramBot(apiKey);

            Task<bool> tb = t.TestApiAsync();
            tb.Wait();
            if (tb.Result)
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
        public static void ReleaseCreateTelegramBotClient(TelegramBotClient stop) { stop.StopReceiving(); }
    }
}