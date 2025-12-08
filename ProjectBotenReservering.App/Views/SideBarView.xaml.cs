using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.Core.Models; 

namespace ProjectBotenReservering.App.Views;

public partial class SideBarView : ContentPage
{
    public SideBarView(SideBarViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    private void OnTabButtonClicked(object sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: TabItem tabItem } &&
            this.BindingContext is SideBarViewModel vm)
        {
            if (vm.SelectTabCommand.CanExecute(tabItem))
            {
                vm.SelectTabCommand.Execute(tabItem);
            }
        }
    }
}