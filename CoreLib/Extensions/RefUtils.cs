namespace CoreLib.Extensions
{
  using System;
  using System.Threading;

  public class RefUtils
  {
    public static void Dispose<T>(ref T disposable) where T : class, IDisposable
    {
      var d = Interlocked.Exchange(ref disposable, null);
      d?.Dispose();
    }

    public static void DisposeReset(ref IDisposable disposable, IDisposable newValue)
    {
      var d = Interlocked.Exchange(ref disposable, newValue);
      d?.Dispose();
    }
  }
}