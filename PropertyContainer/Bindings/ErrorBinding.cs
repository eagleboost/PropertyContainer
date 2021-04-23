namespace PropertyContainer.Bindings
{
  using System;
  using System.Windows;

  public class ErrorBinding : BindingDecoratorBase
  {
    public DependencyProperty Property { get; set; }
    
    public ErrorBinding(string path)
    {
      Path = new PropertyPath(path);
    }

    public override object ProvideValue(IServiceProvider provider)
    {
      if (Property != null && TryGetTargetItems<FrameworkElement>(provider, out var element, out _))
      {
        ErrorInfoBehavior.SetTargetPropertyProperty(element, Property);
      }
      
      return base.ProvideValue(provider);
    }
  }
}