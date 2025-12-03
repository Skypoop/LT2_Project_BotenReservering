using ProjectBotenReservering.Core.Interfaces.Context;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class ClientService(IClientRepository clientRepository, IClientContext clientContext) : IClientService
{
    public Client? GetCurrentClient()
    {
        int clientId = clientContext.GetCurrentClientId();
        return clientRepository.Get(clientId);
    }
}
