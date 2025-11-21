using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface IWindConstraintRepository
{
    public WindConstraint Add(WindConstraint item);
    public WindConstraint? Get(int windforce);
    public List<WindConstraint> GetAll();
}

