using Application.Common.Interfaces.Repositories;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
    }
}
