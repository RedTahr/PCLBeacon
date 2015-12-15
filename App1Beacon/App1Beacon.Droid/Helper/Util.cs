
using AltBeaconOrg.BoundBeacon;
using Android.App;
using Android.Content;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace App1Beacon.Droid.Helper {
	internal static class Util {
		internal static void Notify(Region region = null, [CallerMemberName] string title = "Unknown") {
			Notification.Builder builder = new Notification.Builder(Forms.Context)
							.SetContentTitle(title)
							.SetContentText(region != null ? $"{region.Id1}" : "Region unknown")
							.SetSmallIcon(Resource.Drawable.icon);
			Notification notification = builder.Build();
			NotificationManager notificationManager = Forms.Context.GetSystemService(Context.NotificationService) as NotificationManager;
			const int notificationId = 11;
			notificationManager.Notify(notificationId, notification);
		}
	}
}