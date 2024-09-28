namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUserHelpers _userHelpers;
        private readonly IAccountService _accountService;
        public CustomerService(IUserHelpers userHelpers, IAccountService accountService)
        {
            _userHelpers = userHelpers;
            _accountService = accountService;
        }


    }
}
