using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IClientRepository
{
    public Task<Client> Add(Client item);
    public Task<Client?> Get(string email);
    public Task<Client?> Get(int id);
    public Task<List<Client>> GetAll();
}