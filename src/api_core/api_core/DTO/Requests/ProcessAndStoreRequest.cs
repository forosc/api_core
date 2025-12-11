using System.ComponentModel.DataAnnotations;

namespace api_core.DTO.Requests
{
    public record ProcessAndStoreRequest(
        [Required(ErrorMessage = "La ruta de entrada es obligatoria")]
        [MaxLength(500)]
        string InputPath,

        bool SaveToDatabase = true,
        bool GenerateExcel = true,

        [MaxLength(500)]
        string OutputPath = "");
}
