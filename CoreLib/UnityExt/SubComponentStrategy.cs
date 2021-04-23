namespace CoreLib.UnityExt
{
  using System;
  using System.Runtime.CompilerServices;
  using CoreLib.Core;
  using CoreLib.Extensions;
  using Unity.Builder;
  using Unity.Strategies;

  public class SubComponentStrategy : BuilderStrategy
  {
    public override void PostBuildUp(ref BuilderContext context)
    {
      base.PostBuildUp(ref context);

      if (context.Type.IsSubclassOf<ISubComponent>())
      {
        if (context.Parent != IntPtr.Zero)
        {
          unsafe
          {
            var parentContext = Unsafe.AsRef<BuilderContext>(context.Parent.ToPointer());
            var parent = parentContext.Existing;
            var component = (ISubComponent)context.Existing;
            component.SetParent(parent);
          }
        }
      }
    }
  }
}