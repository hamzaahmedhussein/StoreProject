using Application.DTOs;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IBasketService
    {
        Task<CustomerBasket> GetBasketAsync(string id);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string id);
        Task<CustomerBasket> AddItemToBasketAsync(string basketId, BasketItemDto item);

    }
}
