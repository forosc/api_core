using Application.UseCases.ProcessSapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Reports
{
    public interface IProcessSapDataCommandHandler
    {
        Task Handle(
            ProcessSapDataCommand command,
            CancellationToken cancellationToken = default);
    }
}
