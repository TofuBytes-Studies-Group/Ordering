namespace Ordering.API.DTOs
{
    public class OrderLineDto
    {
        public Guid Id { get; set; }
        public string DishName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }

        public override string ToString()
    {
        return $"OrderLine ID: {Id}, Dish: {DishName}, Quantity: {Quantity}, Price: {Price}";
    }
        
    }
    
}