using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AssetBundleEditor
{
    [MenuItem("Itools/BuildeAssetBuild")]
    public static void BuildAssetBundle()
    {
        string outPath = Application.dataPath + "/AssetBundles";
        BuildPipeline.BuildAssetBundles(outPath, 0,EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }
	[MenuItem("Itools/MarkAssetBundle")]
    public static void MarkAssetBundle()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();
        string path = Application.dataPath + "/Prefabs/";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
        
        for (int i = 0; i < fileInfo.Length; i++)
        {
            FileSystemInfo tempFile = fileInfo[i];
            if (tempFile is DirectoryInfo)
            {
                string tempPath = Path.Combine(path,tempFile.Name);
                SenecenOverView(tempPath);
            }
        }
        AssetDatabase.Refresh();
    }
    //对整个场景遍历
    public static void SenecenOverView(string scencePath)
    {
        string textFileName = "Record.txt";
        string tempPath = scencePath + textFileName;
        FileStream fs = new FileStream(tempPath,FileMode.OpenOrCreate);
        StreamWriter bw = new StreamWriter(fs);
        //存储对应关系
        Dictionary<string, string> readDict = new Dictionary<string, string>();
        ChangeHead(scencePath, readDict);
        foreach ( string key in readDict.Keys)
        {
            bw.Write(key);
            bw.Write(" ");
            bw.Write(readDict[key]);
            bw.Write("\n");
        }
        bw.Close();
        fs.Close();
    }
    /// <summary>
    /// (字符串)截取相对路径
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="theWriter"></param>
    /// D:/TuLuFish/Assets/Scences/  scenceOne
    public static void ChangeHead(string fullPath,Dictionary<string ,string>theWriter)
    {
        int tempCount = fullPath.IndexOf("Assets");
        int tempLength = fullPath.Length;
        string replacePath = fullPath.Substring(tempCount,tempLength-tempCount);
        DirectoryInfo dir = new DirectoryInfo(fullPath);
        if (dir!=null)
        {
            ListFiles(dir,replacePath,theWriter);
        }
        else
        {
            Debug.Log("this path is not exits");
        }
    }
    public static void ListFiles(FileSystemInfo info,string replacePath,Dictionary<string,string>theWriter)
    {
        if (!info.Exists)
        {
            Debug.Log("is not exits");
            return;
        }
        DirectoryInfo dir = info as DirectoryInfo;
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i] as FileInfo;
            //对文件的操作
            if (file!=null)
            {
                ChangeMark(file,replacePath,theWriter);
            }
            else//对于目录的操作
            {
                ListFiles(files[i],replacePath,theWriter);
            }
        }
    }
    public static string FixedWindowsPath(string path)
    {
        path = path.Replace("\\","/");
        return path;
    }
    public static string  GetBundlePath(FileInfo file,string replacePath)
    {
        string tempPath = file.FullName;
        Debug.Log("tempPath=="+tempPath);
        tempPath = FixedWindowsPath(tempPath);
        Debug.Log("replace==" + replacePath);
        int assetCount = tempPath.IndexOf(replacePath);
        Debug.Log("file.Name=="+file.Name);
        assetCount += replacePath.Length + 1;
        int nameCount = tempPath.LastIndexOf(file.Name);
        int tempLength = nameCount - assetCount;
        int tempCount = replacePath.LastIndexOf("/");
        string scenceHead = replacePath.Substring(tempCount+1,replacePath.Length-tempCount-1);
        Debug.Log("scenceHead=="+scenceHead);
        if (tempLength>0)
        {
            Debug.Log("assetCount==" + assetCount);
            string subString = tempPath.Substring(assetCount,tempPath.Length-assetCount);
            string[] result = subString.Split("/".ToCharArray());
            return scenceHead + "/" + result[0];
        }
        else
        {
            return scenceHead;
        }
    }
    public static void ChangeMark(FileInfo tempFile,string replacePath,Dictionary<string, string> theWriter)
    {
        if (tempFile.Extension==".meta")
        {
            return;
        }
        string markStr = GetBundlePath(tempFile,replacePath);
        Debug.Log("markStr=="+markStr);
        ChangeAssetMark(tempFile,markStr,theWriter);

    }
    public static void ChangeAssetMark(FileInfo tempFile,string markStr,Dictionary<string,string > theWriter)
    {
        string fullPath = tempFile.FullName;
        int assetCount = fullPath.IndexOf("Assets");
        string assetPath = fullPath.Substring(assetCount, fullPath.Length - assetCount);
        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
        importer.assetBundleName = markStr;
        if (tempFile.Extension==".unity")
        {
            importer.assetBundleVariant = "u3d";
        }
        else
        {
            importer.assetBundleVariant = "ld";
        }
        string[] subMark = markStr.Split("/".ToCharArray());
        string modleName = "";
        if (subMark.Length>1)
        {
            modleName = subMark[1];
        }
        else
        {
            modleName = markStr;
        }
        string modlePath = markStr.ToLower() + "." + importer.assetBundleVariant;
        if (!theWriter.ContainsKey(modleName))
        {
            theWriter.Add(modleName,modlePath);
        }
    }
}
