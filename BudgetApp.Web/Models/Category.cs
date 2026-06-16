using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Web.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa kategorii jest wymagana")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}