using Domain.Entities;
using Domain.Interfaces;
using RepositoryFramework;

namespace Service
{
    public class ClienteService : BaseService<Cliente, int>
    { 
        protected override IRepository<Cliente, int> repo { get; set; } = new ClienteRepository();
    }
}
