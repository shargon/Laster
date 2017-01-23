using Laster.Core.Classes.RaiseMode;
using Laster.Core.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Laster.Inputs.Remote
{
    public class MongoTailInput : IDataInput
    {
        /// <summary>
        /// Cadena de conexión
        /// </summary>
        [DefaultValue("mongodb://localhost:27017/db")]
        public string MongoUrl { get; set; }
        /// <summary>
        /// Query
        /// </summary>
        [DefaultValue("Collection")]
        public string Collection { get; set; }

        public override string Title { get { return "Remote - Mongo Tail"; } }

        public MongoTailInput() : base()
        {
            DesignBackColor = Color.DeepPink;

            Collection = "Collection";
            MongoUrl = "mongodb://localhost:27017/db";
            RaiseMode = new DataInputAutomatic()
            {
                RunOnStart = true,
                StopOnStart = false,
            };
        }
        protected override IData OnGetData()
        {
            MongoUrl Url = new MongoUrl(MongoUrl);

            MongoClient _MongoClient = new MongoClient(Url);
            IMongoDatabase _DB = _MongoClient.GetDatabase(Url.DatabaseName, new MongoDatabaseSettings()
            {
                GuidRepresentation = GuidRepresentation.CSharpLegacy,
                ReadPreference = new ReadPreference(ReadPreferenceMode.Primary),
            });

            if (_DB == null) throw new Exception("Database must be exits");

            if (!_DB.ListCollections(new ListCollectionsOptions() { Filter = new BsonDocument("name", Collection) }).Any())
            {
                _DB.CreateCollection(Collection, new CreateCollectionOptions()
                    {
                    Capped = true,
                    MaxSize = int.MaxValue,
                    MaxDocuments = int.MaxValue,
                    AutoIndexId = true,
                    }
                );
            }

            return DataEnumerable(Watch(_DB.GetCollection<object>(Collection)));
        }
        static IEnumerable<object> Watch<T>(IMongoCollection<T> collection) where T : class
        {
            BsonValue lastId = BsonMinKey.Value;

            while (true)
            {
                FilterDefinition<T> query = Builders<T>.Filter.Gt("_id", lastId);

                using (IAsyncCursor<T> cursor = collection.FindSync(query, new FindOptions<T>
                {
                    CursorType = CursorType.TailableAwait,
                    NoCursorTimeout = true,
                    Sort = Builders<T>.Sort.Ascending("$natural")
                }))
                {
                    while (cursor.MoveNext())
                        foreach (T document in cursor.Current)
                        {
                            lastId = document.ToBsonDocument()["_id"];
                            yield return document;
                        }
                }
            }
        }
        /// <summary>
        /// Libración de recursos
        /// </summary>
        public override void Dispose()
        {
            Free();
            base.Dispose();
        }
        protected override void OnStart() { Free(); }
        void Free() { }
    }
}