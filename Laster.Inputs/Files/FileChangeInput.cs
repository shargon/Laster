using Laster.Core.Classes.RaiseMode;
using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using System.Drawing;
using System.Threading;
using IO = System.IO;

namespace Laster.Inputs.Files
{
    public class FileChangeInput : IDataInput
    {
        IO.FileSystemWatcher _Watcher;
        public string File { get; set; }
        public EAction Action { get; set; }
        public SerializationHelper.EEncoding Encoding { get; set; }

        public enum EAction
        {
            ReturnFileString,
            ReturnFileByteArray,
            ReturnFileName
        }

        public override string Title { get { return "Files - Detect changes"; } }


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

            DesignBackColor = Color.Brown;
        }

        protected override IData OnGetData()
        {
            if (!IO.File.Exists(File)) return DataEmpty();

            switch (Action)
            {
                case EAction.ReturnFileName: return DataObject(File);
                default:
                    {
                        IData ret = DataEmpty();
                        Interlocked.Exchange(ref IsTrying, 1);
                        for (int x = 0; x < 100; x++)
                        {
                            try
                            {
                                switch (Action)
                                {
                                    case EAction.ReturnFileByteArray: ret = DataObject(IO.File.ReadAllBytes(File)); break;
                                    case EAction.ReturnFileString:
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