#pragma once

EVT_WDF_DEVICE_SELF_MANAGED_IO_INIT DsHidMini_EvtWdfDeviceSelfManagedIoInit;
EVT_WDF_DEVICE_SELF_MANAGED_IO_SUSPEND DsHidMini_EvtWdfDeviceSelfManagedIoSuspend;

EVT_WDF_DEVICE_PREPARE_HARDWARE DsHidMini_EvtDevicePrepareHardware;
EVT_WDF_DEVICE_D0_ENTRY DsHidMini_EvtDeviceD0Entry;
EVT_WDF_DEVICE_D0_EXIT DsHidMini_EvtDeviceD0Exit;
