namespace DesafioWaBack.Domain.Models;

public class Order
{
    public long Number { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string Address { get; set; }
    public DeliveryTeam? DeliveryTeam { get; set; }
    public IEnumerable<OrderItem> Products { get; set; }
}