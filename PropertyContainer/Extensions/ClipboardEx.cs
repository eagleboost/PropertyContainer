namespace PropertyContainer.Extensions
{
  using System.Windows;
  using CoreLib.Extensions;

  /// <summary>
  /// ClipboardEx
  /// </summary>
  public static class ClipboardEx
  {
    public static void SetText(string text)
    {
      if (text.HasValue())
      {
        Clipboard.SetText(text);
      }
    }
  }
}