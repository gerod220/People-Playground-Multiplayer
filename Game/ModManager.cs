using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiplayer.Game
{
    public class ModManager
    {
		private static List<ModEntryBehaviour> _modEntrys;
		public static List<ModEntryBehaviour> ModEntrys
		{
			get
			{
				_modEntrys = new List<ModEntryBehaviour>();
				ModListBehaviour ModList = UnityEngine.Object.FindObjectOfType<ModListBehaviour>();
				foreach (object obj in ModList.Container)
				{
					Transform transform = (Transform)obj;
					if (transform.GetComponent<ModEntryBehaviour>())
					{
						_modEntrys.Add(transform.GetComponent<ModEntryBehaviour>());
					}
				}
				return _modEntrys;
			}
		}

		public static void InvokeToAllMods(string method)
        {
			foreach (ModMetaData mod in ModLoader.LoadedMods)
			{
				Invoke(mod, method, new object[] { });
			}
		}

		public static object Invoke(ModMetaData mod, string method, object[] _params)
		{
			ModScript modScript;
			ModLoader.ModScripts.TryGetValue(mod, out modScript);
			MethodInfo methodInfo = modScript.LoadedAssembly.GetType(mod.EntryPoint).GetMethod(method);
			try
			{
				return methodInfo.Invoke(null, (_params != null ? _params : new object[0]));
			}
			catch { }
			return new object[] { };
		}
	}
}
