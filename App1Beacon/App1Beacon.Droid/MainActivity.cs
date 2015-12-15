using AltBeaconOrg.BoundBeacon;
using AltBeaconOrg.BoundBeacon.Powersave;
using AltBeaconOrg.BoundBeacon.Startup;
using Android.App;
using Android.Content.PM;
using Android.OS;
using App1Beacon.Interface;

namespace App1Beacon.Droid {
	[Activity(Label = "App1Beacon", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity, IBootstrapNotifier, IBeaconConsumer {

		BackgroundPowerSaver backgroundPowerSaver;

		protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			backgroundPowerSaver = new BackgroundPowerSaver(this);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(new App());
		}
		public void DidDetermineStateForRegion(int state, Region region) {
			System.Diagnostics.Debug.WriteLine($"DidDetermineStateForRegion {state}, {region}");
			Helper.Util.Notify(region);
		}

		public void DidEnterRegion(Region region) {
			System.Diagnostics.Debug.WriteLine($"DidEnterRegion {region}");
			Helper.Util.Notify(region);
		}

		public void DidExitRegion(Region region) {
			System.Diagnostics.Debug.WriteLine($"DidExitRegion {region}");
			Helper.Util.Notify(region);
		}

		public void OnBeaconServiceConnect() {
			var beaconService = Xamarin.Forms.DependencyService.Get<IAltBeaconService>();

			beaconService.StartMonitoring();
			beaconService.StartRanging();
		}
	}
}

