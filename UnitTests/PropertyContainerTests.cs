namespace UnitTests
{
  using System;
  using System.Linq;
  using CoreLib.Core;
  using CoreLib.Extensions;
  using CoreLib.UnityExt;
  using NUnit.Framework;
  using Unity;

  public class PropertyContainerTests
  {
    [Test]
    public void Task_01_Resolve_01_Not_Marked()
    {
      var container = new UnityContainer();
      container.AddNewExtension<UnityPropertyContainerExt>();
      
      var vm = container.Resolve<ViewModel>();
      Assert.That(vm.GetType() == typeof(ViewModel));
    }
    
    [Test]
    public void Task_01_Resolve_02_Marked()
    {
      CreateViewModel<ViewModel>(out var vm);
      Assert.That(vm is ViewModel);
      Assert.That(vm.GetType() != typeof(ViewModel));
      Assert.That(vm.GetType().Name == $"{nameof(ViewModel)}_<PropertyContainer>_Impl");
    }
    
    [Test]
    public void Task_01_Resolve_03_Type_Is_Already_Dynamic()
    {
      var container = CreateViewModel<ViewModel>(out var vm);
      var type = vm.GetType();
      var vm2 = container.Resolve(type);
      Assert.That(vm.GetType() == vm2.GetType());
    }
 
    [Test]
    public void Task_01_Resolve_04_Already_Registered()
    {
      var container = new UnityContainer();
      container.AddNewExtension<UnityPropertyContainerExt>();
      container.MarkPropertyContainer<ViewModel>();
      container.RegisterType<ViewModel>();

      var vm = container.Resolve<ViewModel>();
      Assert.That(vm.GetType() == typeof(ViewModel));
    }
    
    [Test]
    public void Task_01_Resolve_05_Has_Ctor()
    {
      Assert.Throws<InvalidOperationException>(() => CreateViewModel<ViewModelWithConstructor>(out var vm));
    }

    [Test]
    public void Task_01_Resolve_06_Has_Dependencies()
    {
      CreateViewModel<ViewModelWithDependencies>(out var _);
    }

    [Test]
    public void Task_02_PropertyChange_01_Normal()
    {
      CreateViewModel<ViewModel>(out var vm);

      ValueChangedArgs<string> nameArgs = null;
      var dName = vm.HookChange(v => v.Name, (_, e) => nameArgs = e);

      ValueChangedArgs<int> ageArgs = null;
      var dAge = vm.HookChange(v => v.Age, (_, e) => ageArgs = e);

      string property = null;
      var dAll = vm.HookChange((_, e) => property = e.PropertyName);

      vm.Name = null;
      Assert.That(nameArgs == null);
      
      vm.Name = "ABC";
      Assert.That(nameArgs != null);
      Assert.That(nameArgs.PropertyArgs.PropertyName == "Name");
      Assert.That(nameArgs.OldValue == null);
      Assert.That(nameArgs.NewValue == "ABC");
      Assert.That(nameArgs.ToString() == "Name: <null>=>ABC");
      Assert.That(vm.Name == "ABC");
      Assert.That(property == "Name");

      nameArgs = null;
      vm.Age = 123;
      Assert.That(nameArgs == null);
      Assert.That(ageArgs.ToString() == "Age: 0=>123");
      Assert.That(property == "Age");

      dName.Dispose();
      dAge.Dispose();
      dAll.Dispose();
    }

    [Test]
    public void Task_02_PropertyChange_02_NonVirtual()
    {
      CreateViewModel<ViewModel>(out var vm);

      ValueChangedArgs<string> nonVirtualNameArgs = null;
      vm.HookChange(v => v.NonVirtualName, (_, e) => nonVirtualNameArgs = e);
      vm.NonVirtualName = "DEF";
      
      Assert.That(nonVirtualNameArgs == null);
    }

    [Test]
    public void Task_03_Property_Interface()
    {
      CreateViewModel<ViewModel>(out var vm);
      var properties = vm.GetProperties();
      foreach (var p in properties)
      {
        Console.WriteLine($"{p.NameArgs.PropertyName}: {p.Type}, {p.Value}");
      }

      var property = properties.First(v => v.NameArgs.PropertyName == "Name");
      Assert.That(property.ToString() == $"{vm.GetType()} Name:{property.Value.ToStringEx()}");

      ValueChangedArgs<object> nameArgs = null;
      using (var dName = property.HookChange((_, e) => nameArgs = e))
      {
        property.Value = "ABC";

        Assert.That(nameArgs.PropertyArgs.PropertyName == "Name");
        Assert.That(nameArgs.OldValue == null);
        Assert.That((string)nameArgs.NewValue == "ABC");
        Assert.That(nameArgs.ToString() == "Name: <null>=>ABC");
      }
    }

    private static IUnityContainer CreateViewModel<T>(out T viewModel) where T : class
    {
      var container = new UnityContainer();
      container.AddNewExtension<UnityPropertyContainerExt>();
      container.MarkPropertyContainer<T>();

      viewModel = container.Resolve<T>();
      return container;
    }
  }
}