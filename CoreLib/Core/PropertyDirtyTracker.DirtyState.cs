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
    
    internal class PropertyDirtyState<TProperty> : IPropertyDirtyState
    {
      private readonly PropertyDirtyTracker _tracker;
      private readonly IProperty<TProperty> _property;
      private TProperty _value;

      public PropertyDirtyState(PropertyDirtyTracker tracker, IProperty<TProperty> property)
      {
        _tracker = tracker;
        _property = property;
        Name = property.NameArgs.PropertyName;
        property.ValueChanged += HandlePropertyValueChanged;
      }

      public bool IsDirty => GetDirtyState();

      public string Name { get; }

      public void Initialize()
      {
        _value = _property.Value;
      }

      private void HandlePropertyValueChanged(object sender, ValueChangedArgs<TProperty> e)
      {
        _tracker.OnPropertyValueChanged(e);
      }

      private bool GetDirtyState()
      {
        if (EqualityComparer<TProperty>.Default.Equals(_value, _property.Value))
        {
          return false;
        }

        return true;
      }
    }
  }
}