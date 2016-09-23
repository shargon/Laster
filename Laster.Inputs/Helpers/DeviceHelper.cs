using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Laster.Inputs.Helpers
{
    public sealed class DeviceHelper
    {
        DeviceHelper() { }

        /// <summary>
        /// Enable or disable a device.
        /// </summary>
        /// <param name="classGuid">The class guid of the device. Available in the device manager.</param>
        /// <param name="instanceId">The device instance id of the device. Available in the device manager.</param>
        /// <param name="enable">True to enable, False to disable.</param>
        /// <remarks>Will throw an exception if the device is not Disableable.</remarks>
        public static void SetDeviceEnabled(Guid classGuid, string instanceId, bool enable)
        {
            NativePlugAndPlayHelper.SafeDeviceInfoSetHandle diSetHandle = null;
            try
            {
                // Get the handle to a device information set for all devices matching classGuid that are present on the 
                // system.
                diSetHandle = NativePlugAndPlayHelper.SetupDiGetClassDevs(ref classGuid, null, IntPtr.Zero, NativePlugAndPlayHelper.SetupDiGetClassDevsFlags.Present);
                // Get the device information data for each matching device.
                NativePlugAndPlayHelper.DeviceInfoData[] diData = GetDeviceInfoData(diSetHandle);
                // Find the index of our instance. i.e. the touchpad mouse - I have 3 mice attached...
                int index = GetIndexOfInstance(diSetHandle, diData, instanceId);
                // Disable...
                EnableDevice(diSetHandle, diData[index], enable);
            }
            finally
            {
                if (diSetHandle != null)
                {
                    if (diSetHandle.IsClosed == false)
                    {
                        diSetHandle.Close();
                    }
                    diSetHandle.Dispose();
                }
            }
        }

        static NativePlugAndPlayHelper.DeviceInfoData[] GetDeviceInfoData(NativePlugAndPlayHelper.SafeDeviceInfoSetHandle handle)
        {
            List<NativePlugAndPlayHelper.DeviceInfoData> data = new List<NativePlugAndPlayHelper.DeviceInfoData>();
            NativePlugAndPlayHelper.DeviceInfoData did = new NativePlugAndPlayHelper.DeviceInfoData();
            int didSize = Marshal.SizeOf(did);
            did.Size = didSize;
            int index = 0;
            while (NativePlugAndPlayHelper.SetupDiEnumDeviceInfo(handle, index, ref did))
            {
                data.Add(did);
                index += 1;
                did = new NativePlugAndPlayHelper.DeviceInfoData();
                did.Size = didSize;
            }
            return data.ToArray();
        }

        // Find the index of the particular DeviceInfoData for the instanceId.
        static int GetIndexOfInstance(NativePlugAndPlayHelper.SafeDeviceInfoSetHandle handle, NativePlugAndPlayHelper.DeviceInfoData[] diData, string instanceId)
        {
            const int ERROR_INSUFFICIENT_BUFFER = 122;
            for (int index = 0; index <= diData.Length - 1; index++)
            {
                StringBuilder sb = new StringBuilder(1);
                int requiredSize = 0;
                bool result = NativePlugAndPlayHelper.SetupDiGetDeviceInstanceId(handle.DangerousGetHandle(), ref diData[index], sb, sb.Capacity, out requiredSize);
                if (result == false && Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    sb.Capacity = requiredSize;
                    result = NativePlugAndPlayHelper.SetupDiGetDeviceInstanceId(handle.DangerousGetHandle(), ref diData[index], sb, sb.Capacity, out requiredSize);
                }
                if (result == false)
                    throw new Win32Exception();
                if (instanceId.Equals(sb.ToString()))
                {
                    return index;
                }
            }
            // not found
            return -1;
        }

        // enable/disable...
        static void EnableDevice(NativePlugAndPlayHelper.SafeDeviceInfoSetHandle handle, NativePlugAndPlayHelper.DeviceInfoData diData, bool enable)
        {
            NativePlugAndPlayHelper.PropertyChangeParameters pars = new NativePlugAndPlayHelper.PropertyChangeParameters();
            // The size is just the size of the header, but we've flattened the structure.
            // The header comprises the first two fields, both integer.
            pars.Size = 8;
            pars.DiFunction = NativePlugAndPlayHelper.DiFunction.PropertyChange;
            pars.Scope = NativePlugAndPlayHelper.Scopes.Global;
            if (enable)
            {
                pars.StateChange = NativePlugAndPlayHelper.StateChangeAction.Enable;
            }
            else
            {
                pars.StateChange = NativePlugAndPlayHelper.StateChangeAction.Disable;
            }

            bool result = NativePlugAndPlayHelper.SetupDiSetClassInstallParams(handle, ref diData, ref pars, Marshal.SizeOf(pars));
            if (result == false) throw new Win32Exception();
            result = NativePlugAndPlayHelper.SetupDiCallClassInstaller(NativePlugAndPlayHelper.DiFunction.PropertyChange, handle, ref diData);
            if (result == false)
            {
                int err = Marshal.GetLastWin32Error();
                if (err == (int)NativePlugAndPlayHelper.SetupApiError.NotDisableable) throw new ArgumentException("Device can't be disabled (programmatically or in Device Manager).");
                else if (err >= (int)NativePlugAndPlayHelper.SetupApiError.NoAssociatedClass && err <= (int)NativePlugAndPlayHelper.SetupApiError.OnlyValidateViaAuthenticode)
                    throw new Win32Exception("SetupAPI error: " + ((NativePlugAndPlayHelper.SetupApiError)err).ToString());
                else throw new Win32Exception();
            }
        }
    }
}