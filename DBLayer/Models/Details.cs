using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DBLayer.Models
{
    public class Details
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("DetailsId")]
        public int Id { get; set; }


        [StringLength(50)]
        public string Description { get; set; }

        [Column(TypeName = "money")]
        public decimal Sum { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
