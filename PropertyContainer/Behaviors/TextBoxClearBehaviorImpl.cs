namespace PropertyContainer.Behaviors
{
  using System;
  using System.Diagnostics;
  using System.Diagnostics.CodeAnalysis;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Input;
  using System.Windows.Threading;
  using CoreLib.Core;
  using CoreLib.Extensions;
  using PropertyContainer.Extensions;

  public partial class TextBoxClearBehavior
  {
    private class TextBoxClearBehaviorImpl : IDisposable
    {
      private readonly TextBox _textBox;
      private ActionDisposable _cleanup;
      private NullTextForType _nullText;
      private static readonly Func<BindingExpression, Type> GetSourcePropertyType;

      [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
      [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
      static TextBoxClearBehaviorImpl()
      {
        var assembly = typeof(BindingExpression).Assembly;
        var getWorkerMethod = typeof(BindingExpression).GetMethod("get_Worker", BindingFlags.Instance | BindingFlags.NonPublic);
        var workerType = assembly.GetType("MS.Internal.Data.BindingWorker");
        var getWorker = getWorkerMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(BindingExpression), workerType));
        var getSourcePropertyTypeMethod = workerType.GetMethod("get_SourcePropertyType", BindingFlags.Instance | BindingFlags.NonPublic);
        var getSourcePropertyType = getSourcePropertyTypeMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(workerType, typeof(Type)));
        
        GetSourcePropertyType = expr =>
        {
          var worker = getWorker.DynamicInvoke(expr);
          var result = (Type)getSourcePropertyType.DynamicInvoke(worker);
          return result;
        };
      }
      
      public TextBoxClearBehaviorImpl(TextBox textBox)
      {
        _textBox = textBox;
      }

      public void Start()
      {
        _textBox.Unloaded += HandleTextBoxUnloaded;
        _textBox.PreviewKeyDown += HandleTextPreviewKeyDown;
        CommandManager.AddPreviewExecutedHandler(_textBox, HandleTextBoxCommandPreviewExecuted);
        
        _cleanup = new ActionDisposable();
        _cleanup.Add(() => _textBox.Unloaded -= HandleTextBoxUnloaded);
        _cleanup.Add(() => _textBox.PreviewKeyDown -= HandleTextPreviewKeyDown);
        _cleanup.Add(() => CommandManager.RemovePreviewExecutedHandler(_textBox, HandleTextBoxCommandPreviewExecuted));
      }

      public void Dispose()
      {
        RefUtils.Dispose(ref _cleanup);
      }
      
      private void HandleTextPreviewKeyDown(object sender, KeyEventArgs e)
      {
        var textBox = (TextBox) sender;
         
        if (textBox.Text == null || textBox.SelectionLength != 0 && textBox.SelectionLength < textBox.Text.Length)
        {
          return;
        }

        if (textBox.SelectionLength == textBox.Text.Length)
        {
          if (e.Key == Key.Delete || e.Key == Key.Back)
          {
            SetNullText(textBox);
            e.Handled = true;
          }
          return;
        }

        if (textBox.SelectionLength == 0 && textBox.Text.Length == 1)
        {
          if (textBox.CaretIndex == 0 && e.Key == Key.Delete ||
              textBox.CaretIndex == 1 && e.Key == Key.Back)
          {
            SetNullText(textBox);
            e.Handled = true;
          }
          return;
        }
      }

      private void HandleTextBoxCommandPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
      {
        if (e.Command == ApplicationCommands.Cut)
        {
          var textBox = (TextBox) sender;
          if (textBox.SelectionLength == textBox.Text.Length)
          {
            ClipboardEx.SetText(textBox.Text);
            SetNullText(textBox);
            e.Handled = true;
          }
        }
      }

      private void HandleTextBoxUnloaded(object sender, RoutedEventArgs e)
      {
        Dispose();
      }

      private string NullText
      {
        get
        {
          _nullText ??= GetNullText();
          return _nullText.Text;
        }
      }

      private NullTextForType GetNullText()
      {
        var expr = _textBox.GetBindingExpression(TextBox.TextProperty);
        var type = GetSourcePropertyType(expr);
        return type.IsClass 
          ? new NullTextForType(type, null) 
          : new NullTextForType(type, Activator.CreateInstance(type).ToString());
      }
      
      private void SetNullText(TextBox textBox)
      {
        SetNullText(textBox, NullText);
      }

      private static void SetNullText(TextBox textBox, string text)
      {
        textBox.Dispatcher.BeginInvoke(() => textBox.Text = text);
      }
      
      [DebuggerDisplay("{Type}:<{Text}>")]
      private class NullTextForType
      {
        public NullTextForType(Type type, string text)
        {
          Type = type;
          Text = text;
        }
        
        public readonly Type Type;
        
        public readonly string Text;
      }
    }
  }
}