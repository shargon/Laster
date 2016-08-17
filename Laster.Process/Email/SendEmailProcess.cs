using Laster.Core.Data;
using Laster.Core.Enums;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Laster.Process.Email
{
    public class SendEmailProcess : IDataProcess
    {
        /// <summary>
        /// Formato
        /// </summary>
        [DefaultValue("application/json")]
        [Category("Attachment")]
        public string ContentType { get; set; }
        /// <summary>
        /// Codificación
        /// </summary>
        [DefaultValue(SerializationHelper.EEncoding.UTF8)]
        [Category("Attachment")]
        public SerializationHelper.EEncoding StringEncoding { get; set; }
        /// <summary>
        /// AttachmentName
        /// </summary>
        [DefaultValue("")]
        [Category("Attachment")]
        public string AttachmentName { get; set; }

        #region SMTP
        /// <summary>
        /// Host
        /// </summary>
        [DefaultValue("")]
        [Category("Connection")]
        public string SmtpHost { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        [DefaultValue(587)]
        [Category("Connection")]
        public int SmtpPort { get; set; }
        /// <summary>
        /// Activar SSL
        /// </summary>
        [DefaultValue(true)]
        [Category("Connection")]
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        [DefaultValue("")]
        [Category("Credentials")]
        public string User { get; set; }
        /// <summary>
        /// Host
        /// </summary>
        [DefaultValue("")]
        [Category("Credentials")]
        public string Password { get; set; }

        /// <summary>
        /// From
        /// </summary>
        [DefaultValue("")]
        [Category("Email")]
        public string From { get; set; }
        /// <summary>
        /// To
        /// </summary>
        [DefaultValue(null)]
        [Category("Email")]
        public string[] To { get; set; }
        /// <summary>
        /// BCC
        /// </summary>
        [DefaultValue(null)]
        [Category("Email")]
        public string[] BCC { get; set; }
        /// <summary>
        /// Subject
        /// </summary>
        [DefaultValue("")]
        [Category("Email")]
        public string Subject { get; set; }
        /// <summary>
        /// Body
        /// </summary>
        [DefaultValue("")]
        [Category("Email")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Body { get; set; }
        /// <summary>
        /// SendEmpty
        /// </summary>
        [DefaultValue(false)]
        [Category("Email")]
        public bool SendEmpty { get; set; }
        /// <summary>
        /// SendAsync
        /// </summary>
        [DefaultValue(true)]
        [Category("Email")]
        public bool SendAsync { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SendEmailProcess()
        {
            ContentType = SerializationHelper.GetMimeType(SerializationHelper.EFormat.Json);
            StringEncoding = SerializationHelper.EEncoding.UTF8;
            DesignBackColor = Color.LightSalmon;
            EnableSsl = true;
            SendAsync = true;
            SendEmpty = false;
            SmtpPort = 587;
        }

        SmtpClient _Smtp;

        public override string Title { get { return "Email - Send"; } }

        public override void OnStart()
        {
            base.OnStart();

            if (_Smtp != null) _Smtp.Dispose();

            _Smtp = new SmtpClient(SmtpHost, SmtpPort);
            _Smtp.Credentials = new NetworkCredential(User, Password);
            _Smtp.EnableSsl = EnableSsl;
        }
        public override void OnStop()
        {
            if (_Smtp != null)
            {
                _Smtp.Dispose();
                _Smtp = null;
            }

            base.OnStop();
        }

        protected override IData OnProcessData(IData data, EEnumerableDataState state)
        {
            if (data == null || data is DataEmpty)
            {
                if (!SendEmpty) return data;
            }

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(From);
            msg.Subject = Subject;
            msg.Body = Body;

            foreach (string t in To) msg.To.Add(t);
            foreach (string t in BCC) msg.Bcc.Add(t);

            using (MemoryStream ms = data.ToStream(StringEncoding))
                msg.Attachments.Add(new Attachment(ms, AttachmentName, ContentType));

            if (SendAsync) _Smtp.SendAsync(msg, null);
            else _Smtp.Send(msg);

            return data;
        }
    }
}