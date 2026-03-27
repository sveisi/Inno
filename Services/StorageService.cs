using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class StorageService : BaseService<Storage>, IStorageService
    {
        public StorageService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public Paging<StorageView> Get(GridifyQuery gridify)
        {
            var res = entities.AsNoTracking().ProjectTo<StorageView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<List<StorageView>> GetAllStorageAsync()
        {
            return await entities.ProjectTo<StorageView>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<StorageView> GetStorageAsync(string id)
        {
            var res = await entities.ProjectTo<StorageView>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == id);

            return res;
        }

        public async Task<Storage> CreateAsync(StorageView v)
        {
            var n = mapper.Map<Storage>(v);
            var res = await AddAsync(n);

            return res;
        }

        public async Task<Storage> UpdateAsync(StorageView v)
        {
            var n = mapper.Map<Storage>(v);
            var res = await UpdateAsync(n);

            return res;
        }
    }
}