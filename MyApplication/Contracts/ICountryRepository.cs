using MyApplication.Data;

namespace MyApplication.Contracts;

public interface ICountryRepository : IGenericRepository<Country>
{
    Task<Country> GetDetails(int id);
}