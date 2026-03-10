using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    [Table("Menu_Orders")]
    public class OrderDetails
    {
        public int FoodSizeId { get; set; }
        [ValidateNever]
        public FoodSize FoodSize { get; set; }

        public int OrderId { get; set; }
        [ValidateNever]
        public Order Order { get; set; }

        [DisplayName("Số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng không được là 1 số âm và lớn hơn 0.")]
        public int Count { get; set; }
    }
}
