using Laster.Core.Data;
using Laster.Core.Interfaces;
using LatchSDK;
using System.Collections.Generic;

namespace Laster.Inputs.Latch
{
    public class LatchInput : IDataInput
    {
        public override string Title { get { return "Latch"; } }

        LatchSDK.Latch _Latch;
        string _AccountId;

        /// <summary>
        /// Id de aplicación
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// Clave secreta
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// Id de operación
        /// </summary>
        public string OperationId { get; set; }
        /// <summary>
        /// Id de cuenta (o codigo de emparejado)
        /// </summary>
        public string AccountId
        {
            get { return _AccountId; }
            set
            {
                LatchSDK.Latch l = new LatchSDK.Latch(AppId, SecretKey);

                if (value.Length <= 10)
                {
                    // Emparejar
                    LatchResponse response = l.Pair(value);
                    if (response != null && response.Error == null)
                    {
                        object o;
                        if (response.Data != null &&
                            response.Data.TryGetValue("accountId", out o) && o != null)
                        {
                            _AccountId = o.ToString();
                            return;
                        }
                    }
                }

                _AccountId = value;
            }
        }
        public override void OnCreate()
        {
            base.OnCreate();
            _Latch = new LatchSDK.Latch(AppId, SecretKey);
        }
        protected override IData OnGetData()
        {
            LatchResponse response = _Latch.Status(AccountId);

            if (response == null || response.Error != null || response.Data == null)
                return null;

            object o;
            if (!response.Data.TryGetValue("operations", out o) || o == null)
                return null;

            if (!(o is Dictionary<string, object>))
                return null;

            Dictionary<string, object> d = (Dictionary<string, object>)o;
            if (!d.TryGetValue(
                string.IsNullOrEmpty(OperationId) ? AppId : OperationId, out o) || o == null)
                return null;

            if (!(o is Dictionary<string, object>))
                return null;

            d = (Dictionary<string, object>)o;
            if (!d.TryGetValue("status", out o) || o == null)
                return null;

            if (o.ToString().ToLowerInvariant() != "on")
                return null;

            return new DataEmpty(this);
        }
        /// <summary>
        /// Liberación de recursos
        /// </summary>
        public override void Dispose() { base.Dispose(); }
    }
}