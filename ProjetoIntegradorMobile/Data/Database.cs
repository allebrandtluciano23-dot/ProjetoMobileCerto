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

        private async Task<SQLiteAsyncConnection> Conexao()
        {
            await Inicializar();
            return _database;
        }

        private static string Agora() => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

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
                    data_criacao TEXT NOT NULL DEFAULT (datetime('now')),

                    FOREIGN KEY (id_usuario)
                        REFERENCES usuario(id_usuario)
                        ON DELETE CASCADE,

                    FOREIGN KEY (id_categoria)
                        REFERENCES categoria(id_categoria)
                        ON DELETE SET NULL
                );
            ");

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

        // =====================================================================
        // USUÁRIO
        // =====================================================================

        /// <summary>
        /// Cadastra um usuário guardando a senha com hash.
        /// Retorna null se o e-mail já estiver cadastrado.
        /// </summary>
        public async Task<Usuario?> CadastrarUsuarioAsync(string nome, string email, string senha)
        {
            var db = await Conexao();
            email = email.Trim().ToLowerInvariant();

            if (await EmailExisteAsync(email))
                return null;

            var usuario = new Usuario
            {
                Nome = nome.Trim(),
                Email = email,
                SenhaHash = SenhaHelper.GerarHash(senha),
                DataCadastro = Agora()
            };

            await db.InsertAsync(usuario); // preenche usuario.IdUsuario
            return usuario;
        }

        /// <summary>
        /// Confere e-mail e senha. Retorna o usuário se estiverem corretos, senão null.
        /// </summary>
        public async Task<Usuario?> AutenticarAsync(string email, string senha)
        {
            var usuario = await BuscarUsuarioPorEmailAsync(email);
            if (usuario == null || !SenhaHelper.Verificar(senha, usuario.SenhaHash))
                return null;
            return usuario;
        }

        public async Task<Usuario?> BuscarUsuarioPorEmailAsync(string email)
        {
            var db = await Conexao();
            email = email.Trim().ToLowerInvariant();
            return await db.Table<Usuario>().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> BuscarUsuarioPorIdAsync(int idUsuario)
        {
            var db = await Conexao();
            return await db.FindAsync<Usuario>(idUsuario);
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            var db = await Conexao();
            email = email.Trim().ToLowerInvariant();
            return await db.Table<Usuario>().CountAsync(u => u.Email == email) > 0;
        }

        public async Task<int> AtualizarUsuarioAsync(Usuario usuario)
        {
            var db = await Conexao();
            return await db.UpdateAsync(usuario);
        }

        public async Task<bool> AlterarSenhaAsync(int idUsuario, string senhaAtual, string novaSenha)
        {
            var db = await Conexao();
            var usuario = await db.FindAsync<Usuario>(idUsuario);
            if (usuario == null || !SenhaHelper.Verificar(senhaAtual, usuario.SenhaHash))
                return false;

            usuario.SenhaHash = SenhaHelper.GerarHash(novaSenha);
            await db.UpdateAsync(usuario);
            return true;
        }

        // Apaga o usuário e, em cascata, tudo dele (receitas, planos, refeições...)
        public async Task<int> ExcluirUsuarioAsync(int idUsuario)
        {
            var db = await Conexao();
            return await db.DeleteAsync<Usuario>(idUsuario);
        }

        // =====================================================================
        // CATEGORIA
        // =====================================================================

        public async Task<List<Categoria>> ListarCategoriasAsync()
        {
            var db = await Conexao();
            return await db.Table<Categoria>().OrderBy(c => c.Nome).ToListAsync();
        }

        public async Task<Categoria?> BuscarCategoriaPorNomeAsync(string nome)
        {
            var db = await Conexao();
            nome = nome.Trim();
            return await db.Table<Categoria>().FirstOrDefaultAsync(c => c.Nome == nome);
        }

        public async Task<Categoria> ObterOuCriarCategoriaAsync(string nome)
        {
            var existente = await BuscarCategoriaPorNomeAsync(nome);
            if (existente != null)
                return existente;

            var db = await Conexao();
            var categoria = new Categoria { Nome = nome.Trim() };
            await db.InsertAsync(categoria);
            return categoria;
        }

        // =====================================================================
        // INGREDIENTE
        // =====================================================================

        public async Task<List<Ingrediente>> ListarIngredientesAsync()
        {
            var db = await Conexao();
            return await db.Table<Ingrediente>().OrderBy(i => i.Nome).ToListAsync();
        }

        public async Task<Ingrediente?> BuscarIngredientePorNomeAsync(string nome)
        {
            var db = await Conexao();
            nome = nome.Trim();
            return await db.Table<Ingrediente>().FirstOrDefaultAsync(i => i.Nome == nome);
        }

        public async Task<Ingrediente> ObterOuCriarIngredienteAsync(string nome, string? unidadePadrao = null)
        {
            var existente = await BuscarIngredientePorNomeAsync(nome);
            if (existente != null)
                return existente;

            var db = await Conexao();
            var ingrediente = new Ingrediente { Nome = nome.Trim(), UnidadePadrao = unidadePadrao };
            await db.InsertAsync(ingrediente);
            return ingrediente;
        }

        // =====================================================================
        // RECEITA
        // =====================================================================

        public async Task<int> InserirReceitaAsync(Receita receita)
        {
            var db = await Conexao();
            if (string.IsNullOrEmpty(receita.DataCriacao))
                receita.DataCriacao = Agora();
            await db.InsertAsync(receita); // preenche receita.IdReceita
            return receita.IdReceita;
        }

        /// <summary>
        /// Salva a receita e os ingredientes de uma vez (se algo falhar, nada é salvo).
        /// Ingredientes que ainda não existem são criados automaticamente.
        /// </summary>
        public async Task<int> SalvarReceitaComIngredientesAsync(
            Receita receita,
            IEnumerable<(string Nome, double Quantidade, string? Unidade)> ingredientes)
        {
            var db = await Conexao();
            if (string.IsNullOrEmpty(receita.DataCriacao))
                receita.DataCriacao = Agora();

            await db.RunInTransactionAsync(conn =>
            {
                conn.Insert(receita);

                foreach (var item in ingredientes)
                {
                    var nome = item.Nome.Trim();
                    var ingrediente = conn.Table<Ingrediente>().FirstOrDefault(i => i.Nome == nome);
                    if (ingrediente == null)
                    {
                        ingrediente = new Ingrediente { Nome = nome, UnidadePadrao = item.Unidade };
                        conn.Insert(ingrediente);
                    }

                    conn.Execute(
                        "INSERT OR REPLACE INTO receita_ingrediente (id_receita, id_ingrediente, quantidade, unidade) VALUES (?, ?, ?, ?)",
                        receita.IdReceita, ingrediente.IdIngrediente, item.Quantidade, item.Unidade);
                }
            });

            return receita.IdReceita;
        }

        public async Task<int> AtualizarReceitaAsync(Receita receita)
        {
            var db = await Conexao();
            return await db.UpdateAsync(receita);
        }

        // Apaga a receita e, em cascata, ingredientes, adaptações, refeições e sugestões ligadas a ela
        public async Task<int> ExcluirReceitaAsync(int idReceita)
        {
            var db = await Conexao();
            return await db.DeleteAsync<Receita>(idReceita);
        }

        public async Task<Receita?> BuscarReceitaPorIdAsync(int idReceita)
        {
            var db = await Conexao();
            return await db.FindAsync<Receita>(idReceita);
        }

        public async Task<List<Receita>> ListarReceitasAsync()
        {
            var db = await Conexao();
            return await db.QueryAsync<Receita>(
                "SELECT * FROM receita ORDER BY data_criacao DESC");
        }

        public async Task<List<Receita>> ListarReceitasPorUsuarioAsync(int idUsuario)
        {
            var db = await Conexao();
            return await db.QueryAsync<Receita>(
                "SELECT * FROM receita WHERE id_usuario = ? ORDER BY data_criacao DESC", idUsuario);
        }

        public async Task<List<Receita>> ListarReceitasPorCategoriaAsync(int idCategoria)
        {
            var db = await Conexao();
            return await db.QueryAsync<Receita>(
                "SELECT * FROM receita WHERE id_categoria = ? ORDER BY titulo", idCategoria);
        }

        /// <summary>Busca receitas pelo título ou por um ingrediente que elas usam.</summary>
        public async Task<List<Receita>> BuscarReceitasAsync(string texto)
        {
            var db = await Conexao();
            var termo = "%" + texto.Trim() + "%";
            return await db.QueryAsync<Receita>(@"
                SELECT DISTINCT r.* FROM receita r
                LEFT JOIN receita_ingrediente ri ON ri.id_receita = r.id_receita
                LEFT JOIN ingrediente i ON i.id_ingrediente = ri.id_ingrediente
                WHERE r.titulo LIKE ? OR i.nome LIKE ?
                ORDER BY r.titulo", termo, termo);
        }

        // =====================================================================
        // INGREDIENTES DA RECEITA
        // =====================================================================

        public async Task AdicionarIngredienteNaReceitaAsync(int idReceita, int idIngrediente, double quantidade, string? unidade)
        {
            var db = await Conexao();
            await db.ExecuteAsync(
                "INSERT OR REPLACE INTO receita_ingrediente (id_receita, id_ingrediente, quantidade, unidade) VALUES (?, ?, ?, ?)",
                idReceita, idIngrediente, quantidade, unidade);
        }

        public async Task RemoverIngredienteDaReceitaAsync(int idReceita, int idIngrediente)
        {
            var db = await Conexao();
            await db.ExecuteAsync(
                "DELETE FROM receita_ingrediente WHERE id_receita = ? AND id_ingrediente = ?",
                idReceita, idIngrediente);
        }

        public async Task<List<IngredienteDaReceita>> ListarIngredientesDaReceitaAsync(int idReceita)
        {
            var db = await Conexao();
            return await db.QueryAsync<IngredienteDaReceita>(@"
                SELECT i.id_ingrediente AS IdIngrediente, i.nome AS Nome,
                       ri.quantidade AS Quantidade, ri.unidade AS Unidade
                FROM receita_ingrediente ri
                JOIN ingrediente i ON i.id_ingrediente = ri.id_ingrediente
                WHERE ri.id_receita = ?
                ORDER BY i.nome", idReceita);
        }

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
