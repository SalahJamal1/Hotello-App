using MyApplication.Contracts;
using MyApplication.Data;

namespace MyApplication.Repository;

public class HotelsRepository : GenericRepository<Hotel>, IHotelsRepository
{
    public HotelsRepository(HotelDBContext context) : base(context)
    {
    }
}