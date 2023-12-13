namespace Mint.Core;

public struct MintConfig
{
    public MintConfig()
    {
        LanguageID = 0;
        Database = new DatabaseConfig();
    }

    public byte LanguageID;
    public DatabaseConfig Database;
    public GameConfig Game;

    public struct DatabaseConfig
    {
        public DatabaseConfig()
        {
            Name = "MintServerDb";
            IP = "127.0.0.1";
            Port = 27017;
        }

        public string Name;
        public string IP;
        public int Port;
    }

    public struct GameConfig
    {
        public GameConfig()
        {
            MOTD = new string[]
            {
                "Добро пожаловать на наш сервер!",
                "Этот сервер использует ядро Mint с открытым исходным кодом, ссылка:",
                "https://github.com/terrariarealms/mint",
            };
        }

        public string? IP;
        public int? Port;
        public string[] MOTD;
    }
}