
using Xamarin.Forms;

namespace App1Beacon.Page {

	public class Main : ContentPage {
			ListView _list;
			ViewModel.Beacon _viewModel;

			public Main() {
				BackgroundColor = Color.White;
				Title = "AltBeacon Forms Sample";

				_viewModel = new ViewModel.Beacon();
				_viewModel.ListChanged += (sender, e) =>
				{
					_list.ItemsSource = _viewModel.Data;
				};

				BindingContext = _viewModel;
				Content = BuildContent();
			}

			private Xamarin.Forms.View BuildContent() {
				_list = new ListView {
					VerticalOptions = LayoutOptions.FillAndExpand,
					ItemTemplate = new DataTemplate(typeof(View.ListItem)),
					RowHeight = 90,
				};

				_list.SetBinding(ListView.ItemsSourceProperty, "Data");

				return _list;
			}

			protected override void OnAppearing() {
				base.OnAppearing();
				_viewModel.Init();
			}
		}
	}