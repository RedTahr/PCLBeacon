using App1Beacon.Interface;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace App1Beacon.ViewModel {

	public class Beacon {
		public Beacon() {
			Data = new List<Model.Beacon>();
		}

		public event EventHandler ListChanged;

		public List<Model.Beacon> Data { get; set; }

		public void Init() {
			var beaconService = DependencyService.Get<IAltBeaconService>();
			beaconService.ListChanged += (sender, e) => {
				Data = e.Data;
				OnListChanged();
			};
			beaconService.DataClearing += (sender, e) => {
				Data.Clear();
				OnListChanged();
			};

			beaconService.InitializeService();
		}

		private void OnListChanged() {
			var handler = ListChanged;
			if(handler != null) {
				handler(this, EventArgs.Empty);
			}
		}
	}
}
