using System.Collections.Generic;

namespace SauceDemoTests.Configuration
{
    public class TestDataConfig
    {
        public required string BaseUrl { get; set; }
        public required string InventoryUrl { get; set; }
        public required string CartUrl { get; set; }
        public required string CheckoutStepOneUrl { get; set; }
        public required string CheckoutStepTwoUrl { get; set; }
        public required string CheckoutCompleteUrl { get; set; }
        public required Dictionary<string, UserCredentials> Users { get; set; }
    }
}