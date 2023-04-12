using System.Diagnostics;
using System.Reflection;

namespace FSRModInstaller.FsrFiles;

public class FsrBinaries
{
    private static List<string> FsrPaths => FsrVersions.FsrVersionsPath;

    private static readonly string _regPath = AssemblyDirectory + "\\RegEdits";

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

    public static void InstallFsr(int versionIndex, string dest)
    {
        foreach (string file in Directory.GetFiles(FsrPaths[versionIndex]))
        {
            var test = dest + "\\" + Path.GetFileName(file);
            File.Copy(file, test, true);
        }
    }

    public static void UninstallFsr(int versionIndex, string gameLoc)
    {
        foreach (string file in Directory.GetFiles(FsrPaths[versionIndex]))
        {
            foreach (var file2 in Directory.GetFiles(gameLoc))
            {
                if (Path.GetFileName(file2) == Path.GetFileName(file))
                {
                    File.Delete(file2);
                }
            }
        }
    }

    public static void ExecuteReg(bool install)
    {
        foreach (var item in Directory.GetFiles(_regPath))
        {
            if (install && item.Contains("Enable"))
            {
                ExecuteRegFileAsProcess(item);
            }

            if (!install && item.Contains("Disable"))
            {
                ExecuteRegFileAsProcess(item);
            }
        }
    }

    private static void ExecuteRegFileAsProcess(string path)
    {
        if (!File.Exists(path)) return;

        Process regeditProcess = Process.Start("regedit.exe", $"\"{path}\"");
        regeditProcess.WaitForExit();
    }
}