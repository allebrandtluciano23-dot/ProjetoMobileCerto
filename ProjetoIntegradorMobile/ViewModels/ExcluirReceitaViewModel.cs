using ProjetoIntegradorMobile.DAO;
using ProjetoIntegradorMobile.Helpers;
using ProjetoIntegradorMobile.Models;

namespace ProjetoIntegradorMobile.ViewModels
{
    // ViewModel da confirmação de exclusão
    public class ExcluirReceitaViewModel : BaseViewModel
    {
        private readonly ReceitaDAO receitaDAO = new ReceitaDAO();

        private readonly int iIdReceita;

        public ExcluirReceitaViewModel(int iIdReceita, string sNomeReceita)
        {
            this.iIdReceita = iIdReceita;
            Mensagem = $"Tem certeza que deseja excluir \"{sNomeReceita}\"? Esta ação não pode ser desfeita.";
        }

        private string sMensagem = "";
        public string Mensagem
        {
            get => sMensagem;
            set => SetProperty(ref sMensagem, value);
        }

        // Avisa se alguém capturou (as cópias serão apagadas)
        public async Task VerificarCapturasAsync()
        {
            try
            {
                int iCapturas = await receitaDAO.ContarCapturasAsync(iIdReceita);
                if (iCapturas == 1)
                    Mensagem += "\n\nAtenção: 1 pessoa capturou esta receita. A versão dela também será apagada.";
                else if (iCapturas > 1)
                    Mensagem += $"\n\nAtenção: {iCapturas} pessoas capturaram esta receita. As versões delas também serão apagadas.";
            }
            catch (Exception)
            {
                // Sem contagem, segue sem o aviso
            }
        }

        // Exclui a receita (ingredientes e cópias vão junto)
        public async Task<bool> ExcluirAsync()
        {
            if (bProcessando)
                return false;

            MensagemErro = "";
            bProcessando = true;
            try
            {
                Receita? receita = await receitaDAO.BuscarPorIdAsync(iIdReceita);
                if (receita == null)
                    return true;  // já não existe

                // Só o autor pode excluir
                if (SessaoUsuario.UsuarioAtual?.IdUsuario != receita.IdUsuario)
                {
                    MensagemErro = "Só quem criou a receita pode excluí-la.";
                    return false;
                }

                await receitaDAO.ExcluirAsync(iIdReceita);
                ApagarFoto(receita.CaminhoFoto);
                return true;
            }
            catch (Exception)
            {
                MensagemErro = "Não foi possível excluir a receita. Tente novamente.";
                return false;
            }
            finally
            {
                bProcessando = false;
            }
        }

        // Apaga o arquivo da foto
        private static void ApagarFoto(string? sCaminhoFoto)
        {
            try
            {
                if (!string.IsNullOrEmpty(sCaminhoFoto) && File.Exists(sCaminhoFoto))
                    File.Delete(sCaminhoFoto);
            }
            catch (Exception)
            {
                // Falha ao apagar a foto não afeta o usuário
            }
        }
    }
}
