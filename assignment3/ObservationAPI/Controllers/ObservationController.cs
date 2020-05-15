using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ObservationAPI.Models;
using ObservationAPI.Services;

namespace ObservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObservationController : ControllerBase
    {
        private IObservationService _observationService;

        public ObservationController(IObservationService observationService)
        {
            _observationService = observationService;
        }

        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Observation newObservation)
        {
            // Validate passed data
            if (newObservation.Date == new DateTime())
                return BadRequest("Observation date is required");
            if (newObservation.Location.Name == "" || newObservation.Location.Name == null)
                return BadRequest("Observation name is required");

            Observation res = _observationService.Upload(newObservation);

            return Ok(res);
        }

        [HttpDelete, Authorize]
        public IActionResult Remove([FromBody] Observation toDelete)
        {
            // Validate data
            if (toDelete.Id == 0)
                return BadRequest("Observation Id cannot be 0");

            Observation deleted = _observationService.Delete(toDelete.Id);

            if (deleted == null)
                return BadRequest($"No observation with ID {toDelete.Id} exists");

            return Ok(deleted);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_observationService.GetAll());
        }

        [HttpGet("latest")]
        public IActionResult GetLatest()
        {
            return Ok(_observationService.GetLatest());
        }

        [HttpGet("{date:datetime}")]
        public IActionResult GetDate(DateTime date)
        {
            return Ok(_observationService.GetDate(date));
        }

        [HttpGet("{date1:datetime}/{date2:datetime}")]
        public IActionResult GetPeriod(DateTime date1, DateTime date2)
        {
            return Ok(_observationService.GetPeriod(date1, date2));
        }
    }
}
