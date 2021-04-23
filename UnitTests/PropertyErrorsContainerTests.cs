namespace UnitTests
{
  using System;
  using System.ComponentModel;
  using System.Linq;
  using CoreLib.Core;
  using CoreLib.Extensions;
  using NSubstitute;
  using NUnit.Framework;

  public partial class PropertyErrorsContainerTests
  {
    private const string NullName = "Name cannot be null or empty";
    private const string NameTooShort = "Name is too short";

    [Test]
    public void Task_01_Creation()
    {
      var vm = CreateViewModel();
      var container = CreateContainer(vm);
      Assert.That(!container.HasErrors);
      Assert.That(!container.Errors.Cast<string>().Any());
      Assert.That(container.GetErrors("Name") == PropertyErrorsContainer.EmptyErrors);
      Assert.That(container.GetPropertyErrors("Name") == null);
    }
    
    [Test]
    public void Task_02_Add_And_Remove_Validation()
    {
      var vm = CreateViewModel();
      var container = CreateContainer(vm);
      
      PropertyErrorsChangedEventArgs errorArgs = null;
      container.PropertyErrorsChanged += (s, e) => errorArgs = e;
      
      container.SetupValidation(v => v.Name, ValidateName);
      
      Assert.That(errorArgs != null);
      Assert.That(errorArgs.Type == PropertyErrorsChangeType.Add);
      Assert.That(container.GetPropertyErrors("Name") == errorArgs.PropertyErrors);

      errorArgs = null;
      container.ClearValidations();
      
      Assert.That(errorArgs != null);
      Assert.That(errorArgs.Type == PropertyErrorsChangeType.Remove);
      Assert.That(container.GetPropertyErrors("Name") == null);
    }
    
    [Test]
    public void Task_03_Initial_Validation()
    {
      var vm = CreateViewModel();
      var container = CreateContainer(vm);
      
      DataErrorsChangedEventArgs containerArgs = null;
      container.ErrorsChanged += (s, e) => containerArgs = e;
      
      container.SetupValidation(v => v.Name, ValidateName);
      Assert.That(containerArgs != null);

      Assert.That(container.HasErrors);
      Assert.That(container.Errors.Cast<string>().ToArray()[0] == NullName);
      Assert.That(container.GetErrors("Name").Cast<string>().ToArray()[0] == NullName);
    }

    
    [Test]
    public void Task_02_Validation_Pass()
    {
      var vm = CreateViewModel();
      var container = CreateContainer(vm);
      
      container.SetupValidation(v => v.Name, ValidateName);
      
      DataErrorsChangedEventArgs containerArgs = null;
      container.ErrorsChanged += (s, e) => containerArgs = e;

      vm.Name = "Long name";
      
      Assert.That(containerArgs != null);
      Assert.That(!container.HasErrors);
      Assert.That(!container.Errors.Cast<string>().Any());
      Assert.That(!container.GetErrors("Name").Cast<string>().Any());
      Assert.That(container.GetPropertyErrors("Name").Errors.Count == 0);
    }
    
    public class TestViewModel : IPropertyContainer, INotifyPropertyChanged, IPropertyChangeNotifiable
    {
      private IProperty<string> _name;

      IPropertyStore IPropertyContainer.PropertyStore => PropertyStore;

      public PropertyStore<TestViewModel> PropertyStore { get; set; }
      
      public void Initialize()
      {
        _name = PropertyStore.CreateImplicit<string>(nameof(Name));
      }
      

      public string Name
      {
        get => _name.Value;
        set => _name.Value = value;
      }

      public event PropertyChangedEventHandler PropertyChanged;
      
      public void NotifyPropertyChanged(PropertyChangedEventArgs args)
      {
        PropertyChanged?.Invoke(this, args);
      }
    }

    private static TestViewModel CreateViewModel()
    {
      var vm = new TestViewModel();
      vm.PropertyStore = new PropertyStore<TestViewModel>(vm);
      vm.Initialize();
      return vm;
    }

    private static PropertyErrorsContainer<TestViewModel> CreateContainer(TestViewModel vm)
    {
      var container = new PropertyErrorsContainer<TestViewModel>();
      container.SetParent(vm);
      return container;
    }

    private static PropertyErrorsContainer.PropertyErrors CreatePropertyErrors(string name)
    {
      return new PropertyErrorsContainer.PropertyErrors(name);
    }
    
    private static PropertyErrorsContainer<TestViewModel>.PropertyValidator<T> CreatePropertyValidator<T>(string name, 
      out IPropertyErrorsContainerInternal<TestViewModel> container, 
      out IProperty<T> property,
      Func<TestViewModel, T, string> validateFunc, T defValue = default)
    {
      container = Substitute.For<IPropertyErrorsContainerInternal<TestViewModel>>();
      property = Substitute.For<IProperty<T>>();
      property.NameArgs.Returns(new PropertyChangedEventArgs(name));
      property.Value.Returns(defValue);
      return new PropertyErrorsContainer<TestViewModel>.PropertyValidator<T>(container, property, validateFunc);
    }
    
    private static string ValidateName(TestViewModel vm, string name)
    {
      if (name.IsNullOrEmpty())
      {
        return NullName;
      }
        
      if (name.Length < 3)
      {
        return NameTooShort;
      }

      return null;
    }
  }
}