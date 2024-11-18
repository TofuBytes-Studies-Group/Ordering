using Ordering.Domain.Entities;

namespace Ordering.Domain.Interfaces;

public interface IDish
{
    public Guid Id { get; }
    public string Name { get; }
    public int Price { get; }
    Dish? GetById(Guid id);
    IEnumerable<Dish> GetAll();
}