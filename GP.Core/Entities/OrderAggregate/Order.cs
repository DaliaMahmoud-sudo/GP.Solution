using GP.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace GP.Core.Entites.OrderAggregate
{
    public class Order
    {
        public Order()
        {
            
        }
        public Order(string buyerEmail, ShippingAddress shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }
        public int Id { get; set; }

        [ValidateNever]
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public Status Status { get; set; } = Status.Pending;
        [ValidateNever]
        public ShippingAddress ShippingAddress { get; set; }
        [ValidateNever]
        public DeliveryMethod DeliveryMethod { get; set; }
        [ValidateNever]
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();

        public decimal SubTotal { get; set; }
         
        public decimal GetTotal()
        {
            return SubTotal+DeliveryMethod.Cost;
        }
        public string PaymentIntentId { get; set; }


    }
}
