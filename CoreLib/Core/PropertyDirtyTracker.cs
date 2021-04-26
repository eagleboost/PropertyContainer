namespace CoreLib.Core
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using CoreLib.Extensions;

  /// <summary>
  /// PropertyDirtyTracker
  /// </summary>
  public sealed partial class PropertyDirtyTracker : NotifyPropertyChangedBase, IPropertyDirtyTracker, ISubComponent
  {
    #region Declarations
    private List<IPropertyDirtyState> _dirtyStates;
    private IPropertyContainer _container;
    #endregion Declarations
    
    #region ISubComponent
    public void SetParent(object parent)
    {
      _container = (IPropertyContainer) parent;
      MarkInitialStates();
    }
    #endregion ISubComponent
    
    #region IPropertyDirtyTracker
    public bool IsDirty => GetDirtyState();

    public IEnumerable DirtyItems => GetDirtyItems();
    
    public void MarkInitialStates()
    {
      EnsureDirtyStates();

      foreach (var dirtyState in _dirtyStates)
      {
        dirtyState.Initialize();
      }
    }
    #endregion IPropertyDirtyTracker

    #region Private Methods
    private void EnsureDirtyStates()
    {
      if (_dirtyStates == null)
      {
        var properties = _container.PropertyStore.Properties;
        _dirtyStates = new List<IPropertyDirtyState>(properties.Count);
        foreach (var property in properties)
        {
          var dirtyState = CreatePropertyDirtyState(this, property);
          _dirtyStates.Add(dirtyState);
        }
      }
    }

    private static IPropertyDirtyState CreatePropertyDirtyState(PropertyDirtyTracker tracker, IProperty property)
    {
      var type = property.Type;
      var dirtyStateType = GetDirtyStateType(type);
      var dirtyState = (IPropertyDirtyState)Activator.CreateInstance(dirtyStateType, tracker, property);
      return dirtyState;
    }

    private static Type GetDirtyStateType(Type propertyType)
    {
      if (propertyType != typeof(string) && propertyType.IsSubclassOf<IEnumerable>())
      {
        var elementType = GetElementType(propertyType);
        if (elementType != null)
        {
          return typeof(CollectionDirtyStateT<,>).MakeGenericType(propertyType, elementType);
        }

        return typeof(CollectionDirtyState<>).MakeGenericType(propertyType);
      }
      
      return typeof(SingleDirtyState<>).MakeGenericType(propertyType);
    }
    
    private static Type GetElementType(Type type)
    {
      var result = GetElementTypeCore(type);
      if (result == null)
      {
        foreach (var intf in type.GetInterfaces())
        {
          result = GetElementTypeCore(intf);
          if (result != null)
          {
            return result;
          }
        }
      }

      return result;
    }
    
    private static Type GetElementTypeCore(Type type)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
      {
        return type.GetGenericArguments()[0];
      }
      
      return null;
    }
    
    private List<IPropertyDirtyState> GetDirtyStates()
    {
      var dirtyStates = _dirtyStates ?? throw new InvalidOperationException("Please call MarkInitialStates() first");
      return dirtyStates;
    }
    
    private bool GetDirtyState()
    {
      var dirtyStates = GetDirtyStates();
      return dirtyStates.Any(i => i.IsDirty);
    }

    private IEnumerable<string> GetDirtyItems()
    {
      var dirtyStates = GetDirtyStates();
      return dirtyStates.Where(i => i.IsDirty).Select(i => i.Name);
    }
    
    private void OnPropertyValueChanged<TProperty>(ValueChangedArgs<TProperty> e)
    {
      NotifyPropertyChanged(DirtyStateArgs.IsDirty);
      NotifyPropertyChanged(DirtyStateArgs.DirtyItems);
    }
    #endregion Private Methods
  }
}