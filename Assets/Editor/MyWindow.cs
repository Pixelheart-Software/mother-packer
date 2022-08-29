using System.Collections;
using System.Collections.Generic;
using System.IO;
using Codice.Utils;
using UnityEngine;
using UnityEditor;

public class WizardCreatePackage : ScriptableWizard
{
    [Header("Required Fields")]
    public string companyName;
    public string packageName;

    [Header("Optional Fields")]
    public bool addGitignoreFile = true;

    [TextArea] public string packageDescription;

    public string unityVersion;
    public string unityRelease;

    public string documentationUrl;
    public string changelogUrl;
    public string licensesUrl;

    [Header("Author")]
    public string authorName;
    public string authorEmail;
    public string authorUrl;

    [Header("Publishing Config")]
    public string registry;

    [Header("Assembly Definition")]
    public bool createAssemblyDefinition = true;
    public string includePlatforms;
    public string excludePlatforms;
    public bool allowUnsafeCode;
    public bool overrideReferences;
    public string precompiledReferences;
    public bool autoReferenced;
    public string defineConstraints;
    public bool noEngineReferences;
    
    

    [MenuItem("MotherPacker/Create Package")]
    static void CreateWizard()
    {
        var wizard = ScriptableWizard.DisplayWizard<WizardCreatePackage>("Create Package", "Create");

        var lastDotPos = Application.unityVersion.LastIndexOf(".");

        wizard.unityVersion = Application.unityVersion.Substring(0, lastDotPos);
        wizard.unityRelease = Application.unityVersion.Substring(lastDotPos + 1);
    }

    void OnWizardCreate()
    {
        var dataPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);

        var packagesPath = dataPath + "Packages";

        var packageDirName = "com." + this.companyName.ToLower() + "." + this.packageName.ToLower();

        string packagePath = packagesPath + Path.DirectorySeparatorChar + packageDirName;

        Directory.CreateDirectory(packagePath);

        var writer = File.CreateText(packagePath + Path.DirectorySeparatorChar + "package.json");
        writer.Write(PACKAGE_JSON,
            companyName.ToLower(),
            packageName.ToLower(),
            HttpUtility.JavaScriptStringEncode(packageDescription),
            unityVersion,
            unityRelease,
            documentationUrl,
            changelogUrl,
            licensesUrl,
            authorName,
            authorEmail,
            authorUrl,
            registry
        );
        writer.Flush();
        writer.Close();

        if (createAssemblyDefinition)
        {
            Directory.CreateDirectory(packagePath + Path.DirectorySeparatorChar + "Runtime");
            Directory.CreateDirectory(packagePath + Path.DirectorySeparatorChar + "Runtime" +
                                      Path.DirectorySeparatorChar +
                                      "Scripts");

            writer = File.CreateText(packagePath + Path.DirectorySeparatorChar + "Runtime" +
                                     Path.DirectorySeparatorChar +
                                     "Test.asmdef");
            writer.Write(ASMDEF,
                companyName.ToLower(),
                packageName.ToLower(),
                includePlatforms,
                excludePlatforms,
                allowUnsafeCode,
                overrideReferences,
                precompiledReferences,
                autoReferenced,
                defineConstraints,
                noEngineReferences
            );
            writer.Flush();
            writer.Close();
        }

        if (addGitignoreFile)
        {
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
  ""description"": ""{2}"",
  ""unity"": ""{3}"",
  ""unityRelease"": ""{4}"",
  ""documentationUrl"": ""{5}"",
  ""changelogUrl"": ""{6}"",
  ""licensesUrl"": ""{7}"",
  ""author"": {{
    ""name"": ""{8}"",
    ""email"": ""{9}"",
    ""url"": ""{10}""
  }},
  ""publishConfig"": {{
	""registry"": ""{11}""
  }}
}}";

    private static string ASMDEF = @"{{
  ""name"": ""{0}.{1}"",
  ""includePlatforms"": [{2}],
  ""excludePlatforms"": [{}],
  ""allowUnsafeCode"": {},
  ""overrideReferences"": {},
  ""precompiledReferences"": [{}],
  ""autoReferenced"": {},
  ""defineConstraints"": [{}],
  ""noEngineReferences"": {}
}}";
}