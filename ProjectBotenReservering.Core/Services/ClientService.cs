using ProjectBotenReservering.Core.Interfaces.Context;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IClientContext _clientContext;

    public ClientService(IClientRepository clientRepository, IClientContext clientContext)
    {
        this._clientRepository = clientRepository;
        this._clientContext = clientContext;
    }

    public Client? GetCurrentClient()
    {
        int clientId = _clientContext.GetCurrentClientId();
        return _clientRepository.Get(clientId);
    }
}
