using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IDamageReportRepository
{
    public Task<DamageReport> Add(DamageReport item);
    public Task<DamageReport?> Get(int id);
    public Task<List<DamageReport>> GetAll();
    public Task<List<DamageReport>> GetByClientId(int clientId);
    public Task<List<DamageReport>> GetByBoatId(int boatId);
}

