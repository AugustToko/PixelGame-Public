using UnityEngine;
using System.Collections;
using UnityEditor;

#if (UNITY_EDITOR) 

public static class Exporter {

	[MenuItem("Export/Export with tags and layers, Input settings")]
	public static void export()
	{
		string[] projectContent = new string[] {"Assets", "ProjectSettings/TagManager.asset","ProjectSettings/InputManager.asset","ProjectSettings/ProjectSettings.asset"};
		AssetDatabase.ExportPackage(projectContent, "Done.unitypackage",ExportPackageOptions.Interactive | ExportPackageOptions.Recurse |ExportPackageOptions.IncludeDependencies);
	}

}
#endif