using Android.Bluetooth.LE;

namespace BleGattTest
{
	public class BleScanCallback : ScanCallback
	{
		private readonly MainActivity _Activity;
		public BleScanCallback(MainActivity activity)
		{
			_Activity = activity;
		}

		public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
		{
			base.OnScanResult(callbackType, result);

			_Activity.OnBeaconFound();
		}
	}
}