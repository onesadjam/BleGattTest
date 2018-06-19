using System.Collections.Generic;
using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Java.Util;
using ScanMode = Android.Bluetooth.LE.ScanMode;

namespace BleGattTest
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		private readonly string _MyDeviceAddress = "B0:B0:B0:B0:00:18";
		private BluetoothLeScanner _Scanner;
		private BluetoothManager _Manager;
		private BluetoothAdapter _Adapter;
		private BleScanCallback _ScanCallback;
		private BleGattCallback _GattCallback;
		private BluetoothDevice _Device;
		private TextView _LogTextView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

			// Start scanning for the beacon we want to write a value to
			var scanModeBuilder = new ScanSettings.Builder();
			scanModeBuilder.SetScanMode(ScanMode.LowLatency);

			var deviceAddressFilterBuilder = new ScanFilter.Builder();
			deviceAddressFilterBuilder.SetDeviceAddress(_MyDeviceAddress);

			_Manager = (BluetoothManager)GetSystemService("bluetooth");
			_Adapter = _Manager.Adapter;
			_Scanner = _Adapter.BluetoothLeScanner;
			_ScanCallback = new BleScanCallback(this);
			_GattCallback = new BleGattCallback(this);

			_LogTextView = FindViewById<TextView>(Resource.Id.logTextView);

			_Scanner.StartScan(
				new List<ScanFilter>
				{
					deviceAddressFilterBuilder.Build()
				}, scanModeBuilder.Build(), _ScanCallback);

			_LogTextView.Text = "Started scanning....";
		}

		public void OnBeaconFound()
		{
			AppendLogText("Found BLE device");
			_Device = _Adapter.GetRemoteDevice(_MyDeviceAddress);
			_Device.ConnectGatt(this, false, _GattCallback);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_Scanner.StopScan(_ScanCallback);
		}

		public void AppendLogText(string text)
		{
			RunOnUiThread(() => { _LogTextView.Text = _LogTextView.Text + "\n" + text; });
		}
	}
}

