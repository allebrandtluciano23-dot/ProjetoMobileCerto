using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjetoIntegradorMobile.ViewModels
{
    // Classe base de todos os ViewModels.
    // Avisa a tela (View) sempre que uma propriedade muda, para o Binding atualizar o que aparece.
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string sPropriedade = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sPropriedade));
        }

        // Guarda o novo valor e avisa a tela, só se o valor realmente mudou
        protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string sPropriedade = "")
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor))
                return false;

            campo = valor;
            OnPropertyChanged(sPropriedade);
            return true;
        }

        // ---------- Mensagem de erro (usada por todas as telas com formulário) ----------

        private string sMensagemErro = "";
        public string MensagemErro
        {
            get => sMensagemErro;
            set
            {
                if (SetProperty(ref sMensagemErro, value))
                    OnPropertyChanged(nameof(TemErro));
            }
        }

        // O Label de erro fica visível só quando existe mensagem
        public bool TemErro => !string.IsNullOrEmpty(MensagemErro);

        // Evita que um toque duplo no botão execute a ação duas vezes
        protected bool bProcessando = false;
    }
}
