using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ProcessSapData
{
    public class ProcessSapDataCommandValidator : AbstractValidator<ProcessSapDataCommand>
    {
        public ProcessSapDataCommandValidator()
        {
            RuleFor(x => x.InputPath)
                .NotEmpty().WithMessage("La ruta de entrada es requerida")
                .Must(BeAValidPath).WithMessage("La ruta de entrada no es válida")
                .Must(File.Exists).WithMessage("El archivo de entrada no existe en la ruta especificada")
                .Must(path =>
                    path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Solo se aceptan archivos .txt o .csv");

            RuleFor(x => x.OutputPath)
                .NotEmpty().WithMessage("La ruta de salida es requerida")
                .Must(BeAValidPath).WithMessage("La ruta de salida no es válida")
                .Must(path => !File.Exists(path) || HasWritePermission(path))
                .WithMessage("No se puede escribir en la ruta de salida especificada")
                .Must(path => path.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                .WithMessage("El archivo de salida debe tener extensión .xlsx");
        }

        private bool BeAValidPath(string path)
        {
            try
            {
                var fullPath = Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool HasWritePermission(string filePath)
        {
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Write))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
