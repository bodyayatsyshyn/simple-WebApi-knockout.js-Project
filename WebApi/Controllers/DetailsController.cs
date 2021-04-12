using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBLayer;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        private readonly WebAppDbContext _context;
        public DetailsController(WebAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> Get()
        {
            var detailsList = await _context.Details.ToListAsync();
            var data = detailsList.Join(await _context.Invoices.ToListAsync(), d => d.InvoiceId, i => i.Id,
                (details, invoice) => new
                {
                    details.Id,
                    details.Description,
                    details.Sum,
                    InvoiceNumber = invoice.InvoiceNumber
                }).ToList();
            data.Sort((first, second) => first.Id.CompareTo(second.Id));
            return new ActionResult<IEnumerable<dynamic>>(data);
        }

        // GET api/Details/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Details>> Get(int id)
        {
            Details details = await _context.Details.FirstOrDefaultAsync(x => x.Id == id);
            if (details == null)
                return NotFound();
            return new ObjectResult(details);
        }

        // POST api/Details
        [HttpPost]
        public async Task<ActionResult<Details>> Post(Details details)
        {
            if (details == null)
            {
                return BadRequest();
            }

            details.Id = 0;
            await _context.Details.AddAsync(details);
            await _context.SaveChangesAsync();
            return Ok(details);
        }

        // PUT api/Details/
        [HttpPut("{id}")]
        public async Task<ActionResult<Details>> Put(int id, [FromBody]Details details)
        {
            if (details == null)
            {
                return BadRequest();
            }
            if (!_context.Details.Any(x => x.Id == details.Id))
            {
                return NotFound();
            }

            _context.Update(details);
            await _context.SaveChangesAsync();
            return Ok(details);
        }

        // DELETE api/Details/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Details>> Delete(int id)
        {
            Details details = _context.Details.FirstOrDefault(x => x.Id == id);
            if (details == null)
            {
                return NotFound();
            }
            _context.Details.Remove(details);
            await _context.SaveChangesAsync();
            var list = await _context.Details.ToListAsync();
            return Ok(details);
        }
    }
}
