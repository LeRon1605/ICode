using API.Models.Data;
using API.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Repository
{
    public interface IUserRepository: IRepository<User>
    {
        User GetUserWithRole(Expression<Func<User, bool>> expression);
    }
    public class UserRepository: BaseRepository<User>, IUserRepository
    {
        public UserRepository(ICodeDbContext context): base(context)
        {

        }

        public User GetUserWithRole(Expression<Func<User, bool>> expression)
        {
            return _context.Users.Include(user => user.Role).FirstOrDefault(expression);
        }
    }
}
