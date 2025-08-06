using MyApplication.Models.Hotles;

namespace MyApplication.Models.Country;

public class CountryDto : CountriesDto
{
    public IList<HotelDto> Hotels { get; set; }
}