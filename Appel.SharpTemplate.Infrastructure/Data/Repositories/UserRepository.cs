using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IRepositoryBase<User> _repository;

        public UserRepository(IRepositoryBase<User> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetAsync(Expression<Func<User, bool>> filter = null)
        {
            return await _repository.GetAsync(filter);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(User entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(User entity)
        {
            await _repository.UpdateAsync(entity);
        }
    }
}
