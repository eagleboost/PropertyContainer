namespace UnitTests
{
  using System.ComponentModel;
  using System.Linq;
  using CoreLib.Core;
  using NUnit.Framework;

  public partial class PropertyStoreTests
  {
    [Test]
    public void Task_01_GetProperty()
    {
      var vm = new TestViewModel();
      var store = CreateStore(vm);
      Assert.That(store.GetProperty("abc") == null);
      
      store.Create("abc", out var property, "value");
      Assert.That(property.NameArgs.PropertyName == "abc");
      Assert.That(property.Value == "value");
      
      Assert.That(store.GetProperty("abc") == property);
      Assert.That(store.Properties.Count == 1);
      Assert.That(store.Properties.First() == property);
    }

    [Test]
    public void Task_02_PropertyChange()
    {
      var vm = new TestViewModel();
      
      PropertyChangedEventArgs args = null;
      vm.PropertyChanged += (s, e) => args = e;
      
      var store = CreateStore(vm);
     
      store.Create("abc", out var property, "value");

      property.Value = "value1";
      
      Assert.That(args!= null);
      Assert.That(args.PropertyName == "abc");
    }

    private class TestViewModel : INotifyPropertyChanged, IPropertyChangeNotifiable
    {
      public event PropertyChangedEventHandler PropertyChanged;
      
      public void NotifyPropertyChanged(PropertyChangedEventArgs args)
      {
        PropertyChanged?.Invoke(this, args);
      }

      public override string ToString()
      {
        return "TestViewModel";
      }
    }

    private PropertyStore<TestViewModel> CreateStore(TestViewModel vm)
    {
      return new PropertyStore<TestViewModel>(vm);
    }
  }
}