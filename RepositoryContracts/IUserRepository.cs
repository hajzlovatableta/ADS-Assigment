using Entities;

namespace RepositoryContracts;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task DeleteAsync(int id);
    Task UpdateAsync(User user);
    Task<User> GetByIdAsync(int id);
    IQueryable<User> GetAll();
}