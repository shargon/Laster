using Laster.Core.Classes.RaiseMode;
using Laster.Core.Enums;
using Laster.Core.Interfaces;
using Laster.Inputs.Helpers;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Laster.Inputs.Local
{
    /// <summary>
    /// Only windows
    /// </summary>
    public class MultimediaDevicesInput : IDataInput
    {
        string[] _Send;
        MultiMediaNotificationListener _Notificator;

        public override string Title { get { return "Local - Multimedia devices"; } }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public MultimediaDevicesInput() : base()
        {
            RaiseMode = new DataInputAutomatic() { StopOnStart = false, RunOnStart = true };
            DesignBackColor = Color.Green;
        }
        protected override IData OnGetData()
        {
            return Reduce(EReduceZeroEntries.Empty, _Send);
        }
        protected override void OnStart()
        {
            if (_Notificator != null)
            {
                _Notificator.Dispose();
                _Notificator = null;
            }

            _Notificator = new MultiMediaNotificationListener();
            _Notificator.OnChange += OnChange;
            _Send = _Notificator.GetConnected();
        }
        protected override void OnStop()
        {
            _Send = null;

            if (_Notificator != null)
            {
                _Notificator.Dispose();
                _Notificator = null;
            }
            base.OnStop();
        }
        void OnChange(object sender, EventArgs e)
        {
            string[] send = _Notificator.GetConnected();
            if (_Send != null)
            {
                if (string.Join("\n", send) == string.Join("\n", _Send)) return;
            }

            _Send = send;

            if (RaiseMode != null && RaiseMode is ITriggerRaiseMode)
                ((ITriggerRaiseMode)RaiseMode).RaiseTrigger(EventArgs.Empty);
        }

        #region api
        class MultiMediaNotificationListener : MultimediaDevicesHelper.IMMNotificationClient, IDisposable
        {
            readonly MultimediaDevicesHelper.IMMDeviceEnumerator _deviceEnumerator;
            public event EventHandler OnChange;

            public MultiMediaNotificationListener()
            {
                if (Environment.OSVersion.Version.Major < 6)
                    throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");

                _deviceEnumerator = (MultimediaDevicesHelper.IMMDeviceEnumerator)new MultimediaDevicesHelper.MMDeviceEnumerator();
                _deviceEnumerator.RegisterEndpointNotificationCallback(this);
            }
            ~MultiMediaNotificationListener()
            {
                Dispose(false);
            }

            public string[] GetConnected()
            {
                MultimediaDevicesHelper.IMMDeviceCollection deviceCollection;
                _deviceEnumerator.EnumAudioEndpoints(MultimediaDevicesHelper.EDataFlow.eRender, (uint)MultimediaDevicesHelper.DeviceState.DEVICE_STATE_ACTIVE, out deviceCollection);

                uint deviceCount = 0;
                deviceCollection.GetCount(out deviceCount);

                string[] ret = new string[deviceCount];
                for (uint i = 0; i < deviceCount; i++)
                {
                    MultimediaDevicesHelper.IMMDevice device;
                    deviceCollection.Item(i, out device);

                    MultimediaDevicesHelper.IPropertyStore propertyStore;
                    device.OpenPropertyStore((uint)MultimediaDevicesHelper.STGM.STGM_READ, out propertyStore);

                    MultimediaDevicesHelper.PROPVARIANT property;
                    propertyStore.GetValue(ref MultimediaDevicesHelper.PropertyKey.PKEY_Device_DeviceDesc, out property);

                    string value = (string)property.Value;
                    Marshal.ReleaseComObject(propertyStore);
                    Marshal.ReleaseComObject(device);

                    ret[i] = value;
                }

                Marshal.ReleaseComObject(deviceCollection);
                return ret;
            }

            public void OnDeviceStateChanged(string pwstrDeviceId, uint dwNewState)
            {
                //IMMDevice device;
                //_deviceEnumerator.GetDevice(pwstrDeviceId, out device);

                //IPropertyStore propertyStore;
                //device.OpenPropertyStore((uint)STGM.STGM_READ, out propertyStore);

                ////Trace.WriteLine(string.Format("OnDeviceStateChanged:\n  Device Id {0}\tDevice State {1}", pwstrDeviceId, (DeviceState)dwNewState));

                //var properties = PropertyKey.GetPropertyKeys()
                //    .Select(
                //    propertyKey =>
                //    {
                //        PROPVARIANT property;
                //        propertyStore.GetValue(ref propertyKey, out property);
                //        return new { Key = PropertyKey.GetKeyName(propertyKey), Value = property.Value };
                //    })
                //    .Where(@t => @t.Value != null);

                //foreach (var property in properties)
                //{
                //    Trace.WriteLine(string.Format("    {0}\t{1}", property.Key, property.Value));
                //}

                //Marshal.ReleaseComObject(propertyStore);
                //Marshal.ReleaseComObject(device);

                OnChange?.Invoke(this, EventArgs.Empty);
            }
            public void OnDefaultDeviceChanged(MultimediaDevicesHelper.EDataFlow dataFlow, MultimediaDevicesHelper.ERole deviceRole, string pwstrDefaultDeviceId) { OnChange?.Invoke(this, EventArgs.Empty); }
            public void OnDeviceAdded(string deviceId) { OnChange?.Invoke(this, EventArgs.Empty); }
            public void OnDeviceRemoved(string deviceId) { OnChange?.Invoke(this, EventArgs.Empty); }
            public void OnPropertyValueChanged(string pwstrDeviceId, MultimediaDevicesHelper.PropertyKey key) { }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            private void Dispose(bool disposing)
            {
                _deviceEnumerator.UnregisterEndpointNotificationCallback(this);
                Marshal.ReleaseComObject(_deviceEnumerator);
            }
        }
        #endregion
    }
}