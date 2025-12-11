using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Processed
{
    public class ProcessedBalanceData
    {
        public int Id { get; set; }
        public string Material { get; set; } = string.Empty;
        public decimal StockTotal { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public string Moneda { get; set; } = string.Empty;
        public string CuentaMayor { get; set; } = string.Empty;
        public string Almacen { get; set; } = string.Empty;
        public DateTime ProcessingDate { get; set; } = DateTime.UtcNow;
        public string SourceFile { get; set; } = string.Empty;
        public ProcessingStatus Status { get; set; } = ProcessingStatus.Pending;
    }

    public enum ProcessingStatus
    {
        Pending,
        Validated,
        Processed,
        Error,
        Archived
    }
}
