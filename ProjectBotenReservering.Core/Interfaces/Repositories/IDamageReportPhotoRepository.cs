using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IDamageReportPhotoRepository
{
    public Task<DamageReportPhoto> Add(DamageReportPhoto item);
    public Task<DamageReportPhoto>? Get(int id);
    public Task<List<DamageReportPhoto>> GetByDamageReportId(int damageReportId);
    public Task Delete(int id);
}

