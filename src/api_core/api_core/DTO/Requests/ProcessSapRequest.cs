using System.ComponentModel.DataAnnotations;

namespace api_core.DTO.Requests
{
    public record ProcessSapRequest(
        [Required(ErrorMessage = "La ruta de entrada es obligatoria")]
        [MaxLength(500, ErrorMessage = "La ruta no puede exceder 500 caracteres")]
        string InputPath,

        [Required(ErrorMessage = "La ruta de salida es obligatoria")]
        [MaxLength(500, ErrorMessage = "La ruta no puede exceder 500 caracteres")]
        string OutputPath);
}
