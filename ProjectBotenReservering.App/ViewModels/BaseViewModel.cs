namespace ProjectBotenReservering.App.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = "";
    public virtual void Load() { }
    public virtual void OnAppearing() { }
    public virtual void OnDisappearing() { }
}
