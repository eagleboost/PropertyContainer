namespace CoreLib.UnityExt
{
  using Unity;

  public static class UnityContainerExt
  {
    public static void MarkPropertyContainer<T>(this IUnityContainer container) where T : class
    {
      container.RegisterType<PropertyContainerTypeKey<T>>();
    }
  }
}