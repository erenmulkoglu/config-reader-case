using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigReader.Api.Documents;
using ConfigReader.Api.Events;
using ConfigReader.Api.Messaging;
using ConfigReader.Api.Repositories;
using ConfigReader.Api.Services;
using FluentAssertions;
using Moq;

namespace ConfigReader.Tests;


/// ConfigurationAdminService sınıfının silme (Delete) işlemini doğrulayan unit testlerini içeriyor
public class ConfigurationAdminServiceDeleteTests
{
    /// Kayıt bulunduğunda DeleteAsync metodunun başarılı şekilde çalıştığını doğrular

    [Fact]
    public async Task DeleteAsync_Should_Return_True_When_Document_Exists()
    {
        var id = "config-id-1";

        var existingDocument = new ConfigurationDocument
        {
            Id = id,
            Name = "SiteName",
            Type = "string",
            Value = "soty.io",
            IsActive = true,
            ApplicationName = "SERVICE-A",
            Version = 1
        };

        var repositoryMock = new Mock<IConfigurationRepository>();
        var publisherMock = new Mock<IEventPublisher>();

        repositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDocument);

        repositoryMock
            .Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new ConfigurationAdminService(
            repositoryMock.Object,
            publisherMock.Object);

        var result = await service.DeleteAsync(id);

        result.Should().BeTrue();

        repositoryMock.Verify(
            x => x.DeleteAsync(id, It.IsAny<CancellationToken>()),
            Times.Once);

        publisherMock.Verify(
            x => x.PublishConfigurationChangedAsync(
                It.Is<ConfigurationChangedEvent>(e =>
                    e.Id == id &&
                    e.Name == "SiteName" &&
                    e.ApplicationName == "SERVICE-A" &&
                    e.Operation == "Deleted"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }


    /// Silinmek istenen kayıt bulunamadığında DeleteAsync metodunun false döndürdüğünü doğrular

    [Fact]
    public async Task DeleteAsync_Should_Return_False_When_Document_Does_Not_Exist()
    {
        var id = "not-found-id";

        var repositoryMock = new Mock<IConfigurationRepository>();
        var publisherMock = new Mock<IEventPublisher>();

        repositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConfigurationDocument?)null);

        var service = new ConfigurationAdminService(
            repositoryMock.Object,
            publisherMock.Object);

        var result = await service.DeleteAsync(id);

        result.Should().BeFalse();

        repositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        publisherMock.Verify(
            x => x.PublishConfigurationChangedAsync(
                It.IsAny<ConfigurationChangedEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
