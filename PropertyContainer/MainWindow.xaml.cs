namespace PropertyContainer
{
  using System;
  using System.ComponentModel;
  using CoreLib.Core;
  using CoreLib.UnityExt;
  using PropertyContainer.ViewModels;
  using Unity;
  using System.Windows;
  using static CoreLib.Extensions.RefUtils;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    private IDisposable _nameCleanup;
    private IDisposable _ageCleanup;
    
    public MainWindow()
    {
      InitializeComponent();
      
      var container = new UnityContainer();
      container.AddNewExtension<UnityPropertyContainerExt>();
      container.MarkPropertyContainer<ViewModel>();
      container.MarkPropertyContainer<ViewModelWithErrorInfo>();
      container.MarkPropertyContainer<ViewModelWithDirtyTracker>();
      container.RegisterType<IPropertyDirtyTracker, PropertyDirtyTracker>();

      var vm = ViewModel = container.Resolve<ViewModel>();
      vm.HookChange(o => o.Name, HandleNameChanged);
      vm.HookChange(o => o.Age, HandleAgeChanged);
      vm.HookChange(HandlePropertyChanged);

      var vmWithErrorInfo = ViewModelWithErrorInfo = container.Resolve<ViewModelWithErrorInfo>();
      vmWithErrorInfo.HookChange(o => o.Name, HandleNameChanged);
      vmWithErrorInfo.HookChange(o => o.Age, HandleAgeChanged);
      vmWithErrorInfo.HookChange(HandlePropertyChanged);
      
      var viewModelWithDirtyTracker = ViewModelWithDirtyTracker = container.Resolve<ViewModelWithDirtyTracker>();
      viewModelWithDirtyTracker.HookChange(o => o.Name, HandleNameChanged);
      viewModelWithDirtyTracker.HookChange(o => o.Age, HandleAgeChanged);
      viewModelWithDirtyTracker.HookChange(HandlePropertyChanged);
      
      DataContext = this;
    }

    public ViewModel ViewModel { get; set; }
    
    public ViewModelWithErrorInfo ViewModelWithErrorInfo { get; set; }
    
    public ViewModelWithDirtyTracker ViewModelWithDirtyTracker { get; set; }

    private void HandleNameChanged(object sender, ValueChangedArgs<string> e)
    {
      var property = (IProperty) sender;
      LogBox.Items.Add($"[{property.Parent}.Name Changed] {e}");
    }

    private void HandleAgeChanged(object sender, ValueChangedArgs<int> e)
    {
      var property = (IProperty) sender;
      LogBox.Items.Add($"[{property.Parent}.Age Changed] {e}");
    }
    
    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      LogBox.Items.Add($"[{sender}.Property Changed] {e.PropertyName}");
    }

    private void EnableValidationsButtonClick(object sender, RoutedEventArgs e)
    {
      Dispose(ref _nameCleanup);
      _nameCleanup = ViewModelWithErrorInfo.SetupNameValidation();
      
      Dispose(ref _ageCleanup);
      _ageCleanup = ViewModelWithErrorInfo.SetupAgeValidation();
    }

    private void DisableValidationsButtonClick(object sender, RoutedEventArgs e)
    {
      ViewModelWithErrorInfo.ClearValidations();
    }
    
    private void DisableNameValidationsButtonClick(object sender, RoutedEventArgs e)
    {
      Dispose(ref _nameCleanup);
    }
    
    private void DisableAgeValidationsButtonClick(object sender, RoutedEventArgs e)
    {
      Dispose(ref _ageCleanup);
    }
  }
}