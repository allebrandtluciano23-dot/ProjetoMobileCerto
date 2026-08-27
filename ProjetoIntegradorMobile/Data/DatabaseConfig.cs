using ProjetoIntegradorMobile.Data;

namespace ProjetoIntegradorMobile
{
    public static class DatabaseConfig
    {
        private static Database? _database;

        public static Database Database
        {
            get
            {
                if (_database == null)
                {
                    string caminho = Path.Combine(
                        FileSystem.AppDataDirectory,
                        "cheefbook.db3"
                    );

                    _database = new Database(caminho);
                }

                return _database;
            }
        }
    }
}