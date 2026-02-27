using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class UnitService : BaseService<Unit>, IUnitService
    {
        public UnitService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }
    }
}