using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace ProjetoIntegradorMobile.Helpers
{
    // Escolhe foto da galeria e guarda uma cópia no app
    public static class FotoHelper
    {
        // Abre a galeria e copia a foto; null = cancelou
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

        // Apaga uma foto antiga
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
