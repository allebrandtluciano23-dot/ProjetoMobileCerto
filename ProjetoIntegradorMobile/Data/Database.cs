using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;
using SQLite;

namespace ProjetoIntegradorMobile.Data
{
    public class Database
    {
        private readonly SQLiteAsyncConnection _database;
        private Task? _inicializacao;

        public Database(string caminhoBanco)
        {
            _database = new SQLiteAsyncConnection(caminhoBanco);
        }

        // Cria as tabelas uma única vez. Todos os métodos abaixo chamam isto antes de
        // acessar o banco, então não há problema se uma tela consultar antes do App terminar.
        public Task Inicializar() => _inicializacao ??= CriarTabelas();

        // Usado pelos DAOs: garante que as tabelas existem e devolve a conexão
        public async Task<SQLiteAsyncConnection> Conexao()
        {
            await Inicializar();
            return _database;
        }

        public static string Agora() => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        private async Task CriarTabelas()
        {
            // Ativa as chaves estrangeiras
            await _database.ExecuteAsync("PRAGMA foreign_keys = ON;");

            // USUARIO
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS usuario (
                    id_usuario INTEGER PRIMARY KEY AUTOINCREMENT,
                    nome TEXT NOT NULL,
                    email TEXT NOT NULL UNIQUE,
                    senha_hash TEXT NOT NULL,
                    data_cadastro TEXT NOT NULL DEFAULT (datetime('now'))
                );
            ");

            // CATEGORIA
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS categoria (
                    id_categoria INTEGER PRIMARY KEY AUTOINCREMENT,
                    nome TEXT NOT NULL UNIQUE
                );
            ");

            // INGREDIENTE
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS ingrediente (
                    id_ingrediente INTEGER PRIMARY KEY AUTOINCREMENT,
                    nome TEXT NOT NULL UNIQUE,
                    unidade_padrao TEXT
                );
            ");

            // RECEITA
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS receita (
                    id_receita INTEGER PRIMARY KEY AUTOINCREMENT,
                    id_usuario INTEGER NOT NULL,
                    id_categoria INTEGER,
                    titulo TEXT NOT NULL,
                    modo_preparo TEXT NOT NULL,
                    tempo_preparo_min INTEGER,
                    porcoes INTEGER,
                    favorita INTEGER NOT NULL DEFAULT 0,
                    data_criacao TEXT NOT NULL DEFAULT (datetime('now')),

                    FOREIGN KEY (id_usuario)
                        REFERENCES usuario(id_usuario)
                        ON DELETE CASCADE,

                    FOREIGN KEY (id_categoria)
                        REFERENCES categoria(id_categoria)
                        ON DELETE SET NULL
                );
            ");

            // Bancos criados antes da coluna "favorita" existir: adiciona a coluna.
            // Se ela já existir, o SQLite dá erro de coluna duplicada e só ignoramos.
            try
            {
                await _database.ExecuteAsync(
                    "ALTER TABLE receita ADD COLUMN favorita INTEGER NOT NULL DEFAULT 0;");
            }
            catch (SQLiteException)
            {
                // coluna já existe
            }

            // RECEITA_INGREDIENTE
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS receita_ingrediente (
                    id_receita INTEGER NOT NULL,
                    id_ingrediente INTEGER NOT NULL,
                    quantidade REAL NOT NULL,
                    unidade TEXT,

                    PRIMARY KEY (id_receita, id_ingrediente),

                    FOREIGN KEY (id_receita)
                        REFERENCES receita(id_receita)
                        ON DELETE CASCADE,

                    FOREIGN KEY (id_ingrediente)
                        REFERENCES ingrediente(id_ingrediente)
                        ON DELETE RESTRICT
                );
            ");

            // RECEITA_PERSONALIZADA
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS receita_personalizada (
                    id_receita_personalizada INTEGER PRIMARY KEY AUTOINCREMENT,
                    id_receita_original INTEGER NOT NULL,
                    id_usuario INTEGER NOT NULL,
                    titulo TEXT NOT NULL,
                    modo_preparo TEXT NOT NULL,
                    observacoes TEXT,
                    data_captura TEXT NOT NULL DEFAULT (datetime('now')),

                    FOREIGN KEY (id_receita_original)
                        REFERENCES receita(id_receita)
                        ON DELETE CASCADE,

                    FOREIGN KEY (id_usuario)
                        REFERENCES usuario(id_usuario)
                        ON DELETE CASCADE,

                    UNIQUE (id_receita_original, id_usuario)
                );
            ");

            // RECEITA_PERSONALIZADA_INGREDIENTE
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS receita_personalizada_ingrediente (
                    id_receita_personalizada INTEGER NOT NULL,
                    id_ingrediente INTEGER NOT NULL,
                    quantidade REAL NOT NULL,
                    unidade TEXT,

                    PRIMARY KEY (
                        id_receita_personalizada,
                        id_ingrediente
                    ),

                    FOREIGN KEY (id_receita_personalizada)
                        REFERENCES receita_personalizada(
                            id_receita_personalizada
                        )
                        ON DELETE CASCADE,

                    FOREIGN KEY (id_ingrediente)
                        REFERENCES ingrediente(id_ingrediente)
                        ON DELETE RESTRICT
                );
            ");

            // PLANO_ALIMENTAR
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS plano_alimentar (
                    id_plano INTEGER PRIMARY KEY AUTOINCREMENT,
                    id_usuario INTEGER NOT NULL,
                    nome TEXT NOT NULL,
                    descricao TEXT,
                    data_inicio TEXT,
                    data_fim TEXT,

                    FOREIGN KEY (id_usuario)
                        REFERENCES usuario(id_usuario)
                        ON DELETE CASCADE
                );
            ");

            // REFEICAO
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS refeicao (
                    id_refeicao INTEGER PRIMARY KEY AUTOINCREMENT,
                    id_usuario INTEGER NOT NULL,
                    id_plano INTEGER,
                    id_receita INTEGER,
                    id_receita_personalizada INTEGER,

                    tipo_refeicao TEXT NOT NULL
                        CHECK (
                            tipo_refeicao IN (
                                'cafe_da_manha',
                                'almoco',
                                'jantar',
                                'lanche',
                                'ceia'
                            )
                        ),

                    data TEXT NOT NULL,
                    horario TEXT,

                    FOREIGN KEY (id_usuario)
                        REFERENCES usuario(id_usuario)
                        ON DELETE CASCADE,

                    FOREIGN KEY (id_plano)
                        REFERENCES plano_alimentar(id_plano)
                        ON DELETE SET NULL,

                    FOREIGN KEY (id_receita)
                        REFERENCES receita(id_receita)
                        ON DELETE CASCADE,

                    FOREIGN KEY (id_receita_personalizada)
                        REFERENCES receita_personalizada(
                            id_receita_personalizada
                        )
                        ON DELETE CASCADE,

                    CHECK (
                        (id_receita IS NOT NULL
                         AND id_receita_personalizada IS NULL)
                        OR
                        (id_receita IS NULL
                         AND id_receita_personalizada IS NOT NULL)
                    )
                );
            ");

            // SUGESTAO
            await _database.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS sugestao (
                    id_sugestao INTEGER PRIMARY KEY AUTOINCREMENT,
                    id_receita INTEGER NOT NULL,
                    id_usuario INTEGER NOT NULL,
                    id_ingrediente_referenciado INTEGER,
                    etapa_referenciada TEXT,
                    texto TEXT NOT NULL,

                    data_envio TEXT NOT NULL
                        DEFAULT (datetime('now')),

                    status TEXT NOT NULL
                        DEFAULT 'pendente'

                        CHECK (
                            status IN (
                                'pendente',
                                'aceita',
                                'rejeitada'
                            )
                        ),

                    FOREIGN KEY (id_receita)
                        REFERENCES receita(id_receita)
                        ON DELETE CASCADE,

                    FOREIGN KEY (id_usuario)
                        REFERENCES usuario(id_usuario)
                        ON DELETE CASCADE,

                    FOREIGN KEY (id_ingrediente_referenciado)
                        REFERENCES ingrediente(id_ingrediente)
                        ON DELETE SET NULL
                );
            ");

            // ÍNDICES
            await _database.ExecuteAsync(@"
                CREATE INDEX IF NOT EXISTS idx_receita_usuario
                ON receita(id_usuario);
            ");

            await _database.ExecuteAsync(@"
                CREATE INDEX IF NOT EXISTS idx_receita_categoria
                ON receita(id_categoria);
            ");

            await _database.ExecuteAsync(@"
                CREATE INDEX IF NOT EXISTS idx_receita_pers_usuario
                ON receita_personalizada(id_usuario);
            ");

            await _database.ExecuteAsync(@"
                CREATE INDEX IF NOT EXISTS idx_refeicao_usuario_data
                ON refeicao(id_usuario, data);
            ");

            await _database.ExecuteAsync(@"
                CREATE INDEX IF NOT EXISTS idx_refeicao_plano
                ON refeicao(id_plano);
            ");

            await _database.ExecuteAsync(@"
                CREATE INDEX IF NOT EXISTS idx_sugestao_receita
                ON sugestao(id_receita);
            ");

            // CATEGORIAS PADRÃO (as mesmas usadas na tela Nova Receita)
            foreach (var nome in new[] { "Rápidos", "Almoço", "Jantar", "Sobremesas" })
            {
                await _database.ExecuteAsync(
                    "INSERT OR IGNORE INTO categoria (nome) VALUES (?)", nome);
            }
        }

        // Usuário, categoria, ingrediente e receita: ver a pasta DAO

        // =====================================================================
        // RECEITA PERSONALIZADA (adaptação de uma receita por um usuário)
        // =====================================================================

        /// <summary>
        /// Cria a adaptação copiando título, modo de preparo e ingredientes da receita original.
        /// Retorna null se o usuário já tiver uma adaptação dessa receita ou se a receita não existir.
        /// </summary>
        public async Task<ReceitaPersonalizada?> CriarAdaptacaoAsync(int idReceitaOriginal, int idUsuario)
        {
            var db = await Conexao();

            var original = await db.FindAsync<Receita>(idReceitaOriginal);
            if (original == null)
                return null;

            var jaExiste = await db.Table<ReceitaPersonalizada>()
                .CountAsync(r => r.IdReceitaOriginal == idReceitaOriginal && r.IdUsuario == idUsuario);
            if (jaExiste > 0)
                return null;

            var adaptacao = new ReceitaPersonalizada
            {
                IdReceitaOriginal = idReceitaOriginal,
                IdUsuario = idUsuario,
                Titulo = original.Titulo,
                ModoPreparo = original.ModoPreparo,
                DataCaptura = Agora()
            };

            await db.RunInTransactionAsync(conn =>
            {
                conn.Insert(adaptacao);
                conn.Execute(@"
                    INSERT INTO receita_personalizada_ingrediente
                        (id_receita_personalizada, id_ingrediente, quantidade, unidade)
                    SELECT ?, id_ingrediente, quantidade, unidade
                    FROM receita_ingrediente WHERE id_receita = ?",
                    adaptacao.IdReceitaPersonalizada, idReceitaOriginal);
            });

            return adaptacao;
        }

        public async Task<int> AtualizarAdaptacaoAsync(ReceitaPersonalizada adaptacao)
        {
            var db = await Conexao();
            return await db.UpdateAsync(adaptacao);
        }

        public async Task<int> ExcluirAdaptacaoAsync(int idReceitaPersonalizada)
        {
            var db = await Conexao();
            return await db.DeleteAsync<ReceitaPersonalizada>(idReceitaPersonalizada);
        }

        public async Task<ReceitaPersonalizada?> BuscarAdaptacaoPorIdAsync(int idReceitaPersonalizada)
        {
            var db = await Conexao();
            return await db.FindAsync<ReceitaPersonalizada>(idReceitaPersonalizada);
        }

        public async Task<List<ReceitaPersonalizada>> ListarAdaptacoesPorUsuarioAsync(int idUsuario)
        {
            var db = await Conexao();
            return await db.QueryAsync<ReceitaPersonalizada>(
                "SELECT * FROM receita_personalizada WHERE id_usuario = ? ORDER BY data_captura DESC", idUsuario);
        }

        public async Task AdicionarIngredienteNaAdaptacaoAsync(int idReceitaPersonalizada, int idIngrediente, double quantidade, string? unidade)
        {
            var db = await Conexao();
            await db.ExecuteAsync(
                "INSERT OR REPLACE INTO receita_personalizada_ingrediente (id_receita_personalizada, id_ingrediente, quantidade, unidade) VALUES (?, ?, ?, ?)",
                idReceitaPersonalizada, idIngrediente, quantidade, unidade);
        }

        public async Task RemoverIngredienteDaAdaptacaoAsync(int idReceitaPersonalizada, int idIngrediente)
        {
            var db = await Conexao();
            await db.ExecuteAsync(
                "DELETE FROM receita_personalizada_ingrediente WHERE id_receita_personalizada = ? AND id_ingrediente = ?",
                idReceitaPersonalizada, idIngrediente);
        }

        public async Task<List<IngredienteDaReceita>> ListarIngredientesDaAdaptacaoAsync(int idReceitaPersonalizada)
        {
            var db = await Conexao();
            return await db.QueryAsync<IngredienteDaReceita>(@"
                SELECT i.id_ingrediente AS IdIngrediente, i.nome AS Nome,
                       rpi.quantidade AS Quantidade, rpi.unidade AS Unidade
                FROM receita_personalizada_ingrediente rpi
                JOIN ingrediente i ON i.id_ingrediente = rpi.id_ingrediente
                WHERE rpi.id_receita_personalizada = ?
                ORDER BY i.nome", idReceitaPersonalizada);
        }

        // =====================================================================
        // PLANO ALIMENTAR
        // =====================================================================

        public async Task<int> InserirPlanoAsync(PlanoAlimentar plano)
        {
            var db = await Conexao();
            await db.InsertAsync(plano);
            return plano.IdPlano;
        }

        public async Task<int> AtualizarPlanoAsync(PlanoAlimentar plano)
        {
            var db = await Conexao();
            return await db.UpdateAsync(plano);
        }

        // As refeições do plano não são apagadas: ficam sem plano (ON DELETE SET NULL)
        public async Task<int> ExcluirPlanoAsync(int idPlano)
        {
            var db = await Conexao();
            return await db.DeleteAsync<PlanoAlimentar>(idPlano);
        }

        public async Task<List<PlanoAlimentar>> ListarPlanosPorUsuarioAsync(int idUsuario)
        {
            var db = await Conexao();
            return await db.Table<PlanoAlimentar>().Where(p => p.IdUsuario == idUsuario).ToListAsync();
        }

        // =====================================================================
        // REFEIÇÃO
        // =====================================================================

        public static readonly string[] TiposRefeicao = { "cafe_da_manha", "almoco", "jantar", "lanche", "ceia" };

        /// <summary>
        /// Insere a refeição. Preencha só IdReceita OU IdReceitaPersonalizada,
        /// e TipoRefeicao com um dos valores de TiposRefeicao.
        /// </summary>
        public async Task<int> InserirRefeicaoAsync(Refeicao refeicao)
        {
            if ((refeicao.IdReceita == null) == (refeicao.IdReceitaPersonalizada == null))
                throw new ArgumentException("Informe IdReceita ou IdReceitaPersonalizada (apenas um).");
            if (!TiposRefeicao.Contains(refeicao.TipoRefeicao))
                throw new ArgumentException("TipoRefeicao inválido: " + refeicao.TipoRefeicao);

            var db = await Conexao();
            await db.InsertAsync(refeicao);
            return refeicao.IdRefeicao;
        }

        public async Task<int> ExcluirRefeicaoAsync(int idRefeicao)
        {
            var db = await Conexao();
            return await db.DeleteAsync<Refeicao>(idRefeicao);
        }

        /// <param name="data">Formato "yyyy-MM-dd"</param>
        public async Task<List<Refeicao>> ListarRefeicoesDoDiaAsync(int idUsuario, string data)
        {
            var db = await Conexao();
            return await db.QueryAsync<Refeicao>(
                "SELECT * FROM refeicao WHERE id_usuario = ? AND data = ? ORDER BY horario", idUsuario, data);
        }

        public async Task<List<Refeicao>> ListarRefeicoesDoPlanoAsync(int idPlano)
        {
            var db = await Conexao();
            return await db.QueryAsync<Refeicao>(
                "SELECT * FROM refeicao WHERE id_plano = ? ORDER BY data, horario", idPlano);
        }

        // =====================================================================
        // SUGESTÃO
        // =====================================================================

        public async Task<int> EnviarSugestaoAsync(Sugestao sugestao)
        {
            var db = await Conexao();
            if (string.IsNullOrEmpty(sugestao.DataEnvio))
                sugestao.DataEnvio = Agora();
            sugestao.Status = "pendente";
            await db.InsertAsync(sugestao);
            return sugestao.IdSugestao;
        }

        public async Task<List<Sugestao>> ListarSugestoesDaReceitaAsync(int idReceita)
        {
            var db = await Conexao();
            return await db.QueryAsync<Sugestao>(
                "SELECT * FROM sugestao WHERE id_receita = ? ORDER BY data_envio DESC", idReceita);
        }

        /// <param name="status">"pendente", "aceita" ou "rejeitada"</param>
        public async Task AtualizarStatusSugestaoAsync(int idSugestao, string status)
        {
            if (status != "pendente" && status != "aceita" && status != "rejeitada")
                throw new ArgumentException("Status inválido: " + status);

            var db = await Conexao();
            await db.ExecuteAsync("UPDATE sugestao SET status = ? WHERE id_sugestao = ?", status, idSugestao);
        }

        public async Task<int> ExcluirSugestaoAsync(int idSugestao)
        {
            var db = await Conexao();
            return await db.DeleteAsync<Sugestao>(idSugestao);
        }
    }
}
