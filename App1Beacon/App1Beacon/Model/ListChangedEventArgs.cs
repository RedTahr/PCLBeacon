using System;

namespace App1Beacon.Model {

	public class ListChangedEventArgs : EventArgs {
		public System.Collections.Generic.List<Model.Beacon> Data { get; protected set; }
		public ListChangedEventArgs(System.Collections.Generic.List<Model.Beacon> data) {
			Data = data;
		}
	}
}
