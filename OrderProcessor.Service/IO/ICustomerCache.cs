using OrderProcessor.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.IO
{
    public interface ICustomerCache
    {
        public Customer? GetCustomer(string name);
    }
}
