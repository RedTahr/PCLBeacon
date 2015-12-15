using System;

namespace App1Beacon.Interface {

	public interface IAltBeaconService {
		void InitializeService();
		void StartMonitoring();
		void StartRanging();

		event EventHandler<Model.ListChangedEventArgs> ListChanged;
		event EventHandler DataClearing;
	}
}
