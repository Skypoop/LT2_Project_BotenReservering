using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IClientRepository
{
    public Client Add(Client item);
    public Client? Get(string email);
    public Client? Get(int id);
    public List<Client> GetAll();
}