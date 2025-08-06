using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Contracts;
using MyApplication.Data;
using MyApplication.Exceptions;
using MyApplication.Models.Country;

namespace MyApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICountryRepository _repository;

    public CountriesController(ICountryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CountriesDto>>> GetCountries()
    {
        var countries = await _repository.GetAllAsync();
        var result = _mapper.Map<List<CountriesDto>>(countries);

        return Ok(result);
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountry(int id)
    {
        var country = await _repository.GetDetails(id);

        if (country == null) throw new AppErrorException($"Wrong country id {id}");

        return Ok(_mapper.Map<CountryDto>(country));
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutCountry(int id, CountryBase newCountry)
    {
        var country = await _repository.GetAsync(id);
        if (country == null) throw new AppErrorException($"Wrong country id {id}");

        _mapper.Map(newCountry, country);

        await _repository.UpdateAsync(country);

        return NoContent();
    }

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CountriesDto>> PostCountry(CountryBase newCountry)
    {
        var country = _mapper.Map<Country>(newCountry);
        await _repository.AddAsync(country);
        return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, _mapper.Map<CountriesDto>(country));
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await _repository.GetAsync(id);
        if (country == null) throw new AppErrorException($"Wrong country id {id}");

        await _repository.DeleteAsync(id);

        return NoContent();
    }
}