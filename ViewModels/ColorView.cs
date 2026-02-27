using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class ColorView
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string EnName { get; set; }
    }
}