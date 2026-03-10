using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ecommerce.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        public int FoodSizeId { get; set; }
        [ValidateNever]
        public FoodSize FoodSize { get; set; }

        public string Unit { get; set; }

        public int Quantity { get; set; }
    }
}
