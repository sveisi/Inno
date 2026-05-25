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
            return await ctx.Orders.CurrentOrder(userContextSrv.UserId)
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
            var cust = await ctx.Customers
                .Where(x => x.Id == userContextSrv.CustomerId)
                .Select(x => new
                {
                    x.Id,
                    x.DiscountPercent,
                    x.CreditBalance,
                    x.ParentCustomerId
                })
                .FirstOrDefaultAsync();

            var items = await GetCurrentOrderItemsAsync();

            var totalItems = items.Count;
            var totalAmount = items.Sum(x => x.Qty * x.UnitPrice); ;

            decimal discount = 0;
            if (cust.ParentCustomerId == null)
                discount = totalAmount * cust.DiscountPercent / 100m;

            var paymentAmount = totalAmount - discount;

            return new OrderSummaryView
            {
                ItemsCount = (int)totalItems,
                TotalAmount = totalAmount,
                DiscountAmount = discount,
                PaymentAmount = paymentAmount,
                CreditAmount = cust.CreditBalance
            };
        }

        public async Task<Result<OrderItemView>> AddItemAsync(string productId, decimal qty)
        {
            if (qty <= 0)
                return "مقدار صحیح نیست";
            //بررسی وجود کالا
            var prd = await ctx.Products.Where(x => x.Id == productId)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.EnName,
                    x.UnitId,
                    x.Price
                }).FirstOrDefaultAsync();
            if (prd == null)
                return string.Format(Resources.SharedResource._0_NotFoundMsg, Resources.SharedResource.Product);

            //بررسی موجودی
            var skus = await ctx.SKUs
                .Where(x => x.ProductId == productId)
                .Select(x => new
                {
                    x.Id,
                    x.ProductId,
                    x.LocationId,
                    x.CurrentQty,
                    x.ReservedQty,
                    AvailableQty = x.CurrentQty - x.ReservedQty,
                })
                .Where(x => x.AvailableQty >= qty)
                .ToListAsync();
            if (skus.Count == 0)
                return "موجودی این کالا کافی نیست";
            //انتخاب طاقه
            //ابتدا همان متراژ یا بزرگترین طاقه را انتخاب میکنیم
            var selected = skus.FirstOrDefault(x => x.AvailableQty == qty) ?? skus.OrderByDescending(x => x.AvailableQty).First();
            var selectedSku = ctx.SKUs.Find(selected.Id);
            //TODO: control race condition!
            selectedSku.ReservedQty += qty;

            Order ord = await ctx.Orders.CurrentOrder(userContextSrv.UserId).FirstOrDefaultAsync();
            //ایجاد سفارش در صورت عدم وجود
            if (ord == null)
            {
                ord = new Order
                {
                    CreatedBy = userContextSrv.UserId,
                    Status = OrderStatus.Draft,
                };

                entities.Add(ord);
            }
            //ایحاد قلم سفارش
            var item = new OrderItem
            {
                ProductId = selectedSku.ProductId,
                SKUId = selectedSku.Id,
                Qty = qty,
                UnitPrice = prd.Price,
                Amount = qty * prd.Price,
                Status = OrderItemStatus.Pending
            };
            //اضافه کردن قلم به سفارش
            ord.Items ??= new List<OrderItem>();
            ord.Items.Add(item);
            ord.TotalAmount += item.Amount;

            await ctx.SaveChangesAsync();

            var res = new OrderItemView
            {
                Id = item.Id,
                SKUId = item.SKUId,
                ProductId = item.ProductId,
                ProductName = prd.Name,
                Qty = item.Qty,
                UnitPrice = prd.Price,
                Amount = qty * prd.Price,
            };
            return Result<OrderItemView>.Ok(res);
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
            var order = await ctx.Orders.CurrentOrder(userContextSrv.UserId)
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

                //کم کردن اعتبار از نماینده
                var creditResult = await creditSrv.CreateAsync(new CreditTransactionView
                {
                    CustomerId = buyerCustomer.Id,
                    Amount = order.PaymentAmount,
                    IsIncrement = false,
                    RelatedOrderId = order.Id,
                    Description = $"بابت سفارش {order.Id}"
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
                            Description = $"بابت سفارش زیرمجوعه {cust.FullName} با شماره سفارش {order.Id}"
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
            var ord = await entities.CurrentOrder(userContextSrv.UserId)
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