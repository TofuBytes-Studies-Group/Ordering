using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure;

namespace Ordering.SystemTests
{
    public static class TestHelper
    {
        public static OrderingContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderingContext>()
                .UseInMemoryDatabase("testDB")
                .Options;

            return new OrderingContext(options);
        }
    }
}