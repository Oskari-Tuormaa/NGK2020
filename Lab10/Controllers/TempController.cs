using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using aspnet.Models;

namespace aspnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TempController : ControllerBase
    {
      static public List<TempData> LastData = new List<TempData>();

      [HttpGet]
      public IEnumerable<TempData> Read()
      {
        return LastData;
      }

      [HttpGet("{date}")]
      public TempData ReadDate(DateTime date)
      {
        return LastData.Find(item => item.Date.Date.Equals(date.Date));
      }

      [HttpGet("clear")]
      public IActionResult Clear()
      {
        LastData.Clear();
        return Ok();
      }

      [HttpPost]
      public IActionResult Write([FromBody] TempData data)
      {
        LastData.Add(data.Clone());
        return Ok(data);
      }
    }
}
