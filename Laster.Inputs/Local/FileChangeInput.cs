using Laster.Core.Classes.RaiseMode;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using IO = System.IO;

namespace Laster.Inputs.Local
{
    public class FileChangeInput : IDataInput
    {
        IO.FileSystemWatcher _Watcher;

        [DefaultValue("")]
        public string File { get; set; }
        [DefaultValue(EReturn.ContentAsString)]
        public EReturn Return { get; set; }
        [DefaultValue(SerializationHelper.EEncoding.UTF8)]
        public SerializationHelper.EEncoding Encoding { get; set; }

        public enum EReturn
        {
            ContentAsString,
            ContentAsByteArray,
            FileName
        }

        public override string Title { get { return "Local - Detect file changes"; } }


        long IsTrying = 0;
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public FileChangeInput() : base()
        {
            RaiseMode = new DataInputEventListener()
            {
                EventName = "FileChangeInput",
            };

            Encoding = SerializationHelper.EEncoding.UTF8;
            DesignBackColor = Color.Brown;
        }

        protected override IData OnGetData()
        {
            if (!IO.File.Exists(File)) return DataEmpty();

            switch (Return)
            {
                case EReturn.FileName: return DataObject(File);
                default:
                    {
                        IData ret = DataEmpty();
                        Interlocked.Exchange(ref IsTrying, 1);
                        for (int x = 0; x < 100; x++)
                        {
                            try
                            {
                                switch (Return)
                                {
                                    case EReturn.ContentAsByteArray: ret = DataObject(IO.File.ReadAllBytes(File)); break;
                                    case EReturn.ContentAsString:
                                        {
                                            ret = DataObject(IO.File.ReadAllText(File, SerializationHelper.GetEncoding(Encoding)));
                                            break;
                                        }
                                }
                                break;
                            }
                            catch { }
                            Thread.Sleep(250);
                        }

                        Interlocked.Exchange(ref IsTrying, 0);
                        return ret;
                    }
            }
        }
        public override void OnStart()
        {
            _Watcher = new IO.FileSystemWatcher(IO.Path.GetDirectoryName(File), IO.Path.GetFileName(File))
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
                NotifyFilter = IO.NotifyFilters.LastWrite //| IO.NotifyFilters.Size
            };
            _Watcher.Changed += W_Changed;


            base.OnStart();
        }
        void W_Changed(object sender, IO.FileSystemEventArgs e)
        {
            if (Interlocked.Read(ref IsTrying) == 1) return;

            // Lanzar el evento
            if (RaiseMode is DataInputEventListener)
            {
                DataInputEventListener ev = (DataInputEventListener)RaiseMode;
                ev.RaiseTrigger(e);
            }
        }
        public override void OnStop()
        {
            if (_Watcher != null)
            {
                _Watcher.Dispose();
                _Watcher = null;
            }
            base.OnStop();
        }
    }
}