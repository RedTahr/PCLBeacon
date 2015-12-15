using AltBeaconOrg.BoundBeacon;
using Android.App;
using App1Beacon.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(App1Beacon.Droid.Service.AltBeacon))]

namespace App1Beacon.Droid.Service {
	public class AltBeacon : Java.Lang.Object, IAltBeaconService {
		private readonly MonitorNotifier _monitorNotifier;
		private readonly RangeNotifier _rangeNotifier;
		private BeaconManager _beaconManager;

		Region _tagRegion;

		Region _emptyRegion;
		private Xamarin.Forms.ListView _list;
		private readonly List<Beacon> _data;

		public AltBeacon() {
			_monitorNotifier = new MonitorNotifier();
			_rangeNotifier = new RangeNotifier();
			_data = new List<Beacon>();
		}

		public event EventHandler<Model.ListChangedEventArgs> ListChanged;
		public event EventHandler DataClearing;

		public BeaconManager BeaconManagerImpl {
			get {
				if(_beaconManager == null) {
					_beaconManager = InitializeBeaconManager();
				}
				return _beaconManager;
			}
		}

		public void InitializeService() {
			_beaconManager = InitializeBeaconManager();
		}

		private BeaconManager InitializeBeaconManager() {
			// Enable the BeaconManager 
			BeaconManager bm = BeaconManager.GetInstanceForApplication(Xamarin.Forms.Forms.Context);

			#region Set up Beacon Simulator if testing without a BLE device
			//			var beaconSimulator = new BeaconSimulator();
			//			beaconSimulator.CreateBasicSimulatedBeacons();
			//
			//			BeaconManager.BeaconSimulator = beaconSimulator;
			#endregion

			var iBeaconParser = new BeaconParser();
			//	Estimote > 2013
			iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24");
			bm.BeaconParsers.Add(iBeaconParser);

			_monitorNotifier.EnterRegionComplete += EnteredRegion;
			_monitorNotifier.ExitRegionComplete += ExitedRegion;
			_monitorNotifier.DetermineStateForRegionComplete += DeterminedStateForRegionComplete;
			_rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;

			_tagRegion = new AltBeaconOrg.BoundBeacon.Region("myUniqueBeaconId", Identifier.Parse("E4C8A4FC-F68B-470D-959F-29382AF72CE7"), null, null);
			_tagRegion = new AltBeaconOrg.BoundBeacon.Region("myUniqueBeaconId", Identifier.Parse("B9407F30-F5F8-466E-AFF9-25556B57FE6D"), null, null);
			_emptyRegion = new AltBeaconOrg.BoundBeacon.Region("myEmptyBeaconId", null, null, null);

			bm.SetBackgroundMode(false);
			bm.Bind((IBeaconConsumer)Xamarin.Forms.Forms.Context);

			return bm;
		}

		public void StartMonitoring() {
			BeaconManagerImpl.SetForegroundBetweenScanPeriod(5000); // 5000 milliseconds

			BeaconManagerImpl.SetMonitorNotifier(_monitorNotifier);
			_beaconManager.StartMonitoringBeaconsInRegion(_tagRegion);
			_beaconManager.StartMonitoringBeaconsInRegion(_emptyRegion);
		}

		public void StartRanging() {
			BeaconManagerImpl.SetForegroundBetweenScanPeriod(5000); // 5000 milliseconds

			BeaconManagerImpl.SetRangeNotifier(_rangeNotifier);
			_beaconManager.StartRangingBeaconsInRegion(_tagRegion);
			_beaconManager.StartRangingBeaconsInRegion(_emptyRegion);
		}

		private void DeterminedStateForRegionComplete(object sender, MonitorEventArgs e) {
			Console.WriteLine("DeterminedStateForRegionComplete");
			Helper.Util.Notify(e.Region);
		}

		private void ExitedRegion(object sender, MonitorEventArgs e) {
			Console.WriteLine("ExitedRegion");
			Helper.Util.Notify(e.Region);
		}

		private void EnteredRegion(object sender, MonitorEventArgs e) {
			Console.WriteLine("EnteredRegion");
			Helper.Util.Notify(e.Region);
		}

		async void RangingBeaconsInRegion(object sender, RangeEventArgs e) {
			await ClearData();

			var allBeacons = new List<Beacon>();
			if(e.Beacons.Count > 0) {
				foreach(var b in e.Beacons) {
					allBeacons.Add(b);
				}

				var orderedBeacons = allBeacons.OrderBy(b => b.Distance).ToList();
				await UpdateData(orderedBeacons);
			}
			else {
				// unknown
				await ClearData();
			}
		}

		private async Task UpdateData(List<Beacon> beacons) {
			await Task.Run(() => {
				var newBeacons = new List<Beacon>();
				foreach(var beacon in beacons) {
					if(_data.All(b => b.Id1.ToString() == beacon.Id1.ToString())) {
						newBeacons.Add(beacon);
					}
				}

				((Activity)Xamarin.Forms.Forms.Context).RunOnUiThread(() => {
					foreach(var beacon in newBeacons) {
						_data.Add(beacon);
					}

					if(newBeacons.Count > 0) {
						_data.Sort((x, y) => x.Distance.CompareTo(y.Distance));
						UpdateList();
					}
				});
			});
		}

		private async Task ClearData() {
			((Activity)Xamarin.Forms.Forms.Context).RunOnUiThread(() => {
				_data.Clear();
				OnDataClearing();
			});
		}

		private void OnDataClearing() {
			var handler = DataClearing;
			if(handler != null) {
				handler(this, EventArgs.Empty);
			}
		}

		private void UpdateList() {
			((Activity)Xamarin.Forms.Forms.Context).RunOnUiThread(() => {
				OnListChanged();
			});
		}

		private void OnListChanged() {
			var handler = ListChanged;
			if(handler != null) {
				var data = new List<Model.Beacon>();
				_data.ForEach(b => {
					data.Add(new Model.Beacon { Id = b.Id1.ToString(), Distance = string.Format("{0:N2}m", b.Distance) });
				});
				handler(this, new Model.ListChangedEventArgs(data));
			}
		}
	}
}

