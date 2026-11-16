using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjetoIntegradorMobile.ViewModels
{
    // Base dos ViewModels: avisa a tela quando uma propriedade muda
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string sPropriedade = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(sPropriedade));
        }

        // Guarda o valor e avisa a tela, se mudou
        protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string sPropriedade = "")
        {
            if (EqualityComparer<T>.Default.Equals(campo, valor))
                return false;

            campo = valor;
            OnPropertyChanged(sPropriedade);
            return true;
        }

        // Mensagem de erro

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

        // Mostra o Label de erro só quando há mensagem
        public bool TemErro => !string.IsNullOrEmpty(MensagemErro);

        // Evita executar a ação duas vezes no toque duplo
        protected bool bProcessando = false;
    }
}
