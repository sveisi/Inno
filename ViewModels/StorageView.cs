using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class StorageView
    {
        [Required]
        [DisplayName("Code")]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string EnName { get; set; }
    }
}