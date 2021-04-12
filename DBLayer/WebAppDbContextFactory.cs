using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DBLayer
{
    public class WebAppDbContextFactory : IDesignTimeDbContextFactory<WebAppDbContext>
    {
        public WebAppDbContext CreateDbContext(string[] args = null)
        {
            var options = new DbContextOptionsBuilder<WebAppDbContext>();
            options.UseSqlServer(@"Server=PC;Database=WebAppDatabase;Trusted_Connection=True;");
            return new WebAppDbContext(options.Options);
        }
    }
}
