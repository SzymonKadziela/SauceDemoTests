using System.Collections.Generic;

namespace SauceDemoTests.Configuration
{
    public class TestDataConfig
    {
        public required Dictionary<string, UserCredentials> Users { get; set; }
    }
}