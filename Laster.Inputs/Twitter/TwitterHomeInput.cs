using Laster.Core.Data;
using Laster.Core.Interfaces;
using System.Collections.Generic;
using TweetSharp;

namespace Laster.Inputs.Twitter
{
    public class TwitterHomeInput : Interfaces.ITwitterInput
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
        public TwitterHomeInput() : base() { Init(); }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="raiseMode">Modo de lanzamiento</param>
        public TwitterHomeInput(IDataInputRaiseMode raiseMode) : base(raiseMode) { Init(); }

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

            return new DataEnumerable(this, tweets);
        }
    }
}