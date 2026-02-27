using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class RegionView
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string EnName { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public RegionType Type { get; set; }
        [DisplayName("Type")]
        public string TypeString { get; set; }
    }
}