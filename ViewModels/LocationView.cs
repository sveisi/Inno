using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class LocationView
    {
        [Required]
        [DisplayName("Code")]
        public string Id { get; set; }
        [Required]
        [DisplayName("Storage")]
        public string StorageId { get; set; }
        [Required]
        public string Name { get; set; }
        public string EnName { get; set; }
    }
}