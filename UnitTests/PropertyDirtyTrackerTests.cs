namespace UnitTests
{
  using System;
  using System.Linq;
  using CoreLib.Core;
  using CoreLib.UnityExt;
  using NUnit.Framework;
  using Unity;

  public class PropertyDirtyTrackerTests
  {
    [Test]
    public void Task_01_Creation()
    {
      var vm = CreateViewModel();
      var tracker = CreateDirtyTracker(vm);
      Assert.Throws<InvalidOperationException>(() =>
      {
        var isDirty = tracker.IsDirty;
      });
    }

    [Test]
    public void Task_02_Change_And_Restore()
    {
      var vm = CreateViewModel();
      var tracker = CreateDirtyTracker(vm);
      tracker.MarkInitialStates();
      
      vm.Name = "ABC";
      Assert.That(tracker.IsDirty);
      Assert.That(tracker.DirtyItems.Cast<string>().SequenceEqual(new[] {nameof(vm.Name)}));

      vm.Name = null;
      Assert.That(!tracker.IsDirty);
      Assert.That(!tracker.DirtyItems.Cast<string>().Any());
    }

    [Test]
    public void Task_03_ReInitialize()
    {
      var vm = CreateViewModel();
      var tracker = CreateDirtyTracker(vm);
      tracker.MarkInitialStates();
      
      vm.Name = "ABC";
      Assert.That(tracker.IsDirty);

      tracker.MarkInitialStates();
      Assert.That(!tracker.IsDirty);
      
      vm.Name = "ABC";
      Assert.That(!tracker.IsDirty);
    }

    [Test]
    public void Task_04()
    {
      var vm = CreateViewModel<ViewModelWithDirtyTracker>();
      Assert.That(vm.DirtyTracker != null);
    }
    
    private static ViewModel CreateViewModel()
    {
      return CreateViewModel<ViewModel>();
    }
    
    private static T CreateViewModel<T>() where T : class
    {
      var container = new UnityContainer();
      container.AddNewExtension<UnityPropertyContainerExt>();
      container.MarkPropertyContainer<T>();
      
      return container.Resolve<T>();
    }
    
    private static PropertyDirtyTracker CreateDirtyTracker(ViewModel viewModel)
    {
      var tracker = new PropertyDirtyTracker();
      tracker.SetParent(viewModel);
      return tracker;
    }
  }
}