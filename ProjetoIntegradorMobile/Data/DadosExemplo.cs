using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using SQLite;

namespace ProjetoIntegradorMobile.Data
{
    /// <summary>
    /// Receitas de exemplo gravadas na primeira vez que o app abre,
    /// para a tela inicial não começar vazia.
    /// Ficam no nome de um usuário "CheefBook" (ninguém consegue entrar com ele).
    /// Só rodam uma vez: se esse usuário já existir, nada é feito
    /// (então, se as receitas de exemplo forem excluídas, elas não voltam).
    /// </summary>
    public static class DadosExemplo
    {
        private const string EMAIL_USUARIO_EXEMPLO = "exemplos@cheefbook.app";

        public static async Task InserirAsync(SQLiteAsyncConnection database)
        {
            int iJaExiste = await database.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM usuario WHERE email = ?", EMAIL_USUARIO_EXEMPLO);
            if (iJaExiste > 0)
                return;

            await database.RunInTransactionAsync(conn =>
            {
                Usuario usuario = new Usuario
                {
                    Nome = "CheefBook",
                    Email = EMAIL_USUARIO_EXEMPLO,
                    SenhaHash = SenhaHelper.GerarHash(Guid.NewGuid().ToString()), // senha que ninguém sabe
                    DataCadastro = Database.Agora()
                };
                conn.Insert(usuario);

                InserirReceita(conn, usuario.IdUsuario, "Rápidos", "Omelete de queijo", 10, 1,
                    "1. Bata os ovos com o sal.\n" +
                    "2. Aqueça o azeite na frigideira e despeje os ovos.\n" +
                    "3. Quando firmar, coloque o queijo, dobre ao meio e sirva.",
                    ("Ovo", 3, "un"), ("Queijo muçarela", 50, "g"), ("Azeite", 1, "colher (sopa)"), ("Sal", 1, "pitada"));

                InserirReceita(conn, usuario.IdUsuario, "Almoço", "Frango grelhado com legumes", 40, 2,
                    "1. Tempere o frango com sal e deixe descansar 10 minutos.\n" +
                    "2. Grelhe o frango em fogo médio, 6 minutos de cada lado.\n" +
                    "3. Corte a abobrinha e a cenoura em rodelas e refogue no azeite.\n" +
                    "4. Sirva o frango com os legumes.",
                    ("Peito de frango", 400, "g"), ("Abobrinha", 1, "un"), ("Cenoura", 1, "un"),
                    ("Azeite", 2, "colher (sopa)"), ("Sal", 1, "pitada"));

                InserirReceita(conn, usuario.IdUsuario, "Jantar", "Sopa de legumes", 45, 4,
                    "1. Pique a cebola e refogue no azeite.\n" +
                    "2. Junte a batata e a cenoura em cubos e cubra com água.\n" +
                    "3. Cozinhe por 30 minutos, acerte o sal e sirva.",
                    ("Batata", 2, "un"), ("Cenoura", 2, "un"), ("Cebola", 1, "un"),
                    ("Azeite", 1, "colher (sopa)"), ("Água", 1.5, "l"), ("Sal", 1, "pitada"));

                InserirReceita(conn, usuario.IdUsuario, "Sobremesas", "Mousse de maracujá", 20, 6,
                    "1. Bata no liquidificador o leite condensado, o creme de leite e o suco.\n" +
                    "2. Coloque em taças e leve à geladeira por 3 horas.",
                    ("Leite condensado", 395, "g"), ("Creme de leite", 200, "g"), ("Suco de maracujá", 200, "ml"));
            });
        }

        // Grava uma receita e seus ingredientes (criando os ingredientes que ainda não existem)
        private static void InserirReceita(
            SQLiteConnection conn, int iIdUsuario, string sCategoria, string sTitulo,
            int iTempoMin, int iPorcoes, string sModoPreparo,
            params (string Nome, double Quantidade, string Unidade)[] ingredientes)
        {
            int iIdCategoria = conn.ExecuteScalar<int>(
                "SELECT id_categoria FROM categoria WHERE nome = ?", sCategoria);

            Receita receita = new Receita
            {
                IdUsuario = iIdUsuario,
                IdCategoria = iIdCategoria > 0 ? iIdCategoria : null,
                Titulo = sTitulo,
                ModoPreparo = sModoPreparo,
                TempoPreparoMin = iTempoMin,
                Porcoes = iPorcoes,
                DataCriacao = Database.Agora()
            };
            conn.Insert(receita);

            foreach (var item in ingredientes)
            {
                string sNome = item.Nome;
                Ingrediente? ingrediente = conn.Table<Ingrediente>().FirstOrDefault(i => i.Nome == sNome);
                if (ingrediente == null)
                {
                    ingrediente = new Ingrediente { Nome = sNome, UnidadePadrao = item.Unidade };
                    conn.Insert(ingrediente);
                }

                conn.Execute(
                    "INSERT OR REPLACE INTO receita_ingrediente (id_receita, id_ingrediente, quantidade, unidade) VALUES (?, ?, ?, ?)",
                    receita.IdReceita, ingrediente.IdIngrediente, item.Quantidade, item.Unidade);
            }
        }
    }
}
