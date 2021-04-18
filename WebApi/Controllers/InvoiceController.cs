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
                    EmployeeId = employee.Id,
                    EmployeeName = employee.Name + ' ' + employee.Surname,

                }).GroupJoin(await _context.Details.ToListAsync(), entry => entry.Id, d => d.InvoiceId,
                (entry, details) => new
                {
                    entry.Id,
                    entry.InvoiceNumber,
                    entry.ExecutionDate,
                    entry.Sum,
                    entry.EmployeeId,
                    entry.EmployeeName,
                    Details = details.Select(d => new {d.Id, d.Description, d.Sum})

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

        //[Route("/Details")]
        [HttpGet("/api/[controller]/Details/{id}")]
        public async Task<ActionResult<Details>> GetTaskDetails(int id)
        {
            var details = await _context.Details.Where(d => d.InvoiceId == id).ToListAsync();
            if (details == null)
                return NotFound();
            return new ObjectResult(details);
        }

        // POST api/Invoices
        [HttpPost]
        public async Task<ActionResult<Invoice>> Post(Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest();
            }

            invoice.EmployeeId = invoice.Employee.Id;
            invoice.Employee = null;
            foreach (var invoiceDetail in invoice.Details)
            {
                invoiceDetail.Invoice = invoice;
            }
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // PUT api/Invoices/
        [HttpPut("{id}")]
        public async Task<ActionResult<Invoice>> Put(int id, [FromBody]Invoice invoice)
        {
            if (invoice == null)
            {
                return BadRequest();
            }
            if (!_context.Invoices.Any(x => x.Id == invoice.Id))
            {
                return NotFound();
            }

            invoice.EmployeeId = invoice.Employee.Id;
            invoice.Employee = null;

            var dbInvoice = await _context.Invoices.Where(i => i.Id == invoice.Id).Include(i => i.Details).SingleOrDefaultAsync();
            if (dbInvoice == null)
                return NotFound();


            
            _context.Entry(dbInvoice).CurrentValues.SetValues(invoice);

            foreach (var dbInvoiceDetail in dbInvoice.Details.ToList())
            {
                if (invoice.Details.All(d => d.Id != dbInvoiceDetail.Id))
                {
                    _context.Details.Remove(dbInvoiceDetail);
                }
            }

            foreach (var invoiceDetail in invoice.Details)
            {
                invoiceDetail.InvoiceId = invoice.Id;
                var dbDetails = dbInvoice.Details.SingleOrDefault(d => d.Id == invoiceDetail.Id);
                if (dbDetails == null)
                {
                    dbInvoice.Details.Add(invoiceDetail);
                }
                else
                {
                    _context.Entry(dbDetails).CurrentValues.SetValues(invoiceDetail);
                }
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();


            return Ok();
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

