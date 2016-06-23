using Laster.Core.Data;
using Laster.Core.Interfaces;
using Laster.Inputs.Twitter.Enums;
using System.Collections.Generic;
using TweetSharp;

namespace Laster.Inputs.Twitter
{
    public class TwitterFollowersInput : Interfaces.ITwitterInput
    {
        /// <summary>
        /// Tipo de segidor
        /// </summary>
        public ETwitterFollowType Type { get; set; }
        /// <summary>
        /// Id de usuario
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// Nombre en pantalla
        /// </summary>
        public string ScreenName { get; set; }
        /// <summary>
        /// Incluir entidades de usuario
        /// </summary>
        public bool IncludeUserEntities { get; set; }
        /// <summary>
        /// Saltar estados
        /// </summary>
        public bool? SkipStatus { get; set; }
       
        /// <summary>
        /// Constructor
        /// </summary>
        public TwitterFollowersInput() : base() { Init(); }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="raiseMode">Modo de lanzamiento</param>
        public TwitterFollowersInput(IDataInputRaiseMode raiseMode) : base(raiseMode) { Init(); }

        /// <summary>
        /// Inicializa
        /// </summary>
        void Init()
        {
            Type = ETwitterFollowType.ToHim;
            IncludeUserEntities = false;
            ScreenName = null;
            SkipStatus = null;
        }
        protected override IData OnGetData()
        {
            List<TwitterUser> ls = new List<TwitterUser>();
            TwitterCursorList<TwitterUser> tweets = null;

            do
            {
                if (Type == ETwitterFollowType.Him)
                {
                    tweets = Service.ListFriends(new ListFriendsOptions()
                    {
                        IncludeUserEntities = IncludeUserEntities,
                        ScreenName = ScreenName,
                        SkipStatus = SkipStatus,
                        UserId = UserId,

                        Cursor = tweets == null ? null : tweets.NextCursor
                    });
                }
                else
                {
                    tweets = Service.ListFollowers(new ListFollowersOptions()
                    {
                        IncludeUserEntities = IncludeUserEntities,
                        ScreenName = ScreenName,
                        SkipStatus = SkipStatus,
                        UserId = UserId,

                        Cursor = tweets == null ? null : tweets.NextCursor
                    });
                }

                if (tweets != null)
                {
                    foreach (TwitterUser u in tweets)
                    {
                        ls.Add(u);
                    }
                }

            } while (tweets != null && tweets.NextCursor != null);

            if (ls.Count == 0)
                return new DataEmpty(this);

            return new DataEnumerable(this, ls);
        }
    }
}