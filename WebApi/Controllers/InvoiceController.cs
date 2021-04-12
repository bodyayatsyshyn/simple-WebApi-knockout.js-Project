using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class InvoiceController : ControllerBase
    {
        private readonly WebAppDbContext _context;
        public InvoiceController(WebAppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> Get()
        {
            var invoices = await _context.Invoices.ToListAsync();
            var data = invoices.Join(await _context.Employees.ToListAsync(), i => i.EmployeeId, e => e.Id,
                (invoice, employee) => new
                {
                    invoice.Id,
                    invoice.InvoiceNumber,
                    invoice.ExecutionDate,
                    Sum = invoice.Sum ??= 0,
                    EmployeeName = employee.Name + ' ' + employee.Surname
                }).ToList();
            data.Sort((first, second) => first.Id.CompareTo(second.Id));
            return new ActionResult<IEnumerable<dynamic>>(data);
        }

        // GET api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> Get(int id)
        {
            Invoice invoice = await _context.Invoices.FirstOrDefaultAsync(x => x.Id == id);
            if (invoice == null)
                return NotFound();
            return new ObjectResult(invoice);
        }

        // POST api/Invoices
        [HttpPost]
        public async Task<ActionResult<Invoice>> Post(Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest();
            }

            invoice.Id = 0;
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            return Ok(invoice);
        }

        // PUT api/Invoices/
        [HttpPut("{id}")]
        public async Task<ActionResult<Invoice>> Put(int id, [FromBody]Invoice invoice)
        {
            Debug.Write("http put here");
            if (invoice == null)
            {
                return BadRequest();
            }
            if (!_context.Invoices.Any(x => x.Id == invoice.Id))
            {
                return NotFound();
            }

            _context.Update(invoice);
            await _context.SaveChangesAsync();
            return Ok(invoice);
        }

        // DELETE api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Invoice>> Delete(int id)
        {
            Invoice invoice = _context.Invoices.FirstOrDefault(x => x.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return Ok(invoice);
        }
    }
}

