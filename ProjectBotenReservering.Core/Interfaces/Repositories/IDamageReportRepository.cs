using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IDamageReportRepository
{
    public DamageReport Add(DamageReport item);
    public DamageReport? Get(int id);
    public List<DamageReport> GetAll();
    public List<DamageReport> GetByClientId(int clientId);
    public List<DamageReport> GetByBoatId(int boatId);
}

