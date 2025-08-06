using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Contracts;
using MyApplication.Data;
using MyApplication.Exceptions;
using MyApplication.Models.Hotles;

namespace MyApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IHotelsRepository _repository;

    public HotelsController(IHotelsRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // GET: api/Hotels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
    {
        var hotels = await _repository.GetAllAsync();
        return Ok(_mapper.Map<List<HotelDto>>(hotels));
    }

    // GET: api/Hotels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetHotel(int id)
    {
        var hotel = await _repository.GetAsync(id);

        if (hotel == null) throw new AppErrorException($"Wrong Hotel id {id}");

        return Ok(_mapper.Map<HotelDto>(hotel));
    }

    // PUT: api/Hotels/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutHotel(int id, UpdateHotel newHotel)
    {
        var hotel = await _repository.GetAsync(id);
        if (hotel == null) throw new AppErrorException($"Wrong Hotel id {id}");

        _mapper.Map(newHotel, hotel);

        await _repository.UpdateAsync(hotel);

        return NoContent();
    }

    // POST: api/Hotels
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<HotelDto>> PostHotel(CreateHotel newHotel)
    {
        var hotel = _mapper.Map<Hotel>(newHotel);
        await _repository.AddAsync(hotel);
        return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, _mapper.Map<HotelDto>(hotel));
    }

    // DELETE: api/Hotels/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var hotel = await _repository.GetAsync(id);
        if (hotel == null) throw new AppErrorException($"Wrong Hotel id {id}");

        await _repository.DeleteAsync(id);

        return NoContent();
    }
}