using Laster.Core.Interfaces;
using System.ComponentModel;
using TweetSharp;

namespace Laster.Inputs.Twitter.Interfaces
{
    public class ITwitterInput : IDataInput
    {
        TwitterService _Service = null;
        string _ConsumerKey, _ConsumerSecret;
        string _AccessToken, _AccessTokenSecret;

        [Category("Api")]
        public string ConsumerKey
        {
            get { return _ConsumerKey; }
            set
            {
                if (_ConsumerKey == value) return;
                _ConsumerKey = value;
                Regenerate();
            }
        }
        [Category("Api")]
        public string ConsumerSecret
        {
            get { return _ConsumerSecret; }
            set
            {
                if (_ConsumerSecret == value) return;
                _ConsumerSecret = value;
                Regenerate();
            }
        }
        [Category("Api")]
        public string AccessToken
        {
            get { return _AccessToken; }
            set
            {
                if (_AccessToken == value) return;
                _AccessToken = value;
                Regenerate();
            }
        }
        [Category("Api")]
        public string AccessTokenSecret
        {
            get { return _AccessTokenSecret; }
            set
            {
                if (_AccessTokenSecret == value) return;
                _AccessTokenSecret = value;
                Regenerate();
            }
        }

        internal TwitterService Service { get { return _Service; } }

        /// <summary>
        /// Constructor
        /// </summary>
        protected ITwitterInput() : base() { }

        /// <summary>
        /// Liberación de recursos
        /// </summary>
        void CleanResources()
        {
            if (_Service != null)
            {
                _Service = null;
            }
        }
        /// <summary>
        /// Regenera el cliente
        /// </summary>
        void Regenerate()
        {
            CleanResources();
            if (_Service == null)
            {
                _Service = new TwitterService(ConsumerKey, ConsumerSecret);
                _Service.AuthenticateWith(AccessToken, AccessTokenSecret);
            }
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            CleanResources();
        }
        protected override IData OnGetData()
        {
            return DataEmpty();
        }
    }
}