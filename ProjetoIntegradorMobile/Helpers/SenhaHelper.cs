using System.Security.Cryptography;

namespace ProjetoIntegradorMobile.Helpers
{
    /// <summary>
    /// Gera e confere hash de senha com PBKDF2 (SHA-256 + salt aleatório).
    /// Formato salvo no banco: "iteracoes.saltBase64.hashBase64"
    /// </summary>
    public static class SenhaHelper
    {
        private const int TamanhoSalt = 16;
        private const int TamanhoHash = 32;
        private const int Iteracoes = 100_000;

        public static string GerarHash(string senha)
        {
            var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
            return $"{Iteracoes}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verificar(string senha, string hashSalvo)
        {
            var partes = hashSalvo.Split('.');
            if (partes.Length != 3 || !int.TryParse(partes[0], out var iteracoes))
                return false;

            try
            {
                var salt = Convert.FromBase64String(partes[1]);
                var esperado = Convert.FromBase64String(partes[2]);
                var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, HashAlgorithmName.SHA256, esperado.Length);
                return CryptographicOperations.FixedTimeEquals(hash, esperado);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
