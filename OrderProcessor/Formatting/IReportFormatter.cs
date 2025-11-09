using OrderProcessor.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Formatting
{
    public interface IReportFormatter
    {
        string FormatReport(OrdersReport reportData);
    }
}
