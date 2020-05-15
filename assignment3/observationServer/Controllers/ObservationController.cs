using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using observationServer.Models;
using observationServer.Helpers;

namespace observationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObservationController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        public ObservationController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Observation observation)
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            // Validate passed data
            if (observation.Date == new DateTime())
                return BadRequest("Observation date is required");
            if (observation.Name == "" || observation.Name == null)
                return BadRequest("Observation name is required");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            // Assign lowest available id to observation
            int id = 1;
            while (observations.Find(x => x.id == id) != null)
                id++;
            observation.id = id;

            observations.Add(observation);

            System.IO.File.WriteAllText(_appSettings.ObservationDir,
                    JsonConvert.SerializeObject(observations));
            return Ok(observation);
        }

        [HttpDelete, Authorize]
        public IActionResult Remove([FromBody] Observation toDelete)
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            Observation toRemove = observations.Find(x => x.id == toDelete.id);

            if (toRemove == null)
                return BadRequest($"No observation with ID {toDelete.id} exists");

            observations.Remove(toRemove);

            System.IO.File.WriteAllText(_appSettings.ObservationDir,
                    JsonConvert.SerializeObject(observations));
            return Ok(toRemove);
        }

        [HttpGet]
        public IActionResult Get()
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            return Ok(observations);
        }

        [HttpGet("latest")]
        public IActionResult GetLatest()
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            observations.Sort((x, y) => DateTime.Compare(y.Date, x.Date));

            return Ok(observations.Take(3));
        }

        [HttpGet("{date:datetime}")]
        public IActionResult GetDate(DateTime date)
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            return Ok(observations.FindAll(x => x.Date.Date.Equals(date.Date)));
        }

        [HttpGet("{date1:datetime}/{date2:datetime}")]
        public IActionResult GetPeriod(DateTime date1, DateTime date2)
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            return Ok(observations.FindAll(x => x.Date >= date1 && x.Date <= date2));
        }
    }
}
