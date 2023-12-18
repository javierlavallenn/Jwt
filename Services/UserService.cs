using Application.Context;
using Application.Entities;
using Application.Interface;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _store;

        public UserService(ApplicationContext store) => _store = store;

        public async Task<bool> CreateAsync(User user)
        {
            _store.Users.Add(user);
            _store.SaveChanges();
            return true;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _store.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }
    }
}
