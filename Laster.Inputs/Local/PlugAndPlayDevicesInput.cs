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

        public class LogicalDevice
        {
            #region Propiedades

            // https://msdn.microsoft.com/en-us/library/aa387884(v=vs.85).aspx

            //string Caption;
            //string Description;
            //datetime InstallDate;
            //string Name;
            //string Status;
            //uint16 Availability;
            //uint32 ConfigManagerErrorCode;
            //boolean ConfigManagerUserConfig;
            //string CreationClassName;
            //string DeviceID;
            //uint16 PowerManagementCapabilities[];
            //boolean ErrorCleared;
            //string ErrorDescription;
            //uint32 LastErrorCode;
            //string PNPDeviceID;
            //boolean PowerManagementSupported;
            //uint16 StatusInfo;
            //string SystemCreationClassName;
            //string SystemName;

            public string Caption { get; set; }
            public string Description { get; set; }
            public DateTime InstallDate { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            //public int Availability { get; set; }
            //public ulong ConfigManagerErrorCode { get; set; }
            //public bool ConfigManagerUserConfig { get; set; }
            public string CreationClassName { get; set; }
            public string DeviceID { get; set; }
            //public uint[] PowerManagementCapabilities { get; set; }
            //public bool ErrorCleared { get; set; }
            public string ErrorDescription { get; set; }
            public ulong LastErrorCode { get; set; }
            public string PNPDeviceID { get; set; }
            public bool PowerManagementSupported { get; set; }
            //public int StatusInfo { get; set; }
            public string SystemCreationClassName { get; set; }
            public string SystemName { get; set; }
            #endregion

            public LogicalDevice(ManagementBaseObject queryObj)
            {
                //Availability = Convert.ToInt32(CheckNull(queryObj["Availability"], (int)0));
                Caption = CheckNull(queryObj["Caption"], "").ToString();
                //ConfigManagerUserConfig = Convert.ToBoolean(CheckNull(queryObj["ConfigManagerUserConfig"], false));
                CreationClassName = CheckNull(queryObj["CreationClassName"], "").ToString();
                Description = CheckNull(queryObj["Description"], "").ToString();
                //ErrorCleared = Convert.ToBoolean(CheckNull(queryObj["ErrorCleared"], false));
                ErrorDescription = CheckNull(queryObj["ErrorDescription"], "").ToString();
                InstallDate = Convert.ToDateTime(CheckNull(queryObj["InstallDate"], DateTime.MinValue));
                LastErrorCode = Convert.ToUInt64(CheckNull(queryObj["LastErrorCode"], (ulong)0));
                PNPDeviceID = CheckNull(queryObj["PNPDeviceID"], "").ToString();
                //PowerManagementCapabilities = (uint[])CheckNull(queryObj["PowerManagementCapabilities"], null);
                PowerManagementSupported = Convert.ToBoolean(CheckNull(queryObj["PowerManagementSupported"], false));
                Status = CheckNull(queryObj["Status"], "").ToString();
                //StatusInfo = Convert.ToInt32(CheckNull(queryObj["StatusInfo"], (int)0));
                SystemCreationClassName = CheckNull(queryObj["SystemCreationClassName"], "").ToString();
                SystemName = CheckNull(queryObj["SystemName"], "").ToString();
                //ConfigManagerErrorCode = Convert.ToUInt64(CheckNull(queryObj["ConfigManagerErrorCode"], (ulong)0));
                DeviceID = CheckNull(queryObj["DeviceID"], "").ToString();
                Name = CheckNull(queryObj["Name"], "").ToString();
            }
            public T CheckNull<T>(object v1, T v2)
            {
                if (v1 == null) return v2;
                return (T)v1;
            }
        }

        public class PnpDevice : LogicalDevice
        {
            #region Propiedades
            public string ClassGuid { get; set; }
            public string[] CompatibleID { get; set; }
            public string[] HardwareID { get; set; }
            public string Manufacturer { get; set; }
            public string PNPClass { get; set; }
            public bool Present { get; set; }
            public string Service { get; set; }
            #endregion

            public PnpDevice(ManagementBaseObject queryObj) : base(queryObj)
            {
                ClassGuid = CheckNull(queryObj["ClassGuid"], "").ToString();
                CompatibleID = CheckNull<string[]>(queryObj["CompatibleId"], null);
                HardwareID = CheckNull<string[]>(queryObj["HardwareID"], null);
                Manufacturer = CheckNull(queryObj["Manufacturer"], "").ToString();
                PNPClass = CheckNull(queryObj["PNPClass"], "").ToString();
                Present = Convert.ToBoolean(CheckNull(queryObj["Present"], false));
                Service = CheckNull(queryObj["Service"], "").ToString();
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

            public override string ToString() { return DeviceID; }
        }

        public class UsbDevice : LogicalDevice
        {
            #region Propiedades
            //public ushort ClassCode { get; set; }
            //public ushort[] CurrentAlternateSettings { get; set; }
            //public ushort CurrentConfigValue { get; set; }
            //public bool GangSwitched { get; set; }
            //public ushort NumberOfConfigs { get; set; }
            //public ushort NumberOfPorts { get; set; }
            //public ushort ProtocolCode { get; set; }
            //public ushort SubclassCode { get; set; }
            //public uint USBVersion { get; set; }
            #endregion

            public UsbDevice(ManagementBaseObject queryObj) : base(queryObj)
            {
                //ClassCode = Convert.ToUInt16(CheckNull(queryObj["ClassCode"], (ushort)0));
                //CurrentAlternateSettings = (ushort[])CheckNull(queryObj["CurrentAlternateSettings"], null);
                //CurrentConfigValue = Convert.ToUInt16(CheckNull(queryObj["CurrentConfigValue"], (ushort)0));
                //ConfigManagerUserConfig = Convert.ToBoolean(CheckNull(queryObj["ConfigManagerUserConfig"], false));
                //GangSwitched = Convert.ToBoolean(CheckNull(queryObj["GangSwitched"], false));
                //NumberOfConfigs = Convert.ToUInt16(CheckNull(queryObj["NumberOfConfigs"], (ushort)0));
                //NumberOfPorts = Convert.ToUInt16(CheckNull(queryObj["NumberOfPorts"], (ushort)0));
                //ProtocolCode = Convert.ToUInt16(CheckNull(queryObj["ProtocolCode"], (ushort)0));
                //SubclassCode = Convert.ToUInt16(CheckNull(queryObj["SubclassCode"], (ushort)0));
                //USBVersion = Convert.ToUInt32(CheckNull(queryObj["USBVersion"], (uint)0));
            }
            public override string ToString() { return DeviceID; }
        }

        public enum EReturn
        {
            // Plug&Play
            PnpDeviceID,
            PnpObject,

            // Usb
            UsbDeviceID,
            UsbObject,
        }

        ManagementEventWatcher removeWatcher, insertWatcher;
        List<LogicalDevice> items = new List<LogicalDevice>();

        [DefaultValue(EReturn.PnpDeviceID)]
        public EReturn Return { get; set; }

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
                case EReturn.PnpDeviceID:
                case EReturn.UsbDeviceID:
                    {
                        List<string> ls = new List<string>();
                        foreach (LogicalDevice p in items.ToArray()) ls.Add(p.DeviceID);
                        return Reduce(EZeroEntries.Empty, ls.ToArray());
                    }
                default:
                case EReturn.PnpObject:
                case EReturn.UsbObject: return Reduce(EZeroEntries.Empty, items.ToArray());
            }
        }
        protected override void OnStart()
        {
            // Fill PnpDevices
            items.Clear();

            string obj = "Win32_PnPEntity";
            switch (Return)
            {
                case EReturn.PnpDeviceID:
                case EReturn.PnpObject: { obj = "Win32_PnPEntity"; break; }
                case EReturn.UsbDeviceID:
                case EReturn.UsbObject: { obj = "Win32_USBHub"; break; }
            }

            ManagementObjectSearcher searcher = null;
            try
            {
                searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + obj);

                foreach (ManagementObject queryObj in searcher.Get())
                    try
                    {
                        switch (Return)
                        {
                            case EReturn.PnpDeviceID:
                            case EReturn.PnpObject: { items.Add(new PnpDevice(queryObj)); break; }
                            case EReturn.UsbDeviceID:
                            case EReturn.UsbObject: { items.Add(new UsbDevice(queryObj)); break; }
                        }
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
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA '" + obj + "'");

            insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            insertWatcher.Start();

            // TargetInstance ISA 'Win32_USBHub' OR 
            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA '" + obj + "'");
            removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            removeWatcher.Start();
        }
        void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            bool raise = false;
            switch (Return)
            {
                case EReturn.PnpDeviceID:
                case EReturn.PnpObject:
                    {

                        PnpDevice p = new PnpDevice((ManagementBaseObject)e.NewEvent["TargetInstance"]);
                        PnpDevice d = Find(p);
                        if (d == null)
                        {
                            items.Add(p);
                            raise = true;
                        }
                        break;
                    }
                case EReturn.UsbDeviceID:
                case EReturn.UsbObject:
                    {
                        UsbDevice p = new UsbDevice((ManagementBaseObject)e.NewEvent["TargetInstance"]);
                        UsbDevice d = Find(p);
                        if (d == null)
                        {
                            items.Add(p);
                            raise = true;
                        }
                        break;
                    }
            }

            if (raise && RaiseMode != null && RaiseMode is ITriggerRaiseMode)
                ((ITriggerRaiseMode)RaiseMode).RaiseTrigger(EventArgs.Empty);
        }
        void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            bool raise = false;
            switch (Return)
            {
                case EReturn.PnpDeviceID:
                case EReturn.PnpObject:
                    {
                        PnpDevice p = new PnpDevice((ManagementBaseObject)e.NewEvent["TargetInstance"]);
                        PnpDevice d = Find(p);
                        if (d != null)
                        {
                            items.Remove(d);
                            raise = true;
                        }
                        break;
                    }
                case EReturn.UsbDeviceID:
                case EReturn.UsbObject:
                    {
                        UsbDevice p = new UsbDevice((ManagementBaseObject)e.NewEvent["TargetInstance"]);
                        UsbDevice d = Find(p);
                        if (d != null)
                        {
                            items.Remove(d);
                            raise = true;
                        }
                        break;
                    }
            }

            if (raise && RaiseMode != null && RaiseMode is ITriggerRaiseMode)
                ((ITriggerRaiseMode)RaiseMode).RaiseTrigger(EventArgs.Empty);
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
        T Find<T>(T p) where T : LogicalDevice
        {
            lock (items)
                foreach (LogicalDevice cur in items)
                {
                    if (cur.DeviceID == p.DeviceID) return (T)cur;
                }
            return null;
        }
    }
}