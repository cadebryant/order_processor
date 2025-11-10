using Microsoft.Extensions.Caching.Memory;

namespace OrderProcessor.Domain
{
    public class InMemoryCustomerCache(IMemoryCache memoryCache) : ICustomerCache
    {
        private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

        public Customer? GetCustomer(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Customer name cannot be null or empty.", nameof(name));
            }

            if (!_memoryCache.TryGetValue(name, out Customer? customer))
            {
                _memoryCache.Set(name, new Customer(name));
                return null;
            }

            return customer;
        }
    }
}
