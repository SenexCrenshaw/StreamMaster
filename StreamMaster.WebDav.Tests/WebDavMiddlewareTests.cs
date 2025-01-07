using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using StreamMaster.WebDav.Core;
using StreamMaster.WebDav.Domain.Interfaces;
using StreamMaster.WebDav.Domain.Models;

namespace StreamMaster.WebDav.Tests;

public class WebDavMiddlewareTests
{
    private readonly Mock<IWebDavStorageProvider> _storageProviderMock;
    private readonly Mock<ILockManager> _lockManagerMock;
    private readonly WebDavMiddleware _middleware;
    private readonly ILogger<WebDavMiddleware> _logger;

    public WebDavMiddlewareTests()
    {
        _logger = new Mock<ILogger<WebDavMiddleware>>().Object;
        _storageProviderMock = new Mock<IWebDavStorageProvider>();
        _lockManagerMock = new Mock<ILockManager>();
        _middleware = new WebDavMiddleware(
            _ => { return Task.CompletedTask; },
            _storageProviderMock.Object,
        _lockManagerMock.Object,
           _logger
        );
    }

    [Fact]
    public async Task HandleCopyAsync_ShouldCopyResource_WhenSourceAndDestinationAreValid()
    {
        // Arrange
        DefaultHttpContext context = new();

        _storageProviderMock
            .Setup(p => p.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        context.Request.Method = "COPY";
        context.Request.Path = "/source.txt";
        context.Request.Headers["Destination"] = "/destination.txt";

        // Act
        await _middleware.InvokeAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status201Created, context.Response.StatusCode);

        _storageProviderMock.Verify(p =>
            p.CopyAsync("/source.txt", "/destination.txt", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleMoveAsync_ShouldMoveResource_WhenSourceAndDestinationAreValid()
    {
        // Arrange
        DefaultHttpContext context = new();

        _storageProviderMock
            .Setup(p => p.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        context.Request.Method = "MOVE";
        context.Request.Path = "/source.txt";
        context.Request.Headers["Destination"] = "/destination.txt";

        // Act
        await _middleware.InvokeAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status201Created, context.Response.StatusCode);

        _storageProviderMock.Verify(p =>
            p.MoveAsync("/source.txt", "/destination.txt", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleGetAsync_ShouldReturnFileStream_WhenFileExists()
    {
        // Arrange
        MemoryStream fileContent = new(Encoding.UTF8.GetBytes("File Content"));
        _storageProviderMock
            .Setup(p => p.GetFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileContent);

        DefaultHttpContext context = new();
        context.Request.Method = "GET";
        context.Request.Path = "/example.txt";
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.Equal("application/octet-stream", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        string responseContent = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Equal("File Content", responseContent);
    }

    [Fact]
    public async Task HandlePutAsync_ShouldSaveFile_WhenRequestBodyProvided()
    {
        // Arrange
        MemoryStream fileContent = new(Encoding.UTF8.GetBytes("New File Content"));

        _storageProviderMock
            .Setup(p => p.SaveToLocalCacheAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        DefaultHttpContext context = new();
        context.Request.Method = "PUT";
        context.Request.Path = "/newfile.txt";
        context.Request.Body = fileContent;

        // Act
        await _middleware.InvokeAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status201Created, context.Response.StatusCode);

        _storageProviderMock.Verify(p =>
            p.SaveToLocalCacheAsync("/newfile.txt", It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandlePropFindAsync_ShouldReturnMultiStatus_WhenEntriesExist()
    {
        // Arrange
        List<DirectoryEntry> entries =
        [
        new DirectoryEntry { Name = "File1.txt", Path = "/File1.txt", IsDirectory = false },
        new DirectoryEntry { Name = "Dir1", Path = "/Dir1", IsDirectory = true }
    ];

        _storageProviderMock
            .Setup(p => p.ListDirectoryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(entries.ToAsyncEnumerable());

        DefaultHttpContext context = new();
        context.Request.Method = "PROPFIND";
        context.Request.Path = "/example";
        context.Request.Headers["Depth"] = "1";

        // Act
        await _middleware.InvokeAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status207MultiStatus, context.Response.StatusCode);
        Assert.Equal("application/xml", context.Response.ContentType);
    }

    [Fact]
    public async Task HandleMkColAsync_ShouldCreateDirectory_WhenPathDoesNotExist()
    {
        // Arrange
        _storageProviderMock
            .Setup(p => p.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _storageProviderMock
            .Setup(p => p.CreateDirectoryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        DefaultHttpContext context = new();
        context.Request.Method = "MKCOL";
        context.Request.Path = "/new-directory";

        // Act
        await _middleware.InvokeAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status201Created, context.Response.StatusCode);

        _storageProviderMock.Verify(p =>
            p.CreateDirectoryAsync("/new-directory", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleDeleteAsync_ShouldDeleteResource_WhenItExists()
    {
        // Arrange
        _storageProviderMock
            .Setup(p => p.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _storageProviderMock
            .Setup(p => p.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        DefaultHttpContext context = new();
        context.Request.Method = "DELETE";
        context.Request.Path = "/file-to-delete.txt";

        // Act
        await _middleware.InvokeAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);

        _storageProviderMock.Verify(p =>
            p.DeleteAsync("/file-to-delete.txt", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleLockAsync_ShouldReturnLockToken_WhenLockIsAcquired()
    {
        // Arrange
        _lockManagerMock
            .Setup(p => p.AcquireLockAsync(It.IsAny<string>(), It.IsAny<string>(), true, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("lock-token");

        DefaultHttpContext context = new();
        context.Request.Method = "LOCK";
        context.Request.Path = "/locked-resource.txt";
        context.Request.Headers["Owner"] = "test-owner";
        context.Response.Body = new MemoryStream();

        // Act
        await _middleware.InvokeAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.Equal("application/xml", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        string responseContent = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("<D:locktoken>", responseContent);
        Assert.Contains("lock-token", responseContent);

        _lockManagerMock.Verify(p =>
            p.AcquireLockAsync("/locked-resource.txt", "test-owner", true, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleUnlockAsync_ShouldReleaseLock_WhenLockTokenIsValid()
    {
        // Arrange
        DefaultHttpContext context = new();

        _lockManagerMock
            .Setup(p => p.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        context.Request.Method = "UNLOCK";
        context.Request.Path = "/locked-resource.txt";
        context.Request.Headers["Lock-Token"] = "lock-token";

        // Act
        await _middleware.InvokeAsync(context, CancellationToken.None);

        // Assert
        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);

        _lockManagerMock.Verify(p =>
            p.ReleaseLockAsync("lock-token", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (T? item in source)
        {
            yield return item;
            await Task.Yield();
        }
    }
}