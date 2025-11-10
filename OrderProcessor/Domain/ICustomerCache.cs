using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Domain
{
    public interface ICustomerCache
    {
        public Customer? GetCustomer(string name);
    }
}
