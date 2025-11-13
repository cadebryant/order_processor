using OrderProcessor.Service.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Domain
{
    public class Order(
        int id,
        Customer customer,
        string type,
        double amount,
        DateTime? date,
        string region,
        string state)
    {
        public int Id { get; init; } = id;
        public Customer Customer { get; init; } = customer;
        public string Type { get; init; } = type;
        public double Amount { get; init; } = amount;
        public DateTime? Date { get; set; } = date;
        public string Region { get; init; } = region;
        public string State { get; init; } = state;
        public double Net { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
