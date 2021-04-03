using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportDeveloperDto
    {
        [Required]
        public string Name { get; set; }
    }
}