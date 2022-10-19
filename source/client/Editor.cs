
using System;
using System.Text;

public static class Editor {
	
	public static string Edit (string input) {
		
		Console.WriteLine("welcome to the gdacli Editor!");		
		Console.WriteLine("Press Escape to finish & exit. :)");
		Console.WriteLine();
		Console.WriteLine("Press Enter to begin..");
		Console.ReadKey();
		Console.Clear();
		
		string content = input;
		
		Console.Write(content);
		
		while (true) {
			
			ConsoleKeyInfo cki = Console.ReadKey(true);
			
			if (cki.Key == ConsoleKey.Backspace && content.Length > 0) {
				
				content = content.Remove(content.Length - 1);
				Console.Write('\b');
				
			} else if (cki.Key == ConsoleKey.Enter) {
				
				content += "\n";
				Console.Write('\n');
			
			} else if (cki.Key == ConsoleKey.Escape) {
				
				Console.Clear();
				Console.WriteLine("Exit Editor.");
				return content;
			
			} else {
				
				content += cki.KeyChar.ToString();
				Console.Write(cki.KeyChar.ToString());
			}
		}
	}
}
