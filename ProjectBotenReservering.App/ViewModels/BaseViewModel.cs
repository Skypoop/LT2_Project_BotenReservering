namespace ProjectBotenReservering.App.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    string title = "";

    public virtual void Load() { }
    public virtual void OnAppearing() { }
    public virtual void OnDisappearing() { }
}
