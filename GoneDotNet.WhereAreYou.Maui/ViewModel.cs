public abstract partial class ViewModel(BaseServices services) : ObservableValidator
{
    protected BaseServices Services => services;
}