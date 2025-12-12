using System.Data;

namespace ProjectBotenReservering.Core.Interfaces.Mappers
{
    public interface IMapper<T>
    {
        T Map(IDataReader reader);
    }
}