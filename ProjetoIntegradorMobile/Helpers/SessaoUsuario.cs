using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.Helpers
{
    // Usuário logado (em memória)
    public static class SessaoUsuario
    {
        public static Usuario? UsuarioAtual { get; set; }

        public static bool EstaLogado => UsuarioAtual != null;

        public static void Encerrar() => UsuarioAtual = null;
    }
}
