using Application.Entities;

namespace Application.Interface
{
    public interface IUserService
    {
        Task<User> FindByEmailAsync(string email);

        Task<bool> CreateAsync(User user);
    }
}
