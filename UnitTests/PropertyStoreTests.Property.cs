namespace UnitTests
{
  using System.ComponentModel;
  using CoreLib.Core;
  using NUnit.Framework;

  public partial class PropertyStoreTests
  {
    public class PropertyTests
    {
      [Test]
      [TestCase(default(string))]
      [TestCase("Value")]
      public void Task_01_Creation(string defValue)
      {
        var vm = new TestViewModel();
        var property = new Property<TestViewModel, string>(vm, new PropertyChangedEventArgs("ABC"), defValue);
        Assert.That(property.Parent == vm);
        Assert.That(property.Value == defValue);
      }
      
      [Test]
      public void Task_02_ValueChanged()
      {
        var vm = new TestViewModel();
        var property = new Property<TestViewModel, string>(vm, new PropertyChangedEventArgs("ABC"));

        ValueChangedArgs<string> args = null;
        void OnValueChanged(object sender, ValueChangedArgs<string> e)
        {
          args = e;
        }

        using (property.HookChange(OnValueChanged))
        {
          property.Value = "Value";
          Assert.That(property.Value == "Value");
          Assert.That(args.NewValue == "Value");
          Assert.That(args.PropertyArgs == property.NameArgs);
          Assert.That(args.ToString() == $"ABC: <null>=>Value");
        }
      }
      
      [Test]
      public void Task_03_Interface()
      {
        var vm = new TestViewModel();
        IProperty property = new Property<TestViewModel, string>(vm, new PropertyChangedEventArgs("ABC"));

        Assert.That(property.Parent == vm);
        Assert.That(property.Type == typeof(string));
        
        ValueChangedArgs<object> args = null;
        void OnValueChanged(object sender, ValueChangedArgs<object> e)
        {
          args = e;
        }

        using (property.HookChange(OnValueChanged))
        {
          property.Value = "Value";
          Assert.That((string) property.Value == "Value");
          Assert.That((string) args.NewValue == "Value");
          Assert.That(args.PropertyArgs == property.NameArgs);
          Assert.That(property.ToString() == $"TestViewModel ABC:Value");
        }
      }
    }
  }
}