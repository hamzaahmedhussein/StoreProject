using Core.Entities;
using Core.Interfaces;
using Core.Models;

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
        public async Task<CustomerBasket> RemoveItemFromBasketAsync(string basketId, int productId)
        {
            var basket = await _unitOfWork.BasketRepository.GetBasketAsync(basketId);

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

            product.Quantity++;
            await _unitOfWork.Repository<Product>().UpdateAsync(product);

            if (basketItem.Quantity == 1)
                basket.Items.Remove(basketItem);
            else
                basketItem.Quantity--;



            basket.TotalPrice = basket.Items.Sum(i => i.Price * i.Quantity);

            await _unitOfWork.BasketRepository.UpdateBasketAsync(basket);

            await _unitOfWork.CompleteAsync();

            return basket;
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            var result = await _unitOfWork.BasketRepository.DeleteBasketAsync(id);
            await _unitOfWork.CompleteAsync();
            return result;
        }

        public async Task<CustomerBasket> AddItemToBasketAsync(string basketId, int productId)
        {
            var basket = await _unitOfWork.BasketRepository.GetBasketAsync(basketId) ?? new CustomerBasket(basketId);

            var item = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (item == null)
            {
                throw new Exception("Product not found"); // Consider using a custom exception
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
                item.Quantity--;

            }
            else
            {
                basketItem.Quantity++;

                item.Quantity--;

            }

            // Incrementally update the total price
            basket.TotalPrice += item.Price;

            // Update the basket and complete the transaction
            await _unitOfWork.BasketRepository.UpdateBasketAsync(basket);
            await _unitOfWork.CompleteAsync();

            return basket;
        }
    }
}
