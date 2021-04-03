using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportGenreDto
    {
        [Required]
        public string Name { get; set; }
    }
}