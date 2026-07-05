using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Data.Queries;
using Inno.Helper;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IUserContextService userContextSrv;
        private readonly ICreditTransactionService creditSrv;

        public OrderService(InnoContext ctx, IMapper mapper, IUserContextService userContextSrv, ICreditTransactionService creditSrv) : base(ctx, mapper)
        {
            this.userContextSrv = userContextSrv;
            this.creditSrv = creditSrv;
        }

        public Paging<OrderListView> Get(GridifyQuery gridify)
        {
            var res = entities
                .Where(x => x.ConfirmedAt.HasValue)
                .ProjectTo<OrderListView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<List<OrderItemView>> GetCurrentOrderItemsAsync()
        {
            return await ctx.Orders.CurrentOrder(userContextSrv.UserId.Value)
                .SelectMany(x => x.Items)
                .ProjectTo<OrderItemView>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<Result<OrderView>> GetOrderAsync(int id)
        {
            var order = await entities.Where(x => x.Id == id)
                .ProjectTo<OrderView>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return order;
        }

        ///<summary>سفارش ثبت شده برای کاربر جاری</summary>
        public async Task<OrderView> GetCurrentUserOrderAsync(int id)
        {
            var order = await entities.Where(
                x => x.Id == id &&
                x.CreatedBy == userContextSrv.UserId &&
                x.ConfirmedAt.HasValue)
                .ProjectTo<OrderView>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return order;
        }

        public async Task<OrderSummaryView> GetCurrentOrderSummaryAsync()
        {
            // استفاده از Left Join برای واکشی اطلاعات مشتری و والد در یک کوئری
            var data = await (from c in ctx.Customers
                              where c.Id == userContextSrv.CustomerId
                              join p in ctx.Customers on c.ParentCustomerId equals p.Id into parents
                              from parent in parents.DefaultIfEmpty()
                              select new
                              {
                                  c.Id,
                                  c.DiscountPercent,
                                  c.CreditBalance,
                                  c.ParentCustomerId,
                                  ParentCredit = (decimal?)parent.CreditBalance
                              }).FirstOrDefaultAsync();

            if (data == null) return new OrderSummaryView();

            bool isSubCustomer = data.ParentCustomerId != null;

            // اگر زیرمجموعه بود، اعتبار والد و در غیر این صورت اعتبار خودش
            decimal finalCredit = isSubCustomer ? (data.ParentCredit ?? 0) : data.CreditBalance;

            var items = await GetCurrentOrderItemsAsync();
            var totalAmount = items.Sum(x => x.Qty * x.UnitPrice);

            // محاسبه تخفیف فقط برای مشتری اصلی
            decimal discount = 0;
            if (!isSubCustomer)
                discount = totalAmount * data.DiscountPercent / 100m;

            var paymentAmount = totalAmount - discount;

            return new OrderSummaryView
            {
                ItemsCount = items.Count,
                TotalAmount = totalAmount,
                DiscountAmount = discount,
                PaymentAmount = paymentAmount,
                CreditAmount = finalCredit,
                IsSubCustomer = isSubCustomer
            };
        }

        public async Task<Result<OrderItemView>> AddItemAsync(string productId, decimal qty)
        {
            if (qty <= 0)
                return "مقدار صحیح نیست";

            using var transaction = await ctx.Database.BeginTransactionAsync();
            try
            {
                var prd = await ctx.Products
                    .Where(x => x.Id == productId)
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Price
                    })
                    .FirstOrDefaultAsync();

                if (prd == null)
                    return "کالا یافت نشد";

                var ord = await ctx.Orders.CurrentOrder(userContextSrv.UserId.Value)
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.CreatedBy == userContextSrv.UserId && x.Status == OrderStatus.Draft);
                //اگر سفارش جاری پیدا نشد یک سفارش ایجاد میشه
                if (ord == null)
                {
                    ord = new Order
                    {
                        CreatedBy = userContextSrv.UserId.Value,
                        Status = OrderStatus.Draft,
                        Items = new List<OrderItem>()
                    };
                    ctx.Orders.Add(ord);
                }

                ord.Items ??= new List<OrderItem>();

                SKU skuEntry = null;
                OrderItem existingItem = null;

                var orderItemSkuIds = ord.Items
                    .Where(x => x.ProductId == productId)
                    .Select(x => x.SKUId)
                    .Distinct()
                    .ToList();

                //چک میکنیم آیا این طاقه‌هایی که الان در سبدش هست، باز هم موجودی دارند؟اگه داشتن از همانهاانتخاب میشود
                if (orderItemSkuIds.Count > 0)
                {
                    var candidateSkus = await ctx.SKUs
                        .Where(x => orderItemSkuIds.Contains(x.Id))
                        .ToListAsync();

                    var matchedSku = candidateSkus
                        .Where(x => (x.CurrentQty - x.ReservedQty) >= qty)
                        .OrderBy(x => (x.CurrentQty - x.ReservedQty) == qty ? 0 : 1)
                        .ThenByDescending(x => x.CurrentQty - x.ReservedQty)
                        .FirstOrDefault();

                    if (matchedSku != null)
                    {
                        skuEntry = matchedSku;
                        existingItem = ord.Items.FirstOrDefault(x => x.SKUId == matchedSku.Id);
                    }
                }

                //اگر طاقه‌های اقلام موجودی ندارند، کوئری یک طاقه جدید پیدا میکند که شرط متراژ را داشته باشد
                if (skuEntry == null)
                {
                    var skus = await ctx.SKUs
                        .Where(x => x.ProductId == productId)
                        .ToListAsync();

                    skuEntry = skus
                        .Where(x => (x.CurrentQty - x.ReservedQty) >= qty)
                        .OrderBy(x => (x.CurrentQty - x.ReservedQty) == qty ? 0 : 1)
                        .ThenByDescending(x => x.CurrentQty - x.ReservedQty)
                        .FirstOrDefault();

                    if (skuEntry == null)
                        return "موجودی کافی یافت نشد";

                    existingItem = ord.Items.FirstOrDefault(x => x.SKUId == skuEntry.Id);
                }

                if ((skuEntry.CurrentQty - skuEntry.ReservedQty) < qty)
                    return "موجودی در حین عملیات تغییر کرد";

                if (existingItem != null)
                {
                    existingItem.Qty += qty;
                    existingItem.Amount = existingItem.Qty * prd.Price;
                }
                else
                {
                    existingItem = new OrderItem
                    {
                        ProductId = prd.Id,
                        SKUId = skuEntry.Id,
                        Qty = qty,
                        UnitPrice = prd.Price,
                        Amount = qty * prd.Price,
                        Status = OrderItemStatus.Pending
                    };

                    ord.Items.Add(existingItem);
                }

                skuEntry.ReservedQty += qty;
                ord.TotalAmount = ord.Items.Sum(x => x.Amount);

                await ctx.SaveChangesAsync();
                await transaction.CommitAsync();

                var res = new OrderItemView
                {
                    Id = existingItem.Id,
                    SKUId = existingItem.SKUId,
                    ProductId = existingItem.ProductId,
                    ProductName = prd.Name,
                    Qty = existingItem.Qty,
                    UnitPrice = existingItem.UnitPrice,
                    Amount = existingItem.Amount
                };

                return Result<OrderItemView>.Ok(res);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return "تداخل در عملیات، لطفا مجددا تلاش کنید.";
            }
            catch
            {
                await transaction.RollbackAsync();
                return "خطای دیتابیس";
            }
        }

        public async Task<Result<int>> DeleteAsync(int id)
        {
            var order = await entities.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (order == null)
                return Resources.SharedResource.RecordNotFoundMsg;
            if (order.CreatedBy != userContextSrv.UserId)
                return Resources.SharedResource.AccessDeniedMsg;

            entities.Remove(order);
            await ctx.SaveChangesAsync();

            return Result<int>.Ok(1);
        }

        public async Task<Result<int>> CheckoutAsync()
        {
            var order = await ctx.Orders.CurrentOrder(userContextSrv.UserId.Value)
                .Include(x => x.Items)
                .Include(x => x.CreatedByUser.Customer.ParentCustomer)
                .FirstOrDefaultAsync();

            if (order == null)
                return Resources.SharedResource.RecordNotFoundMsg;

            if (order.ConfirmedAt.HasValue)
                return "سفارش قبلا تایید شده است.";

            var cust = order.CreatedByUser.Customer;
            if (cust == null)
                return "کاربر با مشتری ارتباط ندارد";

            if (order.Items.Count == 0)
                return "سبد سفارش شما خالی است!";

            order.TotalAmount = order.Items.Sum(x => x.Qty * x.UnitPrice);
            //اگر نماینده باشد درصد تخفیف از فاکتور کم میشود
            //اگر زیر مجموعه باشد کل مبلغ از حساب نماینده کم میشود ولی مبلغ تخفیف به حساب نماینده واریز میشود بصورت هدیه 
            if (cust.ParentCustomer == null)
            {
                order.DiscountAmount = order.TotalAmount * cust.DiscountPercent / 100m;
            }
            else
            {
                order.DiscountAmount = 0;
            }
            order.PaymentAmount = order.TotalAmount - order.DiscountAmount;

            var buyerCustomer = cust.ParentCustomer ?? cust;
            if (buyerCustomer.CreditBalance < order.PaymentAmount)
                return "اعتبار کافی برای ثبت سفارش وجود ندارد.";

            await ExecuteInTransactionAsync(async () =>
            {
                //تایید سفارش
                order.ConfirmedAt = System.DateTime.Now;
                await ctx.SaveChangesAsync();

                string desc = cust.ParentCustomer == null ? $"بابت سفارش {order.Id}" : $"بابت سفارش زیرمجموعه {cust.FullName} با شماره سفارش {order.Id}";
                
                //کم کردن اعتبار از نماینده
                var creditResult = await creditSrv.CreateAsync(new CreditTransactionView
                {
                    CustomerId = buyerCustomer.Id,
                    Amount = order.PaymentAmount,
                    IsIncrement = false,
                    RelatedOrderId = order.Id,
                    Description = desc
                });
                // چک کردن موفقیت در سرویس اعتبار
                if (!creditResult.Success)
                    throw new Exception(creditResult.Error); //for Rollback

                //در صورت خرید زیر مجموعه اعتبار نماینده به میزان تحفیف مشخص شده افزایش می‌یابد
                if (cust.ParentCustomer != null)
                {
                    var giftAmount = order.TotalAmount * cust.ParentCustomer.DiscountPercent / 100m;
                    if (giftAmount > 0)
                    {
                        var giftCreditResult = await creditSrv.CreateAsync(new CreditTransactionView
                        {
                            CustomerId = cust.ParentCustomer.Id,
                            Amount = giftAmount,
                            IsIncrement = true,
                            RelatedOrderId = order.Id,
                            Description = desc
                        });

                        // چک کردن موفقیت در سرویس اعتبار
                        if (!giftCreditResult.Success)
                            throw new Exception(giftCreditResult.Error); //for Rollback
                    }
                }
            });

            return order.Id;
        }

        public async Task<Result> DeleteItemAsync(int id)
        {
            var ord = await entities.CurrentOrder(userContextSrv.UserId.Value)
                .Select(x => new
                {
                    x.Id,
                    x.ConfirmedAt
                }).FirstOrDefaultAsync();
            if (ord == null || ord.ConfirmedAt.HasValue)
                return "سفارش بعد از تایید قابل تغییر نیست.";

            var item = await ctx.OrderItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
                return Resources.SharedResource.RecordNotFoundMsg;

            await ExecuteInTransactionAsync(async () =>
            {
                ctx.OrderItems.Remove(item);
                await ctx.SaveChangesAsync();

                //آزاد کردن موجودی رزرو
                var reservedRes = await ctx.SKUs.Where(x => x.Id == item.SKUId)
                    .ExecuteUpdateAsync(x => x
                        .SetProperty(s => s.ReservedQty, s => s.ReservedQty - item.Qty));

                if (reservedRes == 0)
                    throw new Exception(Resources.SharedResource.RecordNotFoundMsg);
            });

            return Result.Ok();
        }
    }
}