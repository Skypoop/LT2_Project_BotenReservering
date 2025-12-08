using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.Views;

public partial class ReservationFormView : ContentPage
{
    public ReservationFormView(ReservationFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    private void OnAddClientClicked(object sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: Client client } &&
            this.BindingContext is ReservationFormViewModel vm)
        {
            if (vm.AddClientCommand.CanExecute(client))
            {
                vm.AddClientCommand.Execute(client);
            }
        }
    }
    private void OnShowWarningClicked(object sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: Client client } &&
            this.BindingContext is ReservationFormViewModel vm)
        {
            if (vm.ShowQualificationWarningCommand.CanExecute(client))
            {
                vm.ShowQualificationWarningCommand.Execute(client);
            }
        }
    }
    private void OnRemoveClientClicked(object sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: Client client } &&
            this.BindingContext is ReservationFormViewModel vm)
        {
            if (vm.RemoveClientCommand.CanExecute(client))
            {
                vm.RemoveClientCommand.Execute(client);
            }
        }
    }
}