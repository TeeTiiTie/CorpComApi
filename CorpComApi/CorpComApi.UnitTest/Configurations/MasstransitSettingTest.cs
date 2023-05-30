using Microsoft.Extensions.Configuration;
using CorpComApi.Configurations;

namespace CorpComApi.UnitTest.Configurations
{
    public class MasstransitSettingTest
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        /// <summary>
        /// Test MasstransitSetting on RabbitMQSetting
        /// </summary>
        [Test]
        public void RabbitMQSetting_TryPassingConfiguration()
        {
            // Arrange
            var json = _configuration.GetSection("Masstransit");
            Assert.That(json, Is.Not.Null, "Masstransit is not setted in appsetting.json");

            // Act
            var masstransitSetting = json.Get<MasstransitSetting>();

            // Assert
            // Setting must not be null
            Assert.That(masstransitSetting, Is.Not.Null);

            // Warning when RabbitMQ is disabled
            if (!masstransitSetting.EnableRabbitMQ)
            {
                Assert.Warn("RabbitMQ is disabled");
                return;
            }

            var rabbitMQSetting = masstransitSetting.RabbitMQSetting;

            // RabbitMQSetting must not be null
            Assert.That(rabbitMQSetting, Is.Not.Null);

            // RabbitMQSetting.Host must not be null or empty
            Assert.That(rabbitMQSetting.Host, Is.Not.Null);
            Assert.That(rabbitMQSetting.Host, Is.Not.Empty);

            // RabbitMQSetting.Port must not be empty and greater than 0
            Assert.That(rabbitMQSetting.Port, Is.Not.Empty);
            Assert.That(rabbitMQSetting.Port, Is.GreaterThan(0));

            // RabbitMQSetting.Username must not be null or empty
            Assert.That(rabbitMQSetting.Username, Is.Not.Null);
            Assert.That(rabbitMQSetting.Username, Is.Not.Empty);

            // RabbitMQSetting.Password must not be null or empty
            Assert.That(rabbitMQSetting.Password, Is.Not.Null);
            Assert.That(rabbitMQSetting.Password, Is.Not.Empty);

            // RabbitMQSetting.Vhost must not be null or empty
            Assert.That(rabbitMQSetting.Vhost, Is.Not.Null);
            Assert.That(rabbitMQSetting.Vhost, Is.Not.Empty);
        }

        /// <summary>
        /// Test MasstransitSetting on KafkaSetting
        /// </summary>
        [Test]
        public void KafkaSetting_TryPassingConfiguration()
        {
            // Arrange
            var json = _configuration.GetSection("Masstransit");
            if (json is null) Assert.Fail("Masstransit is not setted in appsetting.json");

            // Act
            var masstransitSetting = json.Get<MasstransitSetting>();

            // Assert

            // Setting must not be null
            Assert.That(masstransitSetting, Is.Not.Null);

            // Warning when Kafka is disabled
            if (!masstransitSetting.EnableKafka)
            {
                Assert.Warn("Kafka is disabled");
                return;
            }

            var kafkaSetting = masstransitSetting.KafkaSetting;

            // KafkaSetting must not be null
            Assert.That(kafkaSetting, Is.Not.Null);

            // KafkaSetting.Host must not be null or empty
            Assert.That(kafkaSetting.Host, Is.Not.Null);
            Assert.That(kafkaSetting.Host, Is.Not.Empty);

            // KafkaSetting.Port must not be empty and greater than 0
            Assert.That(kafkaSetting.Port, Is.Not.Empty);
            Assert.That(kafkaSetting.Port, Is.GreaterThan(0));
        }
    }
}