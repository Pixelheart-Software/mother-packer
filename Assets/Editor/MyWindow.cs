using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class WizardCreatePackage : ScriptableWizard
{

    public string companyName;
    public string packageName;
    
    [MenuItem("MotherPacker/Create Package")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<WizardCreatePackage>("Create Package", "Create");
    }

    void OnWizardCreate()
    {
      
        var dataPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);

        var packagesPath = dataPath + "Packages";

        var packageDirName = "com." + this.companyName.ToLower() + "." + this.packageName.ToLower();
        
        string packagePath = packagesPath + Path.DirectorySeparatorChar + packageDirName;
        
        Directory.CreateDirectory(packagePath);

        var writer = File.CreateText(packagePath + Path.DirectorySeparatorChar + "package.json");
        writer.Write(PACKAGE_JSON, this.companyName.ToLower(), this.packageName.ToLower());
        writer.Flush();
        writer.Close();
        
        Directory.CreateDirectory(packagePath + Path.DirectorySeparatorChar + "Runtime");
        Directory.CreateDirectory(packagePath + Path.DirectorySeparatorChar + "Runtime" + Path.DirectorySeparatorChar + "Scripts");
        
        writer = File.CreateText(packagePath + Path.DirectorySeparatorChar + "Runtime" + Path.DirectorySeparatorChar + "Test.asmdef");
        writer.Write(ASMDEF, this.companyName.ToLower(), this.packageName.ToLower());
        writer.Flush();
        writer.Close();

        var gitIgnoreFileName = packagesPath + Path.DirectorySeparatorChar + ".gitignore";
        if (!File.Exists(gitIgnoreFileName))
        {
            File.Create(gitIgnoreFileName);
        }

        using (FileStream gitIgnoreStream = new FileStream(gitIgnoreFileName, FileMode.Append, FileAccess.Write))
        using (var gitIgnoreWriter = new StreamWriter(gitIgnoreStream))
        {
            gitIgnoreWriter.WriteLine(packageDirName);
            gitIgnoreWriter.Flush();
        }
    }

    void OnWizardUpdate()
    {
        isValid = false;
        if (companyName == null || companyName.Trim().Equals(""))
        {
            errorString = "Please fill in company name";
            return;
        }
        if (packageName == null || packageName.Trim().Equals(""))
        {
            errorString = "Please fill in package name";
            return;
        }

        errorString = null;
        isValid = true;
    }


    private static string PACKAGE_JSON = @"{{
  ""name"": ""com.{0}.{1}"",
  ""version"": ""0.0.2"",
  ""displayName"": ""{1}"",
  ""description"": ""This is an example package"",
  ""unity"": ""2020.3"",
  ""unityRelease"": ""32f1"",
  ""documentationUrl"": ""https://example.com/"",
  ""changelogUrl"": ""https://example.com/changelog.html"",
  ""licensesUrl"": ""https://example.com/licensing.html"",
  ""author"": {{
    ""name"": ""Piotr Kaczmarski"",
    ""email"": ""peter@pixelheartsoftware.com"",
    ""url"": ""https://pixelheartsoftware.com""
  }},
  ""publishConfig"": {{
	""registry"": ""http://localhost:4873""
  }}
}}";

    private static string ASMDEF = @"{{
  ""name"": ""{0}.{1}"",
  ""includePlatforms"": [],
  ""excludePlatforms"": [],
  ""allowUnsafeCode"": false,
  ""overrideReferences"": false,
  ""precompiledReferences"": [],
  ""autoReferenced"": true,
  ""defineConstraints"": [],
  ""noEngineReferences"": false
}}";

}