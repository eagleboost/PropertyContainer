namespace UnitTests
{
  using System.Collections;
  using System.Linq;
  using CoreLib.Core;
  using CoreLib.UnityExt;
  using NUnit.Framework;
  using Unity;

  public class PropertyDirtyTrackerTests
  {
    [Test]
    public void Task_01_Change_And_Restore_01_Single()
    {
      var vm = CreateViewModel();
      var tracker = CreateDirtyTracker(vm);
      tracker.MarkInitialStates();
      
      vm.Name = "ABC";
      AssertDirty(tracker, nameof(vm.Name));

      vm.Name = null;
      AssertNotDirty(tracker);
    }

    [Test]
    public void Task_01_Change_And_Restore_02_CollectionT()
    {
      var vm = CreateViewModel();
      var tracker = CreateDirtyTracker(vm);
      tracker.MarkInitialStates();
      
      vm.Address = new []{"Address1"};
      AssertDirty(tracker, nameof(vm.Address));

      vm.Address = null;
      AssertNotDirty(tracker);
      
      vm.Address = new []{"Address1"};
      tracker.MarkInitialStates();
      AssertNotDirty(tracker);
      
      vm.Address = new []{"Address1", "Address2"};
      AssertDirty(tracker, nameof(vm.Address));
      vm.Address = new []{"Address1"};
      AssertNotDirty(tracker);
    }

    [Test]
    public void Task_01_Change_And_Restore_03_Collection()
    {
      var vm = CreateViewModel();
      var tracker = CreateDirtyTracker(vm);
      tracker.MarkInitialStates();
      
      vm.PhoneNumber = new []{123};
      AssertDirty(tracker, nameof(vm.PhoneNumber));

      vm.PhoneNumber = null;
      AssertNotDirty(tracker);
      
      vm.PhoneNumber = new []{123};
      tracker.MarkInitialStates();
      AssertNotDirty(tracker);
      
      vm.PhoneNumber = new []{123, 456};
      AssertDirty(tracker, nameof(vm.PhoneNumber));
      vm.PhoneNumber = new []{123};
      AssertNotDirty(tracker);
    }

    [Test]
    public void Task_02_ReInitialize()
    {
      var vm = CreateViewModel();
      var tracker = CreateDirtyTracker(vm);
      tracker.MarkInitialStates();
      
      vm.Name = "ABC";
      Assert.That(tracker.IsDirty);

      tracker.MarkInitialStates();
      Assert.That(!tracker.IsDirty);
      
      vm.Name = "ABC";
      AssertNotDirty(tracker);
    }

    [Test]
    public void Task_03_Injection()
    {
      var vm = CreateViewModel<ViewModelWithDirtyTracker>();
      Assert.That(vm.DirtyTracker != null);
    }
    
    private static TestViewModel CreateViewModel()
    {
      return CreateViewModel<TestViewModel>();
    }
    
    private static T CreateViewModel<T>() where T : class
    {
      var container = new UnityContainer();
      container.AddNewExtension<UnityPropertyContainerExt>();
      container.MarkPropertyContainer<T>();
      
      return container.Resolve<T>();
    }
    
    private static PropertyDirtyTracker CreateDirtyTracker(TestViewModel testViewModel)
    {
      var tracker = new PropertyDirtyTracker();
      tracker.SetParent(testViewModel);
      return tracker;
    }
    
    private static void AssertNotDirty(PropertyDirtyTracker tracker)
    {
      Assert.That(!tracker.IsDirty);
      Assert.That(!tracker.DirtyItems.Cast<string>().Any());
    }

    private static void AssertDirty(PropertyDirtyTracker tracker, string name)
    {
      Assert.That(tracker.IsDirty);
      Assert.That(tracker.DirtyItems.Cast<string>().SequenceEqual(new[] {name}));
    }

    public class TestViewModel
    {
      public virtual IEnumerable PhoneNumber { get; set; }
      
      public virtual string Name { get; set; }
      
      public virtual string[] Address { get; set; }
    }
  }
}