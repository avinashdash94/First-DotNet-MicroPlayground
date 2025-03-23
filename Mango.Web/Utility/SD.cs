namespace Mango.Web.Utility
{
    //This class constains static details used in the project
    public class SD
    {
        //CouponAPIBase Url is for the Url of the Coupon service(Mango.Services.CouponAPI project)
        public static string CouponAPIBase { get; set; }
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
