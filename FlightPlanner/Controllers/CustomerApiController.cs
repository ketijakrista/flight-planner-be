using FlightPlanner.Models;
using FlightPlanner.Storage;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    [EnableCors]
    public class CustomerApiController : ControllerBase
    {
        [HttpGet]
        [Route("airports")]
        public IActionResult SearchAirports(string search)
        {
            return Ok(FlightStorage.FindAirport(search));
        }

        [HttpPost]
        [Route("flights/search")]
        public IActionResult FindFlights(SearchFlightRequest request)
        {
            if (!FlightStorage.SearchRequestIsValid(request))
                return BadRequest();

            return Ok(FlightStorage.FindFlights(request));
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult FindFlights(int id)
        {
            var flight = FlightStorage.GetFlightById(id);
            if (flight == null)
                return NotFound();

            return Ok(flight);
        }
    }
}
