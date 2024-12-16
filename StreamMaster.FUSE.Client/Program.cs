using System.Net.Sockets;
using System.Text;

namespace StreamMaster.FUSE.Client
{
    internal class Program
    {
        // TCP server endpoint
        private const string ServerIp = "127.0.0.1";

        private const int ServerPort = 9999;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Connecting to filesystem server at {0}:{1}...", ServerIp, ServerPort);
            // We'll connect once at start and reuse the connection in a loop.
            // For each command, we reconnect to keep logic simple. Alternatively, we can keep one connection open.
            // But if the server is designed for one command per connection, we can handle that.

            Console.WriteLine("Type 'exit' to quit.");
            Console.WriteLine("Commands example:");
            Console.WriteLine("  GETATTR /");
            Console.WriteLine("  READDIR /");
            Console.WriteLine("  MKDIR /testdir");
            Console.WriteLine("  RMDIR /testdir");
            Console.WriteLine("  CREATE /myfile");
            Console.WriteLine("  UNLINK /myfile");
            Console.WriteLine("  READ /myfile 0 10");
            Console.WriteLine("  WRITE /myfile 0 5");

            while (true)
            {
                Console.Write("cmd> ");
                string? line = Console.ReadLine();
                if (line == null || string.Equals(line.Trim(), "exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                {
                    continue;
                }

                string cmd = parts[0].ToUpperInvariant();
                // For commands requiring data, handle accordingly

                // Connect for each command
                using TcpClient client = new();
                try
                {
                    await client.ConnectAsync(ServerIp, ServerPort).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error connecting: " + ex.Message);
                    continue;
                }

                await using NetworkStream ns = client.GetStream();
                await using BinaryWriter writer = new(ns, Encoding.UTF8, leaveOpen: true);
                using StreamReader reader = new(ns, Encoding.UTF8, false, 1024, leaveOpen: true);

                // Special handling for WRITE since we must send data after the initial line
                if (cmd == "WRITE" && parts.Length == 4)
                {
                    // WRITE path offset size
                    // After sending the line, we need to read size bytes from the user and send them
                    if (!long.TryParse(parts[2], out long woffset) || !int.TryParse(parts[3], out int wsize))
                    {
                        Console.WriteLine("Invalid WRITE arguments.");
                        continue;
                    }

                    // Send command line
                    string requestLine = $"{cmd} {parts[1]} {woffset} {wsize}\n";
                    byte[] reqData = Encoding.UTF8.GetBytes(requestLine);
                    await ns.WriteAsync(reqData).ConfigureAwait(false);
                    await ns.FlushAsync().ConfigureAwait(false);

                    Console.WriteLine($"Enter {wsize} bytes of data (exactly) for WRITE:");
                    // We'll read a line from user, must ensure length matches wsize
                    // If user provides shorter line, we pad with zeros. If longer, we truncate.
                    string? inputData = Console.ReadLine();
                    inputData ??= "";
                    byte[] writeBuf = Encoding.UTF8.GetBytes(inputData);
                    if (writeBuf.Length > wsize)
                    {
                        // truncate
                        Array.Resize(ref writeBuf, wsize);
                    }
                    else if (writeBuf.Length < wsize)
                    {
                        // pad with zeros
                        Array.Resize(ref writeBuf, wsize);
                    }

                    await ns.WriteAsync(writeBuf).ConfigureAwait(false);
                    await ns.FlushAsync().ConfigureAwait(false);

                    // now read response
                    string? response = await reader.ReadLineAsync().ConfigureAwait(false);
                    if (response != null)
                    {
                        Console.WriteLine(response);
                    }
                }
                else
                {
                    // For other commands, just send the line
                    string requestLine = line + "\n";
                    byte[] reqData = Encoding.UTF8.GetBytes(requestLine);
                    await ns.WriteAsync(reqData).ConfigureAwait(false);
                    await ns.FlushAsync().ConfigureAwait(false);

                    // For READ, we must handle response carefully
                    // For others, just read lines until done
                    if (cmd == "READ" && parts.Length == 4)
                    {
                        // READ path offset size
                        // Expecting "OK <bytes>\n" or "ERR ..."
                        string? response = await reader.ReadLineAsync().ConfigureAwait(false);
                        if (response == null)
                        {
                            Console.WriteLine("No response from server");
                            continue;
                        }
                        Console.WriteLine(response);
                        if (response.StartsWith("OK "))
                        {
                            // Extract number of bytes
                            string[] rparts = response.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (rparts.Length == 2 && int.TryParse(rparts[1], out int count))
                            {
                                if (count > 0)
                                {
                                    // read `count` bytes
                                    byte[] readBuf = new byte[count];
                                    int total = 0;
                                    while (total < count)
                                    {
                                        int rr = await ns.ReadAsync(readBuf.AsMemory(total, count - total)).ConfigureAwait(false);
                                        if (rr <= 0)
                                        {
                                            break;
                                        }

                                        total += rr;
                                    }
                                    string content = Encoding.UTF8.GetString(readBuf, 0, total);
                                    Console.WriteLine("Content:");
                                    Console.WriteLine(content);
                                }
                            }
                        }
                    }
                    else
                    {
                        // For other commands: GETATTR, READDIR, MKDIR, RMDIR, CREATE, UNLINK
                        // The server either sends multiple lines (READDIR) or one line
                        // READDIR returns first line: "OK count" then 'count' lines
                        // We'll just read until no more data or a short delay
                        // A simple approach: first read one line of response

                        string? firstLine = await reader.ReadLineAsync().ConfigureAwait(false);
                        if (firstLine == null)
                        {
                            Console.WriteLine("No response from server");
                            continue;
                        }
                        Console.WriteLine(firstLine);

                        if (cmd == "READDIR" && firstLine.StartsWith("OK "))
                        {
                            // parse count
                            string[] fparts = firstLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (fparts.Length == 2 && int.TryParse(fparts[1], out int count))
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    string? entry = await reader.ReadLineAsync().ConfigureAwait(false);
                                    if (entry == null)
                                    {
                                        break;
                                    }

                                    Console.WriteLine(entry);
                                }
                            }
                        }
                    }
                }

                // Close and loop for next command
            }

            Console.WriteLine("Exiting.");
        }
    }
}