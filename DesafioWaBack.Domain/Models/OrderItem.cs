namespace DesafioWaBack.Domain.Models;

public class OrderItem
{
    public long Id { get; set; }
    public Order Order { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
}