namespace CoreLib.Core
{
  using System.Collections.Generic;

  /// <summary>
  /// PropertyDirtyState
  /// </summary>
  public partial class PropertyDirtyTracker
  {
    internal interface IPropertyDirtyState : IDirtyState
    {
      string Name { get; }
      
      void Initialize();
    }
    
    internal abstract class PropertyDirtyState<T> : IPropertyDirtyState
    {
      protected PropertyDirtyState(PropertyDirtyTracker tracker, IProperty<T> property)
      {
        Tracker = tracker;
        Property = property;
        Name = property.NameArgs.PropertyName;
        property.ValueChanged += HandlePropertyValueChanged;
      }

      public readonly PropertyDirtyTracker Tracker;
      
      public  readonly IProperty<T> Property;

      public bool IsDirty => GetDirtyState();

      public string Name { get; }

      public abstract void Initialize();

      private void HandlePropertyValueChanged(object sender, ValueChangedArgs<T> e)
      {
        Tracker.OnPropertyValueChanged(e);
      }

      protected abstract bool GetDirtyState();
    }
  }
}