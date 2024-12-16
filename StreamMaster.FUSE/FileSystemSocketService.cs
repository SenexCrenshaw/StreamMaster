using System.Net;
using System.Net.Sockets;
using System.Text;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StreamMaster.FUSE;

public sealed class FileSystemSocketService(IFileSystemManager fsManager, ILogger<FileSystemSocketService> logger) : BackgroundService
{
    private readonly IPAddress ip = IPAddress.Any;
    private readonly int port = 9999;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TcpListener listener = new(ip, port);
        listener.Start();
        logger.LogInformation("Filesystem server listening on {ip}:{port}", ip, port);

        while (!stoppingToken.IsCancellationRequested)
        {
            TcpClient client;
            try
            {
                client = await listener.AcceptTcpClientAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            _ = Task.Run(() => HandleClientAsync(client, stoppingToken), stoppingToken);
        }

        listener.Stop();
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        await using NetworkStream ns = client.GetStream();
        try
        {
            using StreamReader reader = new(ns, Encoding.UTF8, false, 1024, leaveOpen: true);
            await using BinaryWriter writer = new(ns, Encoding.UTF8, leaveOpen: true);

            string? requestLine = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (requestLine == null)
            {
                return;
            }

            string[] parts = requestLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                return;
            }

            string cmd = parts[0].ToUpperInvariant();
            string path = parts[1];

            switch (cmd)
            {
                case "GETATTR":
                    if (fsManager.GetAttr(path, out bool isDir, out long size))
                    {
                        await WriteAsync(writer, $"OK {(isDir ? "dir" : "file")} {size}\n", cancellationToken);
                    }
                    else
                    {
                        await WriteAsync(writer, "ERR ENOENT\n", cancellationToken);
                    }

                    break;

                case "READDIR":
                    {
                        IFileSystemNode[] entries = [.. fsManager.ReadDir(path)];
                        await WriteAsync(writer, $"OK {entries.Length}\n", cancellationToken);
                        foreach (IFileSystemNode? e in entries)
                        {
                            await WriteAsync(writer, $"{e.Name} {(e.IsDirectory ? "dir" : "file")}\n", cancellationToken);
                        }
                    }
                    break;

                case "MKDIR":
                    {
                        bool ok = fsManager.Mkdir(path);
                        await WriteAsync(writer, ok ? "OK\n" : "ERR EIO\n", cancellationToken);
                    }
                    break;

                case "RMDIR":
                    {
                        bool ok = fsManager.Rmdir(path);
                        await WriteAsync(writer, ok ? "OK\n" : "ERR EIO\n", cancellationToken);
                    }
                    break;

                case "CREATE":
                    {
                        bool ok = fsManager.CreateFile(path);
                        await WriteAsync(writer, ok ? "OK\n" : "ERR EIO\n", cancellationToken);
                    }
                    break;

                case "UNLINK":
                    {
                        bool ok = fsManager.Unlink(path);
                        await WriteAsync(writer, ok ? "OK\n" : "ERR EIO\n", cancellationToken);
                    }
                    break;

                case "READ":
                    {
                        if (parts.Length < 4 || !long.TryParse(parts[2], out long roffset) || !int.TryParse(parts[3], out int rsize))
                        { await WriteAsync(writer, "ERR EINVAL\n", cancellationToken); break; }
                        byte[] rbuf = new byte[rsize];
                        int readBytes = fsManager.ReadFile(path, roffset, rsize, rbuf);
                        if (readBytes < 0)
                        {
                            await WriteAsync(writer, "ERR ENOENT\n", cancellationToken);
                        }
                        else
                        {
                            await WriteAsync(writer, $"OK {readBytes}\n", cancellationToken);
                            if (readBytes > 0)
                            {
                                await ns.WriteAsync(rbuf.AsMemory(0, readBytes), cancellationToken);
                            }
                        }
                    }
                    break;

                case "WRITE":
                    {
                        if (parts.Length < 4 || !long.TryParse(parts[2], out long woffset) || !int.TryParse(parts[3], out int wsize))
                        { await WriteAsync(writer, "ERR EINVAL\n", cancellationToken); break; }
                        byte[] wbuf = new byte[wsize];
                        int total = 0;
                        while (total < wsize)
                        {
                            int rr = await ns.ReadAsync(wbuf.AsMemory(total, wsize - total), cancellationToken);
                            if (rr <= 0)
                            {
                                break;
                            }

                            total += rr;
                        }
                        if (total < wsize) { await WriteAsync(writer, "ERR EIO\n", cancellationToken); break; }
                        bool ok = fsManager.WriteFile(path, woffset, wbuf);
                        await WriteAsync(writer, ok ? "OK\n" : "ERR EIO\n", cancellationToken);
                    }
                    break;

                default:
                    await WriteAsync(writer, "ERR ENOSYS\n", cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Client handling error");
        }
    }

    private static async Task WriteAsync(BinaryWriter writer, string line, CancellationToken cancellationToken)
    {
        byte[] data = Encoding.UTF8.GetBytes(line);
        await writer.BaseStream.WriteAsync(data, cancellationToken);
    }
}