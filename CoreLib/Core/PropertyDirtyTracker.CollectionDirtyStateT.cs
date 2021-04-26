namespace CoreLib.Core
{
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
    /// <typeparam name="TElement"></typeparam>
    internal class CollectionDirtyStateT<T, TElement> : PropertyDirtyState<T> where T : IEnumerable<TElement>
    {
      private List<TElement> _value;

      public CollectionDirtyStateT(PropertyDirtyTracker tracker, IProperty<T> property) : base(tracker, property)
      {
      }

      public override void Initialize()
      {
        if (Property.Value != null)
        {
          _value = Property.Value.ToList();
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

        return !Property.Value.SequenceEqual(_value);
      }
    }
  }
}