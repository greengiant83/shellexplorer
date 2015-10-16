using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;





public class DisplayHandle : MonoBehaviour 
{
    TextMesh textMesh;
    string s;

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    /// <summary>
    /// Returns a list of child windows
    /// </summary>
    /// <param name="parent">Parent of the windows to return</param>
    /// <returns>List of child windows</returns>
    public static List<IntPtr> GetChildWindows(IntPtr parent)
    {
        List<IntPtr> result = new List<IntPtr>();
        GCHandle listHandle = GCHandle.Alloc(result);
        try
        {
            EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
            EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
        }
        finally
        {
            if (listHandle.IsAllocated)
                listHandle.Free();
        }
        return result;
    }

    /// <summary>
    /// Callback method to be used when enumerating windows.
    /// </summary>
    /// <param name="handle">Handle of the next window</param>
    /// <param name="pointer">Pointer to a GCHandle that holds a reference to the list to fill</param>
    /// <returns>True to continue the enumeration, false to bail</returns>
    private static bool EnumWindow(IntPtr handle, IntPtr pointer)
    {
        GCHandle gch = GCHandle.FromIntPtr(pointer);
        List<IntPtr> list = gch.Target as List<IntPtr>;
        if (list == null)
        {
            throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
        }
        list.Add(handle);
        //  You can modify this to check to see if you want to cancel the operation, then return a null here
        return true;
    }

    /// <summary>
    /// Delegate for the EnumChildWindows method
    /// </summary>
    /// <param name="hWnd">Window handle</param>
    /// <param name="parameter">Caller-defined variable; we use it for a pointer to our list</param>
    /// <returns>True to continue enumerating, false to bail.</returns>
    public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

    List<IntPtr> GetRootWindowsOfProcess(int pid)
    {
        List<IntPtr> rootWindows = GetChildWindows(IntPtr.Zero);
        List<IntPtr> dsProcRootWindows = new List<IntPtr>();
        foreach (IntPtr hWnd in rootWindows)
        {
            uint lpdwProcessId;
            GetWindowThreadProcessId(hWnd, out lpdwProcessId);
            if (lpdwProcessId == pid)
                dsProcRootWindows.Add(hWnd);
        }
        return dsProcRootWindows;
    }

	void Start () 
    {
        textMesh = GetComponent<TextMesh>();

        s = "Debug:\n";
        s += "Current Process Id: " + Process.GetCurrentProcess().Id + "\n";
        s += "Process Name: " + Process.GetCurrentProcess().ProcessName + "\n";
        s += "Module: " + Process.GetCurrentProcess().Modules.Count + "\n";

        var list = GetRootWindowsOfProcess(Process.GetCurrentProcess().Id);
        s += "list: " + list.Count + "\n";

        foreach (var id in list)
        {
            s += "id: " + id + "\n";
        }

        textMesh.text = s;
	}
	
	void Update () 
    {

        
	}
}
