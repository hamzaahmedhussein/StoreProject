using Core.Entities;

namespace Core.Interfaces
{
    public interface IBasketService
    {
        Task<CustomerBasket> GetBasketAsync(string id);
        Task<CustomerBasket> RemoveItemFromBasketAsync(string basketId, int productId);
        Task<bool> DeleteBasketAsync(string id);
        Task<CustomerBasket> AddItemToBasketAsync(string basketId, int productId);

    }
}
