using Laster.Core.Classes.RaiseMode;
using Laster.Core.Interfaces;
using Laster.Inputs.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Management;

namespace Laster.Inputs.Local
{
    /// <summary>
    /// Only windows
    /// </summary>
    public class PlugAndPlayDevicesInput : IDataInput
    {
        // http://stackoverflow.com/questions/3331043/get-list-of-connected-usb-devices
        public class PnpDevice
        {
            #region Propiedades
            public int Availability { get; set; }
            public string Caption { get; set; }
            public string ClassGuid { get; set; }
            public string[] CompatibleID { get; set; }
            public long ConfigManagerErrorCode { get; set; }
            public bool ConfigManagerUserConfig { get; set; }
            public string CreationClassName { get; set; }
            public string Description { get; set; }
            public string DeviceID { get; set; }
            public bool ErrorCleared { get; set; }
            public string ErrorDescription { get; set; }
            public string[] HardwareID { get; set; }
            public DateTime InstallDate { get; set; }
            public long LastErrorCode { get; set; }
            public string Manufacturer { get; set; }
            public string Name { get; set; }
            public string PNPClass { get; set; }
            public string PNPDeviceID { get; set; }
            public int[] PowerManagementCapabilities { get; set; }
            public bool PowerManagementSupported { get; set; }
            public bool Present { get; set; }
            public string Service { get; set; }
            public string Status { get; set; }
            public int StatusInfo { get; set; }
            public string SystemCreationClassName { get; set; }
            public string SystemName { get; set; }
            #endregion

            public PnpDevice(ManagementBaseObject queryObj)
            {
                Availability = Convert.ToInt32(CheckNull(queryObj["Availability"], -1));
                Caption = CheckNull(queryObj["Caption"], "").ToString();
                ClassGuid = CheckNull(queryObj["ClassGuid"], "").ToString();
                CompatibleID = (string[])CheckNull(queryObj["CompatibleId"], null);
                ConfigManagerErrorCode = Convert.ToInt64(CheckNull(queryObj["ConfigManagerErrorCode"], -1));
                ConfigManagerUserConfig = Convert.ToBoolean(CheckNull(queryObj["ConfigManagerUserConfig"], false));
                CreationClassName = CheckNull(queryObj["CreationClassName"], "").ToString();
                Description = CheckNull(queryObj["Description"], "").ToString();
                DeviceID = CheckNull(queryObj["DeviceID"], "").ToString();
                ErrorCleared = Convert.ToBoolean(CheckNull(queryObj["ErrorCleared"], false));
                HardwareID = (string[])CheckNull(queryObj["HardwareID"], null);
                InstallDate = Convert.ToDateTime(CheckNull(queryObj["InstallDate"], DateTime.MinValue));
                LastErrorCode = Convert.ToInt64(CheckNull(queryObj["LastErrorCode"], -1));
                Manufacturer = CheckNull(queryObj["Manufacturer"], "").ToString();
                Name = CheckNull(queryObj["Name"], "").ToString();
                PNPClass = CheckNull(queryObj["PNPClass"], "").ToString();
                PNPDeviceID = CheckNull(queryObj["PNPDeviceID"], "").ToString();
                PowerManagementCapabilities = (int[])CheckNull(queryObj["PowerManagementCapabilities"], null);
                PowerManagementSupported = Convert.ToBoolean(CheckNull(queryObj["PowerManagementSupported"], false));
                Present = Convert.ToBoolean(CheckNull(queryObj["Present"], false));
                Service = CheckNull(queryObj["Service"], "").ToString();
                Status = CheckNull(queryObj["Status"], "").ToString();
                StatusInfo = Convert.ToInt32(CheckNull(queryObj["StatusInfo"], -1));
                SystemCreationClassName = CheckNull(queryObj["SystemCreationClassName"], "").ToString();
                SystemName = CheckNull(queryObj["SystemName"], "").ToString();
            }

            // Disable (Requeire x86/x64
            public void Enable() { ChangeEnable(true); }
            // Disable (Requeire x86/x64
            public void Disable() { ChangeEnable(false); }

            void ChangeEnable(bool enable)
            {
                ManagementObjectSearcher searcher = null;
                try
                {
                    searcher = new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_PnPEntity WHERE DeviceID='" +
                        DeviceID.Replace("\\", "\\\\") + "'");

                    foreach (ManagementObject queryObj in searcher.Get())
                        try
                        {
                            DeviceHelper.SetDeviceEnabled(Guid.Parse(ClassGuid), PNPDeviceID, enable);

                            //bool reqReboot = false;
                            //object oreqReboot;
                            //if (!enable)
                            //    oreqReboot = queryObj.InvokeMethod("Disable", new object[] { reqReboot });
                            //else
                            //    oreqReboot = queryObj.InvokeMethod("Enable", new object[] { reqReboot });
                        }
                        catch { }
                        finally { queryObj.Dispose(); }
                }
                catch { }
                finally
                {
                    if (searcher != null)
                    {
                        searcher.Dispose();
                        searcher = null;
                    }
                }
            }

            object CheckNull(object v1, object v2)
            {
                if (v1 == null) return v2;
                return v1;
            }

            public override string ToString() { return DeviceID; }
        }

        ManagementEventWatcher removeWatcher, insertWatcher;
        List<PnpDevice> items = new List<PnpDevice>();

        [DefaultValue(EReturn.DeviceID)]
        public EReturn Return { get; set; }

        public enum EReturn
        {
            DeviceID,
            Object,
        }

        public override string Title { get { return "Local - Plug&Play devices"; } }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public PlugAndPlayDevicesInput() : base()
        {
            RaiseMode = new DataInputAutomatic() { StopOnStart = false, RunOnStart = true };
            DesignBackColor = Color.Green;
        }
        protected override IData OnGetData()
        {
            switch (Return)
            {
                case EReturn.DeviceID:
                    {
                        List<string> ls = new List<string>();
                        foreach (PnpDevice p in items.ToArray()) ls.Add(p.DeviceID);
                        return Reduce(EZeroEntries.Empty, ls.ToArray());
                    }
                default:
                case EReturn.Object: return Reduce(EZeroEntries.Empty, items.ToArray());
            }
        }
        protected override void OnStart()
        {
            // Fill PnpDevices
            items.Clear();
            ManagementObjectSearcher searcher = null;
            try
            {
                searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity");

                foreach (ManagementObject queryObj in searcher.Get())
                    try
                    {
                        items.Add(new PnpDevice(queryObj));
                    }
                    catch { }
                    finally { queryObj.Dispose(); }
            }
            catch { }
            finally
            {
                if (searcher != null)
                {
                    searcher.Dispose();
                    searcher = null;
                }
            }

            // Start waiting changes
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");

            insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertWatcher.Start();

            // TargetInstance ISA 'Win32_USBHub' OR 
            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
            removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removeWatcher.Start();
        }

        void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            PnpDevice p = new PnpDevice((ManagementBaseObject)e.NewEvent["TargetInstance"]);
            PnpDevice d = Find(p);
            if (d == null)
            {
                items.Add(p);

                p.Disable();

                if (RaiseMode != null && RaiseMode is ITriggerRaiseMode)
                    ((ITriggerRaiseMode)RaiseMode).RaiseTrigger(EventArgs.Empty);
            }
        }
        void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            PnpDevice p = new PnpDevice((ManagementBaseObject)e.NewEvent["TargetInstance"]);
            PnpDevice d = Find(p);
            if (d != null)
            {
                items.Remove(d);

                if (RaiseMode != null && RaiseMode is ITriggerRaiseMode)
                    ((ITriggerRaiseMode)RaiseMode).RaiseTrigger(EventArgs.Empty);
            }
        }
        PnpDevice Find(PnpDevice p)
        {
            lock (items)
                foreach (PnpDevice cur in items)
                {
                    if (cur.DeviceID == p.DeviceID) return cur;
                }
            return null;
        }
        protected override void OnStop()
        {
            items.Clear();

            if (removeWatcher != null)
            {
                removeWatcher.EventArrived -= new EventArrivedEventHandler(DeviceRemovedEvent);
                //removeWatcher.Stop();
                removeWatcher.Dispose();
                removeWatcher = null;
            }
            if (insertWatcher != null)
            {
                insertWatcher.EventArrived -= new EventArrivedEventHandler(DeviceInsertedEvent);
                //insertWatcher.Stop();
                insertWatcher.Dispose();
                insertWatcher = null;
            }

            base.OnStop();
        }
    }
}