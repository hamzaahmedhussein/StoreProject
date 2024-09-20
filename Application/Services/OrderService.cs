using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepo;

        public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepo)
        {
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);

            if (basket == null)
                throw new ArgumentNullException(nameof(basket), "Basket not found");

            if (basket.Items == null || !basket.Items.Any())
                throw new ArgumentNullException(nameof(basket.Items), "Basket has no items");

            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                if (productItem == null)
                    throw new ArgumentNullException(nameof(productItem), "Product not found");

                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.Picture);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            if (deliveryMethod == null)
                throw new ArgumentNullException(nameof(deliveryMethod), "Delivery method not found");

            if (shippingAddress == null)
                throw new ArgumentNullException(nameof(shippingAddress), "Shipping address is required");

            var subtotal = items.Sum(item => item.Price * item.Quantity);

            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal, "i");

            await _unitOfWork.Repository<Order>().AddAsync(order);

            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0) return null;

            await _basketRepo.DeleteBasketAsync(basketId);

            return order;
        }

        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<DeliveryMethod> AddDeliveryMethodAsync(string shortName, string deliveryTime, string description, decimal price)
        {
            var deliveryMethod = new DeliveryMethod
            {
                ShortName = shortName,
                DeliveryTime = deliveryTime,
                Description = description,
                Price = price
            };

            await _unitOfWork.Repository<DeliveryMethod>().AddAsync(deliveryMethod);

            var result = await _unitOfWork.CompleteAsync();

            return result > 0 ? deliveryMethod : null;
        }


        public Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrderWithItemAndOrderingSpesification(id, buyerEmail);
            return _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderWithItemAndOrderingSpesification(buyerEmail);
            return _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}
