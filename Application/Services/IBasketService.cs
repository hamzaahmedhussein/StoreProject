using Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces
{
    public interface IBasketService
    {
        Task<CustomerBasket> GetBasketAsync(HttpContext httpContext);
        Task<CustomerBasket> RemoveItemFromBasketAsync(HttpContext httpContext, int productId);
        Task<bool> DeleteBasketAsync(HttpContext httpContext);
        Task<CustomerBasket> AddItemToBasketAsync(HttpContext httpContext, int productId);
        Task<string> GetBasketIdFromCookieAsync(HttpContext httpContext);
    }
}
