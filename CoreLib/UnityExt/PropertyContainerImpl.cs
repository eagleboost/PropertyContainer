namespace CoreLib.UnityExt
{
  using System;
  using System.Collections.Concurrent;
  using System.ComponentModel;
  using System.Linq;
  using System.Reflection;
  using System.Reflection.Emit;
  using CoreLib.Core;

  public static partial class PropertyContainerImpl
  {
    private static readonly ConcurrentDictionary<Type, Type> TypeMap = new ConcurrentDictionary<Type, Type>();

    public static Type GetImplType(Type baseType)
    {
      ValidateType(baseType);

      return TypeMap.GetOrAdd(baseType, CreateImplType);
    }

    private static void ValidateType(Type type)
    {
      var constructors = type.GetConstructors();
      if (constructors.Any(c => c.GetParameters().Length > 0))
      {
        throw new InvalidOperationException($"{type} can only have parameterless constructor");
      }
    }
    
    /*
    public class BaseType
    {
      public virtual string Name { get; set; }
    }
     
    public class BaseType_<PropertyContainer>_Impl : BaseType, IPropertyContainer, IPropertyChangeNotifiable, INotifyPropertyChanged
    {
      private PropertyStore<BaseType_<PropertyContainer>_Impl> _store;
      private IProperty<string> _name;

      public BaseType_<PropertyContainer>_Impl()
      {
        _store = new PropertyStore<BaseType_<PropertyContainer>_Impl>(this);
        _name = _store.Create<String>("Name");
      }
        
      public IPropertyStore PropertyStore => _store;
      
      public event PropertyChangedEventHandler PropertyChanged;

      public void NotifyPropertyChanged(PropertyChangedEventArgs args)
      {
        PropertyChanged?.Invoke(this, args);
      }
        
      public override string Name
      { 
        get => _name.Value; 
        set => _name.Value = value; 
      }
    }
    */
    
    private static Type CreateImplType(Type baseType)
    {
      var assemblyName = new AssemblyName {Name = "<PropertyContainer>_Assembly"};
      var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
      var moduleBuilder = assemblyBuilder.DefineDynamicModule("<PropertyContainer>_Module");
      var typeName = GetImplTypeName(baseType);
      
      ////IPropertyContainerMarker = IPropertyContainer, IPropertyChangeNotifiable, INotifyPropertyChanged
      var interfaces = new[] {typeof(IPropertyContainerMarker)};
      var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public, baseType, interfaces);

      ////Emit the PropertyChanged event and return its field so we can use it to implement the NotifyPropertyChanged method
      var eventFieldBuilder = EmitPropertyChangedField(typeBuilder);
      EmitNotifyPropertyChanged(typeBuilder, eventFieldBuilder);

      ////Emit the constructor and return the ILGenerator, it will be used to emit codes to create the IProperty<T>
      ////backing fields in the ctor
      var ctorIl = EmitCtor(typeBuilder, baseType);
      
      ////Emit the PropertyStore<T> field and corresponding property getter
      //// _store = new PropertyStore<BaseType_<PropertyContainer>_Impl>(this);
      var storeFieldBuilder = EmitPropertyStore(typeBuilder, ctorIl);
      
      ////This would be used to Emit codes like below to create backing fields for each property
      //// _name = _store.CreateImplicit<String>("Name");
      var createPropertyMethod = typeof(PropertyStore<>).GetMethod("CreateImplicit", BindingFlags.Instance | BindingFlags.Public);

      ////Prepare the properties need to be overridden and generate property getter and setter
      var targetProperties = GetTargetProperties(baseType);
      foreach (var property in targetProperties)
      {
        ////type of string
        var propertyType = property.PropertyType;
        var propertyName = property.Name;
        var fieldName = ToFieldName(propertyName);
        ////type of IProperty<string>
        var fieldType = typeof(IProperty<>).MakeGenericType(propertyType);
        var fieldBuilder = typeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Private);

        var fieldInfo = new PropertyFieldInfo(propertyName, propertyType, fieldBuilder);
          
        var valueProperty = fieldType.GetProperty("Value");
        var setValueMethod = valueProperty.GetSetMethod();
        var getValueMethod = valueProperty.GetGetMethod();

        ////private IProperty<string> _name;
        EmitPropertyBackingField(ctorIl, storeFieldBuilder, createPropertyMethod, fieldInfo);
        ////get => _name.Value; 
        EmitPropertyGetter(typeBuilder, property, fieldBuilder, getValueMethod);
        ////set => _name.Value = value; 
        EmitPropertySetter(typeBuilder, property, fieldBuilder, setValueMethod);
      }
      
      ctorIl.Emit(OpCodes.Ret);
      
      return typeBuilder.CreateType();
    }

    private static ILGenerator EmitCtor(TypeBuilder tb, Type baseType)
    {
      var ctorBuilder = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
      var ctorIl = ctorBuilder.GetILGenerator();

      var baseCtor = baseType.GetConstructors()[0];

      ////IL_0000: ldarg.0
      ctorIl.Emit(OpCodes.Ldarg_0);
      ////IL_0001: call instance void BaseType::.ctor()
      ctorIl.Emit(OpCodes.Call, baseCtor);

      return ctorIl;
    }
    
    private static FieldBuilder EmitPropertyStore(TypeBuilder tb, ILGenerator ctorIl)
    {
      var storeOpenType = typeof(PropertyStore<>);
      var storeType = storeOpenType.MakeGenericType(tb);
      var storeFieldBuilder = tb.DefineField("_propertyStore", storeType, FieldAttributes.Private);
      
      ////Get the constructor of PropertyStore<T> that is dependent on tb;
      var storeCtor = TypeBuilder.GetConstructor(storeType, storeOpenType.GetConstructors()[0]);

      //IL_0006: ldarg.0
      ctorIl.Emit(OpCodes.Ldarg_0);
      //IL_0006: ldarg.0
      ctorIl.Emit(OpCodes.Ldarg_0);
      //IL_0007: newobj instance void class PropertyStore`1<class C>::.ctor(!0)
      ctorIl.Emit(OpCodes.Newobj, storeCtor);
      //IL_000c: stfld class PropertyStore`1<class C> C::_store
      ctorIl.Emit(OpCodes.Stfld, storeFieldBuilder);

      var getPropertyStore = tb.DefineMethod("get_PropertyStore", DefaultMethodAttributes, typeof(IPropertyStore), Type.EmptyTypes);
      var getIl = getPropertyStore.GetILGenerator();
      
      //IL_0000: ldarg.0
      getIl.Emit(OpCodes.Ldarg_0);
      //IL_0001: ldfld class PropertyStore`1<class C> C::_store
      getIl.Emit(OpCodes.Ldfld, storeFieldBuilder);
      //IL_0006: ret
      getIl.Emit(OpCodes.Ret);

      return storeFieldBuilder;
    }

    private static void EmitPropertyBackingField(ILGenerator ctorIl, FieldBuilder storeFieldBuilder, MethodInfo createPropertyMethod, PropertyFieldInfo fieldInfo)
    {
      var propertyName = fieldInfo.PropertyName;
      var fieldBuilder = fieldInfo.Builder;
      var propertyType = fieldInfo.PropertyType;
      var createMethod = TypeBuilder.GetMethod(storeFieldBuilder.FieldType, createPropertyMethod).MakeGenericMethod(propertyType);
      
      //IL_0011: ldarg.0  
      ctorIl.Emit(OpCodes.Ldarg_0);////Load this
      //IL_0012: ldarg.0  
      ctorIl.Emit(OpCodes.Ldarg_0); ////Load this again
      //IL_0013: ldfld class PropertyStore`1<class C> C::_store
      ctorIl.Emit(OpCodes.Ldfld, storeFieldBuilder); ////Load this._propertyStore
      //IL_0018: ldstr "Name"
      ctorIl.Emit(OpCodes.Ldstr, propertyName); ////Load "Name" as the name of the property

      //IL_001e: callvirt instance class IProperty`1<!!0> class PropertyStore`1<class C>::Create<string>(string, !!0)
      ctorIl.Emit(OpCodes.Callvirt, createMethod);  ////call _store.Create<String>("Name");
      //IL_0023: stfld class IProperty`1<string> C::_name
      ctorIl.Emit(OpCodes.Stfld, fieldBuilder); ////_name = _store.Create<String>("Name");
    }
    
    private static MethodBuilder CreateSetterBuilder(TypeBuilder tb, PropertyInfo property)
    {
      var name = property.Name;
      var type = property.PropertyType;
      var result = tb.DefineMethod($"set_{name}", DefaultMethodAttributes, null, new[] {type});
      return result;
    }
    
    private static MethodBuilder CreateGetterBuilder(TypeBuilder tb, PropertyInfo property)
    {
      var name = property.Name;
      var type = property.PropertyType;
      var result = tb.DefineMethod($"get_{name}", DefaultMethodAttributes, type, Type.EmptyTypes);
      return result;
    }

    /*
    public string Name { set => _name.Value = value; }    
    */
    private static void EmitPropertySetter(TypeBuilder tb, PropertyInfo property, FieldBuilder fieldBuilder, MethodInfo setValueMethod)
    {
      var setterBuilder = CreateSetterBuilder(tb, property);
      var setIl = setterBuilder.GetILGenerator();

      //IL_0000: ldarg.0
      setIl.Emit(OpCodes.Ldarg_0); //// Load 'this'
      //IL_0001: ldfld class IProperty`1<string> C::_name
      setIl.Emit(OpCodes.Ldfld, fieldBuilder); //// Load the address of 'this._name'
      //IL_0006: ldarg.1
      setIl.Emit(OpCodes.Ldarg_1); //// Load 'value'
      //IL_0007: callvirt instance void class IProperty`1<string>::set_Value(!0)
      setIl.Emit(OpCodes.Callvirt, setValueMethod); ////_name.set_Value(value)
      setIl.Emit(OpCodes.Ret);
    }
    
    /*
    public string Name { get => _name.Value; }    
    */
    private static void EmitPropertyGetter(TypeBuilder tb, PropertyInfo property, FieldBuilder fieldBuilder, MethodInfo getValueMethod)
    {
      var getterBuilder = CreateGetterBuilder(tb, property);
      var getIl = getterBuilder.GetILGenerator();

      //IL_0000: ldarg.0
      getIl.Emit(OpCodes.Ldarg_0); //// Load 'this'
      //IL_0001: ldfld class IProperty`1<string> C::_name
      getIl.Emit(OpCodes.Ldfld, fieldBuilder); //// Load the address of 'this._name'
      //IL_0006: callvirt instance !0 class IProperty`1<string>::get_Value()
      getIl.Emit(OpCodes.Callvirt, getValueMethod); ////_name.get_Value()
      getIl.Emit(OpCodes.Ret);
    }
    
    private static FieldBuilder EmitPropertyChangedField(TypeBuilder tb)
    {
      var eventFieldBuilder = tb.DefineField("PropertyChanged", EventHandlerType, FieldAttributes.Public);
      var eventBuilder = tb.DefineEvent("PropertyChanged", EventAttributes.None, EventHandlerType);
      
      eventBuilder.SetAddOnMethod(CreateAddMethod(tb, eventFieldBuilder));
      eventBuilder.SetRemoveOnMethod(CreateRemoveMethod(tb, eventFieldBuilder));

      return eventFieldBuilder;
    }
    
    private static MethodBuilder CreateAddMethod(TypeBuilder tb, FieldBuilder eventFieldBuilder)
    {
      return CreateAddRemoveMethodCore(tb, eventFieldBuilder, AddPropertyChangedMethod);
    }

    private static MethodBuilder CreateRemoveMethod(TypeBuilder tb, FieldBuilder eventFieldBuilder)
    {
      return CreateAddRemoveMethodCore(tb, eventFieldBuilder, RemovePropertyChangedMethod);
    }

    private static MethodBuilder CreateAddRemoveMethodCore(TypeBuilder typeBuilder, FieldBuilder eventFieldBuilder, MethodInfo addRemoveMethod)
    {
      var methodName = addRemoveMethod.Name;
      var method = typeBuilder.DefineMethod(methodName, AddRemoveMethodAttributes, null, new[] { EventHandlerType });
      method.SetImplementationFlags(EventMethodImplFlags);
 
      var ilGen = method.GetILGenerator();

      var delegateAction = addRemoveMethod == AddPropertyChangedMethod ? "Combine" : "Remove";
      var dlMethod = typeof(Delegate).GetMethod(delegateAction, new[] {typeof(Delegate), typeof(Delegate)});
      
      //// PropertyChanged += value;
      //// PropertyChanged -= value;
      ilGen.Emit(OpCodes.Ldarg_0);
      ilGen.Emit(OpCodes.Ldarg_0);
      ilGen.Emit(OpCodes.Ldfld, eventFieldBuilder);
      ilGen.Emit(OpCodes.Ldarg_1);
      ilGen.EmitCall(OpCodes.Call, dlMethod, null);
      ilGen.Emit(OpCodes.Castclass, EventHandlerType);
      ilGen.Emit(OpCodes.Stfld, eventFieldBuilder);
      ilGen.Emit(OpCodes.Ret);
 
      typeBuilder.DefineMethodOverride(method, addRemoveMethod);
 
      return method;
    }
    
    private static MethodBuilder EmitNotifyPropertyChanged(TypeBuilder typeBuilder, FieldBuilder eventFieldBuilder)
    {
      var methodBuilder = typeBuilder.DefineMethod("NotifyPropertyChanged", DefaultMethodAttributes, null, new Type[] { typeof(PropertyChangedEventArgs) });
 
      var methodIl = methodBuilder.GetILGenerator();
      
      var labelExit = methodIl.DefineLabel();
 
      // if (PropertyChanged == null)
      // {
      //      return;
      // }
      methodIl.Emit(OpCodes.Ldarg_0);
      methodIl.Emit(OpCodes.Ldfld, eventFieldBuilder);
      methodIl.Emit(OpCodes.Ldnull);
      methodIl.Emit(OpCodes.Ceq);
      methodIl.Emit(OpCodes.Brtrue, labelExit);
 
      // this.PropertyChanged(this,PropertyChangedEventArgs);
      methodIl.Emit(OpCodes.Ldarg_0);
      methodIl.Emit(OpCodes.Ldfld, eventFieldBuilder);
      methodIl.Emit(OpCodes.Ldarg_0);
      methodIl.Emit(OpCodes.Ldarg_1);
      methodIl.EmitCall(OpCodes.Callvirt, EventHandlerType.GetMethod("Invoke"), null);
 
      // return;
      methodIl.MarkLabel(labelExit);
      methodIl.Emit(OpCodes.Ret);
 
      return methodBuilder;
    }
  }
}