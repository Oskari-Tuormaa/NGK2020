using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ObservationsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/Observation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Observation>>> Get()
        {
            return await _context.Observations.ToListAsync();
        }

        // GET: api/Observation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Observation>> GetObservation(int id)
        {
            var item = await _context.Observations.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }
        [HttpGet("latest")]
        public async Task<ActionResult<Observation>> GetLatest()
        {
            var items = await _context.Observations.ToListAsync();

            
            return items.OrderByDescending(g => g.Date).First();
        }
        [HttpGet("fromdate/{date:datetime}")]
        public async Task<ActionResult<IEnumerable<Observation>>> GetFromDate(DateTime date)
        {
            var items = await _context.Observations.ToListAsync();

            return items.FindAll(x => x.Date.Date.Equals(date.Date));
        }
        [HttpGet("fromperiod/{date1:datetime}/{date2:datetime}")]
        public async Task<ActionResult<IEnumerable<Observation>>> GetPeriod(DateTime date1, DateTime date2)
        {
            var items = await _context.Observations.ToListAsync();

            return items.FindAll(x => x.Date >= date1 && x.Date <= date2);
        }

        // POST: api/Observation
        [HttpPost, Authorize]
      
        public async Task<ActionResult<Observation>> PostObservation(Observation obs)
        {

            _context.Observations.Add(obs);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = obs.ObservationId }, obs);
        }

        // PUT: api/Observation/5
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutObservation(int id, Observation obs)
        {
            if (obs == null || obs.ObservationId != id)
            {
                return BadRequest();
            }
            _context.Entry(obs).State = EntityState.Modified;
            
            try
            { await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ObservationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE: api/Observation/5
        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult<Observation>> DeleteObservation(int id)
        {
            var item = await _context.Observations.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            _context.Observations.Remove(item);
            await _context.SaveChangesAsync();

            return item;
        }
        private bool ObservationExists(long id)
        {
            return _context.Observations.Any(e => e.ObservationId == id);
        }
    }
}
