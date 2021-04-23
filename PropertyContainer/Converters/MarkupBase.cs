namespace PropertyContainer.Converters
{
  using System;
  using System.Windows.Markup;

  public abstract class MarkupBase<T> : MarkupExtension where T : MarkupBase<T>, new()
  {
    public static readonly T Instance = new T();
    
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return Instance;
    }
  }
}