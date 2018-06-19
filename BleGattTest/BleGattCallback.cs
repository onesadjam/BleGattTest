using System;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Widget;
using Java.Util;

namespace BleGattTest
{
	public class BleGattCallback : BluetoothGattCallback
	{
		private const string _ConfigurationServiceUuid = "3052c6a5-0928-a48f-8d40-4375aaa9a55e";
		private const string _ConfigurationKeyCharacteristicUuid = "2dcb38cd-f329-49aa-6749-6d6e6bb8c6b2";
		private const string _ConfigurationValue = "02D015424321C334A48CFEAF86109C4F";
		private readonly UUID _ConfigurationKeyCharacteristic = UUID.FromString(_ConfigurationKeyCharacteristicUuid);
		private readonly UUID _ConfigurationService = UUID.FromString(_ConfigurationServiceUuid);
		private readonly MainActivity _MainActivity;

		public BleGattCallback(MainActivity activity)
		{
			_MainActivity = activity;
		}

		public override void OnCharacteristicWrite(
			BluetoothGatt gatt,
			BluetoothGattCharacteristic characteristic,
			[Android.Runtime.GeneratedEnum] GattStatus status)
		{
			base.OnCharacteristicWrite(gatt, characteristic, status);

			_MainActivity.AppendLogText(status != GattStatus.Success
				? "OnCharacteristicWrite: Write failed."
				: "OnCharacteristicWrite: Write succeded.");
			gatt.Disconnect();
		}

		public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
		{
			base.OnConnectionStateChange(gatt, status, newState);

			switch (newState)
			{
				case ProfileState.Disconnected:
					_MainActivity.AppendLogText("Disconnected");
					break;
				case ProfileState.Connected:
					_MainActivity.AppendLogText("Connected");
					gatt.DiscoverServices();
					break;
			}
		}

		public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
		{
			if (status != GattStatus.Success)
			{
				_MainActivity.AppendLogText("Failed to discover services");
				gatt.Disconnect();
				return;
			}
			_MainActivity.AppendLogText("Services Discovered");

			var configurationService = gatt.GetService(_ConfigurationService);
			if (configurationService == null)
			{
				_MainActivity.AppendLogText("Unable to find configuration service");
				gatt.Disconnect();
				return;
			}

			var configurationKeyCharacteristic = configurationService.GetCharacteristic(_ConfigurationKeyCharacteristic);
			if (configurationKeyCharacteristic == null)
			{
				_MainActivity.AppendLogText("Unable to find characteristic");
				gatt.Disconnect();
				return;
			}

			var configKeyBytes = StringToByteArrayFastest(_ConfigurationValue);
			configurationKeyCharacteristic.SetValue(configKeyBytes);
			if (!gatt.WriteCharacteristic(configurationKeyCharacteristic))
			{
				_MainActivity.AppendLogText("Failed to write characteristic");
			}
			else
			{
				_MainActivity.AppendLogText("Writing characteristic...");
			}
		}

		public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
		{
			base.OnCharacteristicChanged(gatt, characteristic);
			Console.WriteLine("ProvisionBeaconActivity:GattCallback.OnCharacteristicChanged");
		}

		public override void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
		{
			base.OnDescriptorRead(gatt, descriptor, status);
			Console.WriteLine("ProvisionBeaconActivity:GattCallback.OnDescriptorRead");
		}

		public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic,
			GattStatus status)
		{
			base.OnCharacteristicRead(gatt, characteristic, status);
			Console.WriteLine("ProvisionBeaconActivity:GattCallback.OnCharacteristicRead");
		}

		public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
		{
			base.OnDescriptorWrite(gatt, descriptor, status);
			Console.WriteLine("ProvisionBeaconActivity:GattCallback.OnDescriptorWrite");
		}

		public override void OnMtuChanged(BluetoothGatt gatt, int mtu, GattStatus status)
		{
			base.OnMtuChanged(gatt, mtu, status);
			Console.WriteLine("ProvisionBeaconActivity:GattCallback.OnMtuChanged");
		}

		public override void OnPhyRead(BluetoothGatt gatt, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy, GattStatus status)
		{
			base.OnPhyRead(gatt, txPhy, rxPhy, status);
			Console.WriteLine("ProvisionBeaconActivity:GattCallback.OnPhyRead");
		}

		public override void OnPhyUpdate(BluetoothGatt gatt, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy, GattStatus status)
		{
			base.OnPhyUpdate(gatt, txPhy, rxPhy, status);
			Console.WriteLine("ProvisionBeaconActivity:GattCallback.OnPhyUpdate");
		}

		public override void OnReadRemoteRssi(BluetoothGatt gatt, int rssi, GattStatus status)
		{
			base.OnReadRemoteRssi(gatt, rssi, status);
			Console.WriteLine("ProvisionBeaconActivity:GattCallback.OnReadRemoteRssi");
		}

		public override void OnReliableWriteCompleted(BluetoothGatt gatt, GattStatus status)
		{
			base.OnReliableWriteCompleted(gatt, status);
			Console.WriteLine("ProvisionBeaconActivity:GattCallback.OnReliableWriteCompleted");
		}

		private byte[] StringToByteArrayFastest(string hex)
		{
			if (hex.Length % 2 == 1)
				throw new Exception("The binary key cannot have an odd number of digits");

			byte[] arr = new byte[hex.Length >> 1];

			for (int i = 0; i < hex.Length >> 1; ++i)
			{
				arr[i] = (byte) ((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
			}

			return arr;
		}

		private int GetHexVal(char hex)
		{
			int val = (int) hex;
			//For uppercase A-F letters:
			// return val - (val < 58 ? 48 : 55);
			//For lowercase a-f letters:
			return val - (val < 58 ? 48 : 87);
			//Or the two combined, but a bit slower:
			//return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
		}
	}
}