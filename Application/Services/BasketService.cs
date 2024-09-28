using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class BasketService : IBasketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BasketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket> GetBasketAsync(HttpContext httpContext)
        {
            string id = await GetBasketIdFromCookieAsync(httpContext);
            return await _unitOfWork.BasketRepository.GetBasketAsync(id);
        }
        public async Task<CustomerBasket> RemoveItemFromBasketAsync(HttpContext httpContext, int productId)
        {
            var basket = await GetBasketAsync(httpContext);

            if (basket == null)
            {
                throw new Exception("Basket not found");
            }

            var basketItem = basket.Items.FirstOrDefault(i => i.Id == productId);

            if (basketItem == null)
            {
                throw new Exception("Item not found in the basket");
            }

            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }


            if (basketItem.Quantity == 1)
                basket.Items.Remove(basketItem);
            else
                basketItem.Quantity--;



            basket.TotalPrice = basket.Items.Sum(i => i.Price * i.Quantity);

            await _unitOfWork.BasketRepository.UpdateBasketAsync(basket);

            await _unitOfWork.CompleteAsync();

            return basket;
        }

        public async Task<bool> DeleteBasketAsync(HttpContext httpContext)
        {
            string id = await GetBasketIdFromCookieAsync(httpContext);

            var result = await _unitOfWork.BasketRepository.DeleteBasketAsync(id);
            await _unitOfWork.CompleteAsync();
            return result;
        }

        public async Task<CustomerBasket> AddItemToBasketAsync(HttpContext httpContext, int productId)
        {
            string basketId = await GetBasketIdFromCookieAsync(httpContext);

            if (string.IsNullOrEmpty(basketId))
            {
                basketId = Guid.NewGuid().ToString();
                await SetBasketIdCookieAsync(httpContext, basketId);
            }

            var basket = await _unitOfWork.BasketRepository.GetBasketAsync(basketId)
                         ?? new CustomerBasket(basketId);

            var item = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (item == null)
            {
                throw new Exception("Product not found");
            }

            if (item.Quantity <= 0)
            {
                throw new Exception("Product is out of stock");
            }

            var basketItem = basket.Items.FirstOrDefault(i => i.Id == item.Id);

            if (basketItem == null)
            {
                basketItem = new BasketItem
                {
                    Id = item.Id,
                    ProductName = item.Name,
                    Price = item.Price,
                    Quantity = 1,
                    Picture = item.Picture,
                    Brand = item.Brand,
                    Category = item.Category
                };
                basket.Items.Add(basketItem);
            }
            else
            {
                basketItem.Quantity++;
            }


            basket.TotalPrice += item.Price;

            await _unitOfWork.BasketRepository.UpdateBasketAsync(basket);
            await _unitOfWork.CompleteAsync();

            return basket;
        }

        private async Task SetBasketIdCookieAsync(HttpContext httpContext, string basketId)
        {
            CookieOptions options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
            httpContext.Response.Cookies.Append("BasketId", basketId, options);
        }

        public async Task<string> GetBasketIdFromCookieAsync(HttpContext httpContext)
        {
            return httpContext.Request.Cookies["BasketId"];
        }
    }
}
