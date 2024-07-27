using Application.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services
{
    public class BasketService : IBasketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BasketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket> GetBasketAsync(string id)
        {
            return await _unitOfWork.BasketRepository.GetBasketAsync(id);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var updatedBasket = await _unitOfWork.BasketRepository.UpdateBasketAsync(basket);
            await _unitOfWork.CompleteAsync();
            return updatedBasket;
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            var result = await _unitOfWork.BasketRepository.DeleteBasketAsync(id);
            await _unitOfWork.CompleteAsync();
            return result;
        }

        public async Task<CustomerBasket> AddItemToBasketAsync(string basketId, BasketItemDto item)
        {
            var basket = await _unitOfWork.BasketRepository.GetBasketAsync(basketId) ?? new CustomerBasket(basketId);

            var basketItem = basket.Items.FirstOrDefault(i => i.Id == item.Id);
            if (basketItem == null)
            {
                basketItem = new BasketItem
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    PictureUrl = item.PictureUrl,
                    Brand = item.Brand,
                    Type = item.Type
                };
                basket.Items.Add(basketItem);
            }
            else
            {
                basketItem.Quantity += item.Quantity;
            }

            await _unitOfWork.BasketRepository.UpdateBasketAsync(basket);

            return basket;
        }
    }
}
