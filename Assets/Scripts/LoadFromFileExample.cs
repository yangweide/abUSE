using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class LoadFromFileExample : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        string path = "AssetBundles/cubewall.unity3d";
        //AssetBundle ab = AssetBundle.LoadFromFile(path);
        //GameObject wallPrefab = ab.LoadAsset<GameObject>("CubeWall");
        //Instantiate(wallPrefab);

        //AssetBundle ab2 = AssetBundle.LoadFromFile("AssetBundles/share.unity3d");
        //Object[] objs= ab.LoadAllAssets();
        //foreach(Object o in objs)
        //{
        //    Instantiate(o);
        //}

        //第一种加载AB的方式 LoadFromMemoryAsync
        //AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));
        //yield return request;
        //AssetBundle ab = request.assetBundle;
        //AssetBundle ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(path));


        //第二种加载AB的方式 LoadFromFile
        //AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        //yield return request;
        //AssetBundle ab = request.assetBundle;

        //第三种加载AB的方式 WWW
        //while(Caching.ready == false)
        //{
        //    yield return null;
        //}

        ////file://  file:///
        ////WWW www = WWW.LoadFromCacheOrDownload(@"file:/E:\Unity Project Workspace\AssetBundleProject\AssetBundles\cubewall.unity3d", 1);
        //WWW www = WWW.LoadFromCacheOrDownload(@"http://localhost/AssetBundles/cubewall.unity3d", 1);
        //yield return www;
        //if ( string.IsNullOrEmpty(www.error)==false )
        //{
        //    Debug.Log(www.error);yield break ;
        //}
        //AssetBundle ab = www.assetBundle;

        //第四种方式 使用UnityWebRequest
        //string uri = @"file:///E:\Unity Project Workspace\AssetBundleProject\AssetBundles\cubewall.unity3d";
        string uri = @"http://localhost/AssetBundles/cubewall.unity3d";
        UnityWebRequest request = UnityWebRequest.GetAssetBundle(uri);
        yield return request.Send();
        //AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);
        AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        
        //使用里面的资源
        GameObject wallPrefab = ab.LoadAsset<GameObject>("CubeWall");
        Instantiate(wallPrefab);


        AssetBundle manifestAB =  AssetBundle.LoadFromFile("AssetBundles/AssetBundles");
        AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        //foreach(string name in manifest.GetAllAssetBundles())
        //{
        //    print(name);
        //}
        string[] strs =  manifest.GetAllDependencies("cubewall.unity3d");
        foreach (string name in strs)
        {
            print(name);
            AssetBundle.LoadFromFile("AssetBundles/" + name);
        }
    }
    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    AssetBundle.LoadFromFile("AssetBundles/share.unity3d");
        //}
    }

}
