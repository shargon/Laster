using Laster.Core.Classes.RaiseMode;
using Laster.Core.Data;
using Laster.Core.Interfaces;
using System;
using System.Collections.Generic;
using TweetSharp;

namespace Laster.Inputs.Twitter
{
    public class TwitterHomeInput : ITwitterInput
    {
        /// <summary>
        /// Detalles del publicador
        /// </summary>
        public bool ContributorDetails { get; set; }
        /// <summary>
        /// Escluir replicas
        /// </summary>
        public bool ExcludeReplies { get; set; }
        /// <summary>
        /// Incluir entidades
        /// </summary>
        public bool IncludeEntities { get; set; }
        /// <summary>
        /// Hacer trim al usuario
        /// </summary>
        public bool TrimUser { get; set; }
        /// <summary>
        /// Cantidad
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TwitterHomeInput(TimeSpan interval) : base(interval) { Init(); }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="trigger">Trigger</param>
        public TwitterHomeInput(DataInputTrigger trigger) : base(trigger) { Init(); }
        /// <summary>
        /// Inicializa
        /// </summary>
        void Init()
        {
            Count = 10;
            TrimUser = false;
            IncludeEntities = true;
            ExcludeReplies = false;
            ContributorDetails = false;
        }
        protected override IData OnGetData()
        {
            IEnumerable<TwitterStatus> tweets = Service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions()
            {
                ContributorDetails = ContributorDetails,
                Count = Count,
                ExcludeReplies = ExcludeReplies,
                IncludeEntities = IncludeEntities,
                TrimUser = TrimUser,
            });

            return new IEnumerableData(this, tweets);
        }
    }
}