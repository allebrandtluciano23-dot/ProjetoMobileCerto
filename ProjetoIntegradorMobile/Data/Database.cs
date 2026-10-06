using SQLite;

namespace ProjetoIntegradorMobile.Data
{
    public class Database
    {
        private readonly SQLiteAsyncConnection _database;

        public Database(string caminhoBanco)
        {
            _database = new SQLiteAsyncConnection(caminhoBanco);
        }

        public async Task Inicializar()
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
        }
    }
}