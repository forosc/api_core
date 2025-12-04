using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.POCO
{
    public record ConsumptionDataWithUnits(
        string Consumption = "",
        string ConsumptionUnit = "",
        string Stock = "",
        string StockUnit = ""
    );
}
