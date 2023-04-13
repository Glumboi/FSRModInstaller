using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FSRModInstaller.FsrFiles;

public class FsrVersions
{
    private static readonly string _fsrBinDir = AssemblyDirectory + "\\FsrVersions";

    private static string AssemblyDirectory
    {
        get
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }

    public static List<string> FsrVersionsPath
    {
        get;
        private set;
    }

    public static List<string> FsrVersionsName
    {
        get;
        private set;
    }

    public static void GetAllFsrVersions()
    {
        if (!Directory.Exists(_fsrBinDir)) return;

        FsrVersionsName = new List<string>();
        FsrVersionsPath = new List<string>();

        var array = Directory.GetDirectories(_fsrBinDir);
        for (var i = array.Length - 1; i >= 0; i--)
        {
            var item = array[i];
            FsrVersionsPath.Add(item);
            FsrVersionsName.Add(Path.GetFileName(item));
        }
    }
}