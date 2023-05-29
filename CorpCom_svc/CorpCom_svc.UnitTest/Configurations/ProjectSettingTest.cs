using Microsoft.Extensions.Configuration;
using Net60_ApiTemplate_2023.Configurations;
using Newtonsoft.Json;

namespace CorpCom_svc.UnitTest.Configurations
{
    /// <summary>
    /// Test project for <see cref="ProjectSetting"/> configuration
    /// </summary>
    public class ProjectSettingTest
    {
        private IConfiguration _configuration;

        /// <summary>
        /// Get configuration from appsetting.json
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        /// <summary>
        /// Test passing configuration
        /// </summary>
        [Test]
        public void TryPassingConfiguration()
        {
            // Arrange
            Console.WriteLine("Get \"Project\" section from appsetting.json");
            var project = _configuration.GetSection("Project");

            Assert.IsNotNull(project, "Project is not setted in appsetting.json");

            // Act
            var projectSetting = project.Get<ProjectSetting>();
            Console.WriteLine($"Configuration : {JsonConvert.SerializeObject(projectSetting)}");

            // Assert
            Assert.IsNotNull(projectSetting);
        }
    }
}
