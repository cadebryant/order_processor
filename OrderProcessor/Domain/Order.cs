using OrderProcessor.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Domain
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
        public Customer Customer { get; set; } = customer;
        public string Type { get; set; } = type;
        public double Amount { get; set; } = amount;
        public DateTime? Date { get; set; } = date;
        public string Region { get; set; } = region;
        public string State { get; set; } = state;
    }
}
