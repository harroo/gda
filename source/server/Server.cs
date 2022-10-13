
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using System.IO;

public static class Server {

    private static TcpListener listener;

    public static void Start () {

        Console.WriteLine("Loading password from passkey.txt..");

        if (!File.Exists("passkey.txt"))
            Console.WriteLine("passkey.txt not found! default password will be used. This is very unsecure! Please don't do this!");

        Console.WriteLine("Starting Server..");

        listener = new TcpListener(IPAddress.Any, 1122);

        try {

            listener.Start();

            Console.WriteLine("Server started successfully!");

            Loop();

        } catch (Exception ex) {

            Console.WriteLine("Failed to start Server!");
            Console.WriteLine(ex.Message);
            Environment.Exit(-1);
        }
    }

    private static void Loop () {

        while (true) {

            try {

				TcpClient client = listener.AcceptTcpClient();
				NetworkStream stream = client.GetStream();

				Console.WriteLine("Client received! " + client.Client.RemoteEndPoint.ToString());

                byte[] recvBuffer = new byte[4];
                stream.Read(recvBuffer, 0, 4);
                recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
                stream.Read(recvBuffer, 0, recvBuffer.Length);

                if (!CheckPasskey(Encoding.Unicode.GetString(recvBuffer))) {

                    client.Close();
                    stream.Close();
                    continue;
                }

                recvBuffer = new byte[1];
                stream.Read(recvBuffer, 0, 1);

                switch (recvBuffer[0]) {

                    case 0: DatabaseAppend(stream); break;
                    case 1: DatabaseFetch(stream); break;
                    
                    case 2: DatabaseSearch(stream); break;
                }

                client.Close();
                stream.Close();

            } catch (Exception ex) {

                Console.WriteLine("Server.Loop() Error:");
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static bool CheckPasskey (string hashToCheck) {

        string input = File.Exists("passkey.txt") ? File.ReadAllLines("passkey.txt")[0] : "acflin";

        HashAlgorithm algorithm = SHA256.Create();
        byte[] hashedData = algorithm.ComputeHash(Encoding.Unicode.GetBytes(input));

        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in hashedData)
            stringBuilder.Append(b.ToString("X2"));

        return stringBuilder.ToString() == hashToCheck;
    }
    
    private static void DatabaseAppend (NetworkStream stream) {
		
		//recv useragent
		byte[] recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);
		string useragent = Encoding.Unicode.GetString(recvBuffer);

		//recv authentication
		recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);

		if (!CheckPasskey(Encoding.Unicode.GetString(recvBuffer))) return;
		
		
		//recv entry name
		recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);
		string entryName = Encoding.Unicode.GetString(recvBuffer);
		
		//recv entry tags (seperated by ',')
		recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);
		string entryTags = Encoding.Unicode.GetString(recvBuffer);
		
		//recv entry content
		recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);
		string entryContent = Encoding.Unicode.GetString(recvBuffer);
		
		Database.WriteEntry(entryName, entryTags, entryContent);
		
		Console.WriteLine(useragent + ": wrote entry: '" + entryName + "', size of: " + entryContent.Length);
	}
    
    private static void DatabaseFetch (NetworkStream stream) {
		
		//recv useragent
		byte[] recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);
		string useragent = Encoding.Unicode.GetString(recvBuffer);

		//recv authentication
		recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);

		if (!CheckPasskey(Encoding.Unicode.GetString(recvBuffer))) return;
		
		
		//recv entry name
		recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);
		string entryName = Encoding.Unicode.GetString(recvBuffer);
		
		//get entry content
		string[] content = Database.ReadEntry(entryName);
		
		//send content to client
		recvBuffer = new byte[1];
		stream.Write(BitConverter.GetBytes(content.Length));
		foreach (string line in content) {
			
			byte[] sendBuffer = Encoding.Unicode.GetBytes(line);
			stream.Write(BitConverter.GetBytes(sendBuffer.Length));
			stream.Write(sendBuffer, 0, sendBuffer.Length);
			
			stream.Read(recvBuffer, 0, 1); //recv confirmation
		}
		
		Console.WriteLine(useragent + ": was sent entry: '" + entryName + "', size of: " + content.Length);
	}
    
    private static void DatabaseSearch (NetworkStream stream) {
		
		//recv useragent
		byte[] recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);
		string useragent = Encoding.Unicode.GetString(recvBuffer);

		//recv authentication
		recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);

		if (!CheckPasskey(Encoding.Unicode.GetString(recvBuffer))) return;
		
		
		//recv query
		recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
		stream.Read(recvBuffer, 0, recvBuffer.Length);
		string query = Encoding.Unicode.GetString(recvBuffer);
		
		//get matches
		string[] matches = Database.Search(query);
		
		//send matches to client
		recvBuffer = new byte[1];
		stream.Write(BitConverter.GetBytes(matches.Length));
		foreach (string match in matches) {
			
			byte[] sendBuffer = Encoding.Unicode.GetBytes(match);
			stream.Write(BitConverter.GetBytes(sendBuffer.Length));
			stream.Write(sendBuffer, 0, sendBuffer.Length);
			
			stream.Read(recvBuffer, 0, 1); //recv confirmation
		}
		
		Console.WriteLine(useragent + ": searched: '" + query + "', amount of results: " + matches.Length);
	}
}
