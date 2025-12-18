namespace ProjectBotenReservering.Core.Interfaces.Helpers;

public interface IPromptHelper
{
    Task<string> LoadPromptAsync(string fileName);
}