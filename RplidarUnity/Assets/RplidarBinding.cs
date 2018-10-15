using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

using System;

[StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
public struct LidarData
{
    [MarshalAs(UnmanagedType.Bool)]
    public bool syncBit;
    [MarshalAs(UnmanagedType.R4)]
    public float theta;
    [MarshalAs(UnmanagedType.Bool)]
    public float distant;
    [MarshalAs(UnmanagedType.SysUInt)]
    public uint quality;
};

public class RplidarBinding
{

    static RplidarBinding()
    {
        var currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
#if UNITY_EDITOR_64
        currentPath += Path.PathSeparator + Application.dataPath + "/Plugins/x86_64/";
#elif UNITY_EDITOR_32
        currentPath += Path.PathSeparator + Application.dataPath+ "/Plugins/x86/";
#endif
        Environment.SetEnvironmentVariable("PATH", currentPath);
    }


    [DllImport("RplidarCpp.dll")]
    public static extern int OnConnect(string port);
    [DllImport("RplidarCpp.dll")]
    public static extern bool OnDisconnect();

    [DllImport("RplidarCpp.dll")]
    public static extern bool StartMotor();
    [DllImport("RplidarCpp.dll")]
    public static extern bool EndMotor();

    [DllImport("RplidarCpp.dll")]
    public static extern bool StartScan();
    [DllImport("RplidarCpp.dll")]
    public static extern bool EndScan();

    [DllImport("RplidarCpp.dll")]
    public static extern bool ReleaseDrive();

    [DllImport("RplidarCpp.dll")]
    public static extern int GetLDataSize();

    [DllImport("RplidarCpp.dll")]
    private static extern void GetLDataSampleArray(IntPtr ptr);

    [DllImport("RplidarCpp.dll")]
    private static extern int GrabData(IntPtr ptr);

    public static LidarData[] GetSampleData()
    {
        var d = new LidarData[2];
        var handler = GCHandle.Alloc(d, GCHandleType.Pinned);
        GetLDataSampleArray(handler.AddrOfPinnedObject());
        handler.Free();
        return d;
    }

    public static int GetData(ref LidarData[] data)
    {
        var handler = GCHandle.Alloc(data, GCHandleType.Pinned);
        int count = GrabData(handler.AddrOfPinnedObject());
        handler.Free();

        return count;
    }
}
