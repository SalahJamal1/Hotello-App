using AutoMapper;
using MyApplication.Data;
using MyApplication.Models.Country;
using MyApplication.Models.Hotles;
using MyApplication.Models.Users;

namespace MyApplication.Configuration;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<ApiUser, ApiUserDto>().ReverseMap();
        CreateMap<ApiUser, UserDto>().ReverseMap();
        CreateMap<Hotel, UpdateHotel>().ReverseMap();
        CreateMap<Hotel, HotelDto>().ReverseMap();
        CreateMap<Hotel, CreateHotel>().ReverseMap();
        CreateMap<Country, CountryBase>().ReverseMap();
        CreateMap<Country, CountryDto>().ReverseMap();
        CreateMap<Country, CountriesDto>().ReverseMap();
    }
}