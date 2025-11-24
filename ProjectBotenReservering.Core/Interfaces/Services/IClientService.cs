namespace ProjectBotenReservering.Core.Interfaces.Services;

using Models;

public interface IClientService
{
    Task<Client?> GetCurrentClient();
}