using GodsEye.API.Features.NotificationGroup;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Response;
using Microsoft.Extensions.Logging;
using Moq;

namespace GodsEye.Tests.Handlers
{
    private readonly Mock<IDapperContext> _contextMock = new();
    private readonly Mock<ILogger<CreateNotificationGroupHandler>> _loggerMock = new();

    [Fact]
    public async Task QuandoProcedureRetornaId_DeveRetornarIdCorreto()
    {
        // Arrange
        _contextMock
            .Setup(x => x.QuerySingleSqlAsync<ProcedureResponse>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcedureResponse { Id = 42 });

        var handler = new CreateNotificationGroupHandler(_contextMock.Object, _loggerMock.Object);
        var command = new CreateNotificationGroupCommand("Grupo", ["email@test.com"]);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task QuandoProcedureRetornaNull_DeveLancarInvalidOperationException()
    {
        // Arrange
        _contextMock
            .Setup(x => x.QuerySingleSqlAsync<ProcedureResponse>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProcedureResponse?)null);

        var handler = new CreateNotificationGroupHandler(_contextMock.Object, _loggerMock.Object);
        var command = new CreateNotificationGroupCommand("Grupo", ["email@test.com"]);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
