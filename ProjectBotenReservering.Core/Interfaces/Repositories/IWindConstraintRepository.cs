using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IWindConstraintRepository
{
    public Task<WindConstraint> Add(WindConstraint item);
    public Task<WindConstraint?> Get(int windforce);
    public Task<List<WindConstraint>> GetAll();
}

