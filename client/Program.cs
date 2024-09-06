// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;

Console.WriteLine("Hello, World!");

// Connect to the unix socket at /tmp/test.sock
using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
client.Connect(new UnixDomainSocketEndPoint("/tmp/test.sock"));
using var stream = new NetworkStream(client, ownsSocket: true);

while (stream.CanRead)
{
    var e = Event.Parser.ParseDelimitedFrom(stream);

    if (e.SessionStarted != null)
    {
        Console.WriteLine($"Session started: {e.SessionStarted.Name}");
    }
    else if (e.SessionEnded != null)
    {
        Console.WriteLine(
            $"Session ended: {e.SessionEnded.Name} (success: {e.SessionEnded.Success})"
        );
    }
    else
    {
        Console.WriteLine("Unknown event");
    }
}
