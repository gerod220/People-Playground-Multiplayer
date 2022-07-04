using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine;

public class MPStarter
{
	static Assembly _AsmMPDLL = null;
	public static void OnLoad()
	{
		string path = $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName}\workshop\content\{SteamworksInitialiser.AppID}\[Mod_ID]\MPChecker\MultiplayerStarter\Client.dll";
		try
		{
			_AsmMPDLL = Assembly.Load(File.ReadAllBytes(@"Mods\MultiplayerStarter\Client.dll"));
		}
		catch (Exception)
		{
			_AsmMPDLL = Assembly.Load(File.ReadAllBytes(path));
			_AsmMPDLL.GetType("Mod").GetMethod("FromSteamWorkshop").Invoke(null, new object[0]);
		}
		_AsmMPDLL.GetType("Mod").GetMethod("LoadResourse").Invoke(null, new object[0]);
		Activator.CreateInstance(_AsmMPDLL.GetType("Mod"));
	}

	public static void Main()
	{
		_AsmMPDLL.GetType("Mod").GetMethod("Main").Invoke(null, new object[0]);
	}
}
