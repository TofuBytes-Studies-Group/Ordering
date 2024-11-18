using Ordering.Domain.Interfaces;

namespace Ordering.Domain.Entities;

public class Dish : IDish
{
    public Dish(Guid id, string name, int price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
    
    public Dish() {}
    
    public Guid Id { get; }
    public string Name { get; }
    public int Price { get; }

    public Dish? GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Dish> GetAll()
    {
        throw new NotImplementedException();
    }
}