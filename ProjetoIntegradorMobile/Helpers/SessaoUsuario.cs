using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.Helpers
{
    // Guarda o usuário logado em memória enquanto o app estiver aberto.
    public static class SessaoUsuario
    {
        public static Usuario? UsuarioAtual { get; set; }

        public static bool EstaLogado => UsuarioAtual != null;

        public static void Encerrar() => UsuarioAtual = null;
    }
}
