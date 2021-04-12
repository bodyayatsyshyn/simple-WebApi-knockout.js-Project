using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DBLayer.Models
{
    [Table("Employee")]
    public class Employee
    {
        public Employee()
        {
            Invoices = new HashSet<Invoice>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("EmployeeId")]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }

    }
}