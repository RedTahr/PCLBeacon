
using Xamarin.Forms;

namespace App1Beacon {
	public class App : Application {

		public App() {
			MainPage = new NavigationPage(App.GetMainPage());
		}

		public static Xamarin.Forms.Page GetMainPage() {
			return new Page.Main();
		}

		protected override void OnStart() {
			// Handle when your app starts
		}

		protected override void OnSleep() {
			// Handle when your app sleeps
		}

		protected override void OnResume() {
			// Handle when your app resumes
		}
	}
}
