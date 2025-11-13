using OrderProcessor.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Formatting
{
    public interface IReportFormatter
    {
        string FormatReport(OrdersReport reportData);
    }
}
