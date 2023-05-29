using Microsoft.Extensions.Configuration;
using Net60_ApiTemplate_2023.Configurations;

namespace CorpCom_svc.UnitTest.Configurations
{
    public class OAuthSettingTest
    {

        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        [Test]
        public void TryPassingConfiguration()
        {
            // Arrange
            var json = _configuration.GetSection("OAuth");
            if (json is null) Assert.Fail("OAuth is not setted in appsetting.json");

            // Act
            var oAuthSetting = json.Get<OAuthSetting>();

            // Assert

            // Setting must not be null
            Assert.IsNotNull(oAuthSetting);

            // Warning when OAuth is disabled
            if (!oAuthSetting.EnableOAuth)
            {
                Assert.Warn("OAuth is disabled");
                return;
            }
        }
    }
}
