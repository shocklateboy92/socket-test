using System.Net.Sockets;
using Google.Protobuf;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// Listen on a unix socket at /tmp/test.sock
using var listener = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
listener.Bind(new UnixDomainSocketEndPoint("/tmp/test.sock"));
listener.Listen();

// Accept a connection
using var client = await listener.AcceptAsync();
using var stream = new NetworkStream(client, ownsSocket: true);

while (stream.CanRead) {
    var start = new Event() {
        SessionStarted = new SessionStarted() {
            Name = Guid.NewGuid().ToString()
        }
    };

    start.WriteDelimitedTo(stream);

    var end = new Event() {
        SessionEnded = new SessionEnded() {
            Name = start.SessionStarted.Name,
            Success = Random.Shared.Next(0, 2) == 0
        }
    };

    end.WriteDelimitedTo(stream);
}