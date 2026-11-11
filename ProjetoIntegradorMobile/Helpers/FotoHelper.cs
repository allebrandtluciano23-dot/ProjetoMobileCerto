using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace ProjetoIntegradorMobile.Helpers
{
    /// <summary>
    /// Escolher uma foto da galeria e guardar uma cópia na pasta do app
    /// (assim ela continua aparecendo mesmo se for apagada da galeria).
    /// </summary>
    public static class FotoHelper
    {
        /// <summary>
        /// Abre a galeria e copia a foto escolhida para FileSystem.AppDataDirectory/sSubpasta.
        /// Retorna o caminho da cópia, ou null se o usuário cancelou.
        /// Se der erro (ex.: permissão negada), a exceção sobe para quem chamou tratar.
        /// </summary>
        public static async Task<string?> EscolherECopiarAsync(string sSubpasta)
        {
            FileResult? foto = await MediaPicker.Default.PickPhotoAsync();
            if (foto == null)
                return null;

            string sPasta = Path.Combine(FileSystem.AppDataDirectory, sSubpasta);
            Directory.CreateDirectory(sPasta);

            string sExtensao = Path.GetExtension(foto.FileName);
            if (string.IsNullOrEmpty(sExtensao))
                sExtensao = ".jpg";

            string sDestino = Path.Combine(sPasta, Guid.NewGuid().ToString("N") + sExtensao);

            using (Stream origem = await foto.OpenReadAsync())
            using (FileStream destino = File.Create(sDestino))
            {
                await origem.CopyToAsync(destino);
            }

            return sDestino;
        }

        // Apaga um arquivo de foto antigo (se não conseguir, não é problema para o usuário)
        public static void Apagar(string? sCaminho)
        {
            try
            {
                if (!string.IsNullOrEmpty(sCaminho) && File.Exists(sCaminho))
                    File.Delete(sCaminho);
            }
            catch (Exception)
            {
            }
        }
    }
}
