
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public static class Network {
	
	private static string address => File.ReadAllLines(gdacli.configPath)[0];
	
	private static string port => File.ReadAllLines(gdacli.configPath)[1];
	
	private static string passwd => File.ReadAllLines(gdacli.configPath)[2];
	
	public static string Fetch (string entryName) {
		
		TcpClient client = new TcpClient(address, int.Parse(port));
		NetworkStream stream = client.GetStream();
		
		//send password
		byte[] buffer = Encoding.Unicode.GetBytes(passwd);
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);
		
		//Console.WriteLine("Connected to Database..");
		
		//send action id
		stream.Write(new byte[1]{1}, 0, 1);
		
		
		//send useragent
		buffer = Encoding.Unicode.GetBytes("gdacli2022");
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);		
		
		//send entry name
		buffer = Encoding.Unicode.GetBytes(entryName);
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);
		
		//recv entry content
		byte[] recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		int lineCount = BitConverter.ToInt32(recvBuffer, 0);
		
		string recvContent = "";
		
		for (int i = 0; i < lineCount; ++i) {
			
			recvBuffer = new byte[4];
			stream.Read(recvBuffer, 0, 4);
			recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
			stream.Read(recvBuffer, 0, recvBuffer.Length);
			
			recvContent += Encoding.Unicode.GetString(recvBuffer) + "\n";
			
			Thread.Sleep(10);
		}
			
		//stream.Write(new byte[1]{69}, 0, 1);
		
		return recvContent;
	}
	
	public static void Post (string entryName, string entryTags, string entryContent) {
		
		TcpClient client = new TcpClient(address, int.Parse(port));
		NetworkStream stream = client.GetStream();
		
		//send password
		byte[] buffer = Encoding.Unicode.GetBytes(passwd);
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);
		
		//Console.WriteLine("Connected to Database..");
		
		//send action id
		stream.Write(new byte[1]{0}, 0, 1);
		
		
		//send useragent
		buffer = Encoding.Unicode.GetBytes("gdacli2022");
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);	
		
		//send entry name
		buffer = Encoding.Unicode.GetBytes(entryName);
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);
		
		//send entry tags
		buffer = Encoding.Unicode.GetBytes(entryTags);
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);
		
		//send entry content
		string[] content = entryContent.Split('\n');
		buffer = new byte[1];
		stream.Write(BitConverter.GetBytes(content.Length));
		foreach (string line in content) {
			
			byte[] sendBuffer = Encoding.Unicode.GetBytes(line);
			stream.Write(BitConverter.GetBytes(sendBuffer.Length));
			stream.Write(sendBuffer, 0, sendBuffer.Length);
		}
			
		//stream.Read(buffer, 0, 1); //recv confirmation
	}
	
	public static string[] Search (string query) {
		
		TcpClient client = new TcpClient(address, int.Parse(port));
		NetworkStream stream = client.GetStream();
		
		//send password
		byte[] buffer = Encoding.Unicode.GetBytes(passwd);
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);
		
		//Console.WriteLine("Connected to Database..");
		
		//send action id
		stream.Write(new byte[1]{2}, 0, 1);
		
		
		//send useragent
		buffer = Encoding.Unicode.GetBytes("gdacli2022");
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);		
		
		//send query
		buffer = Encoding.Unicode.GetBytes(query);
		stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
		stream.Write(buffer, 0, buffer.Length);
		
		//recv results
		byte[] recvBuffer = new byte[4];
		stream.Read(recvBuffer, 0, 4);
		int resultCount = BitConverter.ToInt32(recvBuffer, 0);
		
		string[] recvResults = new string[resultCount];
		
		for (int i = 0; i < resultCount; ++i) {
			
			recvBuffer = new byte[4];
			stream.Read(recvBuffer, 0, 4);
			recvBuffer = new byte[BitConverter.ToInt32(recvBuffer, 0)];
			stream.Read(recvBuffer, 0, recvBuffer.Length);
			
			recvResults[i] = Encoding.Unicode.GetString(recvBuffer);
			
			Thread.Sleep(10);
		}
			
		//stream.Write(new byte[1]{69}, 0, 1);
		
		return recvResults;
	}
}
