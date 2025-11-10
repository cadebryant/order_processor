using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Domain
{
    public class InMemoryCustomerCache : ICustomerCache
    {
        public Customer? GetCustomer(string name)
        {
            throw new NotImplementedException();
        }
    }
}
