using ProjectBotenReservering.App.ViewModels;

namespace ProjectBotenReservering.App.Views;

public partial class SideBarView : ContentPage
{
	public SideBarView(SideBarViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}