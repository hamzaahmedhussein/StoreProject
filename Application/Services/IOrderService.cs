using Core.Entities;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethod, string basketId, Address shippingAddress);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
        Task<DeliveryMethod> AddDeliveryMethodAsync(string shortName, string deliveryTime, string description, decimal price);
    }
}
