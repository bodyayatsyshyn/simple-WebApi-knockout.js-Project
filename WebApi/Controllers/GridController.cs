using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBLayer;
using DBLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GridController : ControllerBase
    {
        private readonly WebAppDbContext _context;
        public GridController(WebAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> Get()
        {
            var employees = await _context.Employees.ToListAsync();
            var invoices = await _context.Invoices.ToListAsync();
            var data = employees.Join(invoices, e => e.Id, i => i.EmployeeId,
                (employee, invoice) => new
                {
                    invoice.Id,
                    EmployeeName = employee.Name + ' ' + employee.Surname,
                    invoice.InvoiceNumber,
                    Date = invoice.ExecutionDate,
                    Sum = invoice.Sum ??= 0,
                }).ToList();
            data.Sort((first, second) => first.Id.CompareTo(second.Id));

            return new ActionResult<IEnumerable<dynamic>>(data);
        }

        // GET api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<dynamic>> Get(int id)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == invoice.EmployeeId);
            if (employee == null || invoice == null)
                return NotFound();
            var data = new
            {
                invoice.Id,
                EmployeeName = employee.Name + ' ' + employee.Surname,
                invoice.InvoiceNumber,
                Date = invoice.ExecutionDate,
                invoice.Sum
            };

            return new ObjectResult(data);
        }

        // DELETE api/Employees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<dynamic>> Delete(int id) //id -> invoice id
        {
            Invoice invoice = _context.Invoices.FirstOrDefault(x => x.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            var details = await _context.Details.Where(d => d.InvoiceId == invoice.Id).ToListAsync();
            foreach (var d in details)
            {
                _context.Details.Remove(d);
            }
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return Ok(invoice);
        }
    }
}

