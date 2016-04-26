using Laster.Core.Classes.RaiseMode;
using Laster.Core.Data;
using Laster.Core.Interfaces;
using Laster.Inputs.Twitter.Enums;
using System;
using System.Collections.Generic;
using TweetSharp;

namespace Laster.Inputs.Twitter
{
    public class TwitterFollowersInput : ITwitterInput
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
        public TwitterFollowersInput(TimeSpan interval) : base(interval) { Init(); }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trigger">Trigger</param>
        public TwitterFollowersInput(DataInputTrigger trigger) : base(trigger) { Init(); }
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
                return new EmptyData(this);

            return new IEnumerableData(this, ls);
        }
    }
}