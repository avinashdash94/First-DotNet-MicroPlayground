﻿namespace Mango.Services.CouponAPI.Models.Dto
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public double CouponAmount { get; set; }
        public int MinAmount { get; set; }
    }
}
