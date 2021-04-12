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
    [ApiController]
    [Route("api/[controller]")]

    public class EmployeeController : ControllerBase
    {
        private readonly WebAppDbContext _context;
        public EmployeeController( WebAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> Get()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> Get(int id)
        {
            Employee employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee == null)
                return NotFound();
            return new ObjectResult(employee);
        }

        // POST api/Employees
        [HttpPost]
        public async Task<ActionResult<Employee>> Post(Employee employee)
        {
            if (employee == null)
            {
                return BadRequest();
            }

            employee.Id = 0;
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return Ok(employee);
        }

        // PUT api/Employees/
        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> Put(int id, [FromBody]Employee employee)
        {
            if (employee == null)
            {
                return BadRequest();
            }
            if (!_context.Employees.Any(x => x.Id == employee.Id))
            {
                return NotFound();
            }

            _context.Update(employee);
            await _context.SaveChangesAsync();
            return Ok(employee);
        }

        // DELETE api/Employees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Employee>> Delete(int id)
        {
            Employee employee = _context.Employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return Ok(employee);
        }
    }
}
