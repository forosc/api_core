using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.POCO
{
    public record BalanceData
    {
        public string CuentaMayor { get; set; } = "";
        public string Almacen { get; set; } = "";
        public string Material { get; set; } = "";
        public string StockTotal { get; set; } = "";
        public string UnidadMedida { get; set; } = "";
        public string ValorTotal { get; set; } = "";
        public string Moneda { get; set; } = "";
        public string Descripcion { get; set; } = ""; // Por si acaso
    }
}
