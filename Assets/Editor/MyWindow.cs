using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class WizardCreatePackage : ScriptableWizard
{

    public string packageName;
    public string resultName;
    
    [MenuItem("MotherPacker/Create Package")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<WizardCreatePackage>("Create Package", "Create", "Apply");
    }

    void OnWizardCreate()
    {
        var dataPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
        Directory.CreateDirectory(dataPath + "Packages" + Path.DirectorySeparatorChar + packageName);
        
    }

    void OnWizardUpdate()
    {
        helpString = "Please set the color of the light!";
        resultName = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
        resultName += "Packages" + Path.DirectorySeparatorChar + packageName;
    }
    
}