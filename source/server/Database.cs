
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class Database {
	
	private static void EnsureDatabase () {
		
		if (!Directory.Exists("database")) Directory.CreateDirectory("database");
		
		if (!Directory.Exists("database/entries")) Directory.CreateDirectory("database/entries");
		
		if (!Directory.Exists("database/tag")) Directory.CreateDirectory("database/tag");
	}

	public static void WriteEntry (string name, string tags, string content) {
		
		EnsureDatabase();
		
		//clear old version if it exists
		if (File.Exists("database/entries/" + name)) File.Delete("database/entries/" + name);
		
		//write all new info into entry
		File.WriteAllText("database/entries/" + name, content);
		
		//write tags
		foreach (string tag in tags.Split(',')) {
			
			if (tag == "") continue;
			
			if (File.Exists("database/tag/" + tag)) { 
				
				File.AppendAllText("database/tag/" + tag, Environment.NewLine + name);
			
			} else {
				
				File.WriteAllText("database/tag/" + tag, name);
			}
		}
	}
	
	public static string[] ReadEntry (string name) {
		
		EnsureDatabase();
		
		//Check the Existance, read & return, either nothing, or the correct value, in a single line.. because screw oderliness I suppose..
		return File.Exists("database/entries/" + name) ? File.ReadAllLines("database/entries/" + name) : new string[] { "404" };
	}
	
	public static string[] Search (string query) {
		
		EnsureDatabase();
		
		List<string> results = new List<string>();
		
		//create list of search terms
		string[] terms = query.Split(' ');
		
		//search entries
		foreach (string file in Directory.GetFiles("database/entries/", "*")) {
			
			string[] parts = file.Split('/');
			string entryName = parts[parts.Length - 1];
			foreach (string term in terms)
				if (entryName.Contains(term))
					results.Add(entryName);
		}
		
		//search tags
		foreach (string file in Directory.GetFiles("database/tag/", "*")) {
			
			string[] parts = file.Split('/');
			string tag = parts[parts.Length - 1];
			
			bool c = false;
			
			foreach (string term in terms)
				if (tag.Contains(term)) c = true;
				
			if (c)			
				foreach (string line in File.ReadAllLines(file)) {
					
					results.Add(line);
				}
		}
		
		return results.ToArray();
	}
}
