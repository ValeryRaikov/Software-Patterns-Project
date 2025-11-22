using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WeddingPlannerWPF.ViewModels
{
    // Базов клас за всички ViewModels с INotifyPropertyChanged -> data binding поддръжка в WPF
    // Уведомява View-то за промени в свойствата на ViewModel-а
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
