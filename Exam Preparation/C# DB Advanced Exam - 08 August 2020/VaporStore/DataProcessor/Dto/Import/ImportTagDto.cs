using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportTagDto
    {
        [Required]
        public string Name { get; set; }
    }
}