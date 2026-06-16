using AutoMapper;
using Inno.Data;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class UnitService : BaseService<Unit>, IUnitService
    {
        public UnitService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public async Task<List<LookupView<int>>> GetLookupAsync()
        {
            return await entities.Select(x => new LookupView<int>(x.Id, x.Name)).ToListAsync();
        }
    }
}