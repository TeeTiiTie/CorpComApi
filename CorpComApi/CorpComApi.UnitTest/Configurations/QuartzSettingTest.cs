using Microsoft.Extensions.Configuration;
using CorpComApi.Configurations;
using Quartz;
using System.Text.RegularExpressions;

namespace CorpComApi.UnitTest.Configurations
{
    public class QuartzSettingTest
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
            var json = _configuration.GetSection("Quartz");
            Assert.That(json, Is.Not.Null, "Quartz is not setted in appsetting.json");

            // Act
            var quartzSetting = json.Get<QuartzSetting>();

            // Assert
            Assert.That(quartzSetting, Is.Not.Null);

            // Warning when Quartz is disabled
            if (!quartzSetting.EnableQuartz)
            {
                Assert.Warn("Quartz is disabled");
                return;
            }

            // Warning when Quartz is enabled, but has no jobs
            Assert.That(quartzSetting.Jobs.Count, Is.Not.Zero, "Quartz is enable, but has no jobs");

            // Assert each job schedule is valid
            foreach (var job in quartzSetting.Jobs)
            {
                Assert.That(IsValidSchedule(job.Value), Is.True, $"Schedule for job {job.Key} is invalid");
            }
        }

        private static bool IsValidSchedule(string schedule)
        {
            var valid = CronExpression.IsValidExpression(schedule);
            // Some expressions are parsed as valid by the above method but they are not valid, like "* * * ? * *&54".
            //In order to avoid such invalid expressions an additional check is required, that is done using the below regex.

            var regex = @"^\s*($|#|\w+\s*=|(\?|\*|(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?(?:,(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?)*)\s+(\?|\*|(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?(?:,(?:[0-5]?\d)(?:(?:-|\/|\,)(?:[0-5]?\d))?)*)\s+(\?|\*|(?:[01]?\d|2[0-3])(?:(?:-|\/|\,)(?:[01]?\d|2[0-3]))?(?:,(?:[01]?\d|2[0-3])(?:(?:-|\/|\,)(?:[01]?\d|2[0-3]))?)*)\s+(\?|\*|(?:0?[1-9]|[12]\d|3[01])(?:(?:-|\/|\,)(?:0?[1-9]|[12]\d|3[01]))?(?:,(?:0?[1-9]|[12]\d|3[01])(?:(?:-|\/|\,)(?:0?[1-9]|[12]\d|3[01]))?)*)\s+(\?|\*|(?:[1-9]|1[012])(?:(?:-|\/|\,)(?:[1-9]|1[012]))?(?:L|W)?(?:,(?:[1-9]|1[012])(?:(?:-|\/|\,)(?:[1-9]|1[012]))?(?:L|W)?)*|\?|\*|(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)(?:(?:-)(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC))?(?:,(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)(?:(?:-)(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC))?)*)\s+(\?|\*|(?:[0-6])(?:(?:-|\/|\,|#)(?:[0-6]))?(?:L)?(?:,(?:[0-6])(?:(?:-|\/|\,|#)(?:[0-6]))?(?:L)?)*|\?|\*|(?:MON|TUE|WED|THU|FRI|SAT|SUN)(?:(?:-)(?:MON|TUE|WED|THU|FRI|SAT|SUN))?(?:,(?:MON|TUE|WED|THU|FRI|SAT|SUN)(?:(?:-)(?:MON|TUE|WED|THU|FRI|SAT|SUN))?)*)(|\s)+(\?|\*|(?:|\d{4})(?:(?:-|\/|\,)(?:|\d{4}))?(?:,(?:|\d{4})(?:(?:-|\/|\,)(?:|\d{4}))?)*))$";

            return valid && Regex.IsMatch(schedule, regex);
        }
    }
}