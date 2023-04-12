using System.Reflection;
using System.Xml;

namespace FSRModInstaller.Xml;

public class GameConfigs
{
    private static readonly string _xmlDir = AssemblyDirectory + "\\Xml";
    private static readonly string _configFile = _xmlDir + "\\GameConfigs.xml";

    private static readonly string _baseContent = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n\r\n<Games>\r\n\t<Watch_Dogs_Legion bin=\"bin\" />\r\n\t<CP2077 bin=\"bin\\\\x64\" />\r\n</Games>";

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

    private static void CreateConfig()
    {
        //Create xml dir
        if (!Directory.Exists(_xmlDir))
        {
            Directory.CreateDirectory(_xmlDir);
        }

        bool fileExists = File.Exists(_configFile);

        if (!File.Exists(_configFile) ||
            File.Exists(_configFile) && string.IsNullOrEmpty(File.ReadAllText(_configFile)))
        {
            // Open the file stream for writing
            using (FileStream fs = new FileStream(_configFile, FileMode.Create))
            {
                // Convert the string to bytes
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(_baseContent);

                // Write the bytes to the file stream
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }

    public static List<Game> GetAllGames(string filePath)
    {
        CreateConfig();

        List<Game> games = new List<Game>();

        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.Load(filePath);

        XmlNodeList gamesList = xmlDoc.GetElementsByTagName("Games");

        foreach (XmlNode gameNode in gamesList[0].ChildNodes)
        {
            string gameName = gameNode.Name;
            string bin = ((XmlElement)gameNode).GetAttribute("bin");

            games.Add(new Game(gameName, bin));
        }

        return games;
    }
}