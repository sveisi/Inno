using AutoMapper;
using Inno.Data;
using Inno.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services
{
    public abstract class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        protected readonly InnoContext ctx;
        protected readonly IMapper mapper;
        protected readonly DbSet<TEntity> entities;

        public BaseService(InnoContext ctx, IMapper mapper)
        {
            this.ctx = ctx;
            this.mapper = mapper;
            entities = ctx.Set<TEntity>();
        }

        //این متد هم بهتر است فقط ویو برگرداند ولی فعلا برای کومبوها نیاز دارم
        public virtual async Task<IList<TEntity>> GetAsync()
        {
            return await entities.AsNoTracking().ToListAsync();
        }

        //public virtual async Task<TEntity> GetByIdAsync(object id)
        //{
        //    if (id == null) throw new ArgumentNullException(nameof(id));
        //    return await entities.FindAsync(new object[] { id });
        //}

        /*public virtual async Task<TView> GetByIdAsync<TView>(object id)
        {
            var query = entities.AsNoTracking().ProjectTo<TView>(mapper.ConfigurationProvider);

            var result = await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id") == id);

            return result;
        }

        public virtual Paging<TView> Get(GridifyQuery gridify)
        {
            var query = entities.AsNoTracking().ProjectTo<TView>(mapper.ConfigurationProvider);
            var result = query.Gridify(gridify);
            return result;
        }*/

        protected virtual async Task<int> SaveChangesAsync()
        {
            return await ctx.SaveChangesAsync();
        }

        protected virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            entities.Add(entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

        protected virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            ctx.Update(entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

        protected virtual async Task DeleteAsync(TEntity entity)
        {
            entities.Remove(entity);
            await ctx.SaveChangesAsync();
        }

        public virtual async Task<TEntity> DeleteAsync(object id)
        {
            var entity = await entities.FindAsync(id);
            await DeleteAsync(entity);

            return entity;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await using var tx = await ctx.Database.BeginTransactionAsync();
            try
            {
                await action();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            //نحوه استفاده
            /*
             await someService.ExecuteInTransactionAsync(async () =>
            {
                await orderSrv.CreateAsync(orderVm);      // داخلش SaveChanges می‌زند
                await inventorySrv.DecreaseAsync(items);  // داخلش SaveChanges می‌زند
                await invoiceSrv.CreateAsync(invoiceVm);  // داخلش SaveChanges می‌زند
            });
             */
        }
    }
}