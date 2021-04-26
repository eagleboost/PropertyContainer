namespace CoreLib.Core
{
  using System.Collections.Generic;

  /// <summary>
  /// PropertyDirtyState
  /// </summary>
  public partial class PropertyDirtyTracker
  {
    /// <summary>
    /// Compare single value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SingleDirtyState<T> : PropertyDirtyState<T>
    {
      private T _value;

      public SingleDirtyState(PropertyDirtyTracker tracker, IProperty<T> property) : base(tracker, property)
      {
      }

      public override void Initialize()
      {
        _value = Property.Value;
      }

      protected override bool GetDirtyState()
      {
        if (EqualityComparer<T>.Default.Equals(_value, Property.Value))
        {
          return false;
        }

        return true;
      }
    }
  }
}