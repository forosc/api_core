using MediatR;

namespace Application.UseCases.ProcessSapData
{
    public record ProcessSapDataCommand(
        string InputPath,
        string OutputPath) : IRequest<ProcessSapDataResult>;

    
}
