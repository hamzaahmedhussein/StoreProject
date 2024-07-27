namespace Application.DTOs
{
    public class OrderDto
    {
        public string BuyerEmail { get; set; }
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public AddressDto ShippintToAddress { get; set; }
    }
}
