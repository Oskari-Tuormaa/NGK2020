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

        List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
            System.IO.File.ReadAllText(_appSettings.ObservationDir));

        if (observations.Count() > 0)
          observation.id = observations.AsEnumerable().OrderByDescending(x => x.id).First().id + 1;
        else
          observation.id = 1;

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

      [HttpGet("{dateStr}")]
      public IActionResult GetDate(string dateStr)
      {
        if (!System.IO.File.Exists(_appSettings.ObservationDir))
          System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

        DateTime date;
        if (!DateTime.TryParse(dateStr, out date))
          return BadRequest("Date in wrong format");

        List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
            System.IO.File.ReadAllText(_appSettings.ObservationDir));

        return Ok(observations.FindAll(x => x.Date.Date.Equals(date.Date)));
      }

      [HttpGet("{dateStr1}/{dateStr2}")]
      public IActionResult GetPeriod(string dateStr1, string dateStr2)
      {
        if (!System.IO.File.Exists(_appSettings.ObservationDir))
          System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

        DateTime date1, date2;

        if (!DateTime.TryParse(dateStr1, out date1))
          return BadRequest("Date1 in wrong format");

        if (!DateTime.TryParse(dateStr2, out date2))
          return BadRequest("Date2 in wrong format");

        List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
            System.IO.File.ReadAllText(_appSettings.ObservationDir));

        return Ok(observations.FindAll(x => x.Date >= date1 && x.Date <= date2));
      }
    }
}
