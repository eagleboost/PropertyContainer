namespace CoreLib.Core
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// PropertyDirtyState
  /// </summary>
  public partial class PropertyDirtyTracker
  {
    /// <summary>
    /// Compare collections, by reference or by elements in sequence
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CollectionDirtyState<T> : PropertyDirtyState<T> where T : IEnumerable
    {
      private object[] _value;

      public CollectionDirtyState(PropertyDirtyTracker tracker, IProperty<T> property) : base(tracker, property)
      {
      }

      private IEnumerable<object> GetValueSequence()
      {
        return Property.Value.Cast<object>();
      }
      
      public override void Initialize()
      {
        if (Property.Value != null)
        {
          _value = GetValueSequence().ToArray();
        }
      }

      protected override bool GetDirtyState()
      {
        if (Property.Value == null)
        {
          return _value != null;
        }

        if (_value == null)
        {
          return true;
        }

        return !GetValueSequence().SequenceEqual(_value);
      }
    }
  }
}