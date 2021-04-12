using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DBLayer.Models
{
    [Table("Invoice")]
    public class Invoice
    {

        public Invoice()
        {
            Details = new HashSet<Details>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("InvoiceId")]
        public int Id { get; set; }


        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        public DateTime ExecutionDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? Sum { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual ICollection<Details> Details { get; set; }

    }
}