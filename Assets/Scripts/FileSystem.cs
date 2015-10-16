using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System;

public class FileSystem : MonoBehaviour 
{
	void Start () 
    {
        discover();
	}

	void Update () 
    {
	
	}

    void discover()
    {
        string startingPath = @"E:\Drive";
        DirectoryInfo di = new DirectoryInfo(startingPath);

        long size = getDirectorySize(di);
        print("Folder size: " + size);
    }

    long getDirectorySize(DirectoryInfo di)
    {
        long size = 0;
        try
        {
            print(di.Name);
            size = di.GetFiles().Sum(i => i.Length);
            //size += di.GetDirectories().Sum(i => getDirectorySize(i));
        }
        catch(Exception ex)
        {

        }
        return size;
    }
}
