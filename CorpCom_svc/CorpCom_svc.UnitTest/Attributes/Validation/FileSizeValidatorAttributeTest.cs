using Microsoft.AspNetCore.Http;
using Moq;
using CorpCom_svc.Attributes;

namespace CorpCom_svc.UnitTest.Attributes.Validation
{
    public class FileSizeValidatorAttributeTest
    {
        [TestCase(1, 200)]
        [TestCase(1, 1024)]
        [TestCase(1, 1024 * 1024)]
        public void FileSizeIsValid(int maxFileSizeMb, long fileSizeByte)
        {
            // Arrange
            var fileSizeValidatorAttribute = new FileSizeValidatorAttribute(maxFileSizeMb);
            Console.WriteLine($"Max File Size: {maxFileSizeMb * 1024 * 1024} bytes");

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(_ => _.Length).Returns(fileSizeByte);

            Console.WriteLine($"File Size: {fileSizeByte} bytes");

            // Act
            var result = fileSizeValidatorAttribute.IsValid(mockFormFile.Object);

            // Assert
            Assert.That(result, Is.True);
            Console.WriteLine($"Result: {result}");
        }

        [TestCase(1, 1024 * 1024 + 1)]
        [TestCase(1, 1024 * 1024 * 2)]
        [TestCase(1, 1024 * 1024 * 3)]
        public void FileSizeIsNotValid(int maxFileSizeMb, long fileSizeByte)
        {
            // Arrange
            var fileSizeValidatorAttribute = new FileSizeValidatorAttribute(maxFileSizeMb);
            Console.WriteLine($"Max File Size: {maxFileSizeMb * 1024 * 1024} bytes");

            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(_ => _.Length).Returns(fileSizeByte);

            Console.WriteLine($"File Size: {fileSizeByte} bytes");

            // Act
            var result = fileSizeValidatorAttribute.IsValid(mockFormFile.Object);

            // Assert
            Assert.That(result, Is.False);
            Console.WriteLine($"Result: {result}");
        }
    }
}