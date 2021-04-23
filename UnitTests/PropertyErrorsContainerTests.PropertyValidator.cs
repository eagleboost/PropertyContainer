namespace UnitTests
{
  using System;
  using System.Linq;
  using CoreLib.Core;
  using CoreLib.Extensions;
  using NSubstitute;
  using NUnit.Framework;

  /// <summary>
  /// PropertyValidatorTests
  /// </summary>
  public partial class PropertyErrorsContainerTests
  {
    public class PropertyValidatorTests
    {
      [Test]
      public void Task_01_Creation_Dispose()
      {
        var validator = CreatePropertyValidator<string>("ABC", out var container, out var _, ValidateName);
        validator.Dispose();

        container.Received().AddValidator(validator);
        container.Received().RemoveValidator(validator);
      }

      [Test]
      [TestCase(default(string))]
      [TestCase("Value")]
      public void Task_02_InitialValidate(string devValue)
      {
        var validator = CreatePropertyValidator("ABC", out var container, out var _, ValidateName, devValue);
        var propertyErrors = validator.PropertyErrors;
        
        EventArgs propertyArgs = null;
        validator.PropertyErrors.ErrorsChanged += (s, e) => propertyArgs = e;

        validator.Validate();

        if (devValue.IsNullOrEmpty())
        {
          container.ReceivedWithAnyArgs().RaiseErrorsChanged(null);
          Assert.That(propertyArgs != null);
          Assert.That(propertyErrors.Errors.First() == NullName);
        }
        else
        {
          container.DidNotReceiveWithAnyArgs().RaiseErrorsChanged(null);
          Assert.That(propertyArgs == null);
        }
      }

      [Test]
      public void Task_03_ValueChange()
      {
        var validator = CreatePropertyValidator<string>("ABC", out var container, out var property, ValidateName);
        validator.Validate();

        var propertyErrors = validator.PropertyErrors;
        
        EventArgs propertyArgs = null;
        propertyErrors.ErrorsChanged += (s, e) => propertyArgs = e;

        ////From null=>"Long name", errors cleared
        property.Value = "Long name";
        property.ValueChanged += Raise.EventWith(property, new ValueChangedArgs<string>(property.NameArgs, null, property.Value));
        
        container.ReceivedWithAnyArgs().RaiseErrorsChanged(null);
        Assert.That(propertyArgs != null);
        Assert.That(!propertyErrors.Errors.Any());
        
        ////From "Long name" to "s", errors added
        propertyArgs = null;
        container.ClearReceivedCalls();
        property.Value = "s";
        property.ValueChanged += Raise.EventWith(property, new ValueChangedArgs<string>(property.NameArgs, null, property.Value));
        
        container.ReceivedWithAnyArgs().RaiseErrorsChanged(null);
        Assert.That(propertyArgs != null);
        Assert.That(propertyErrors.Errors.First() == NameTooShort);
        
        ////From "s" to "ss", no notifications
        propertyArgs = null;
        container.ClearReceivedCalls();
        property.Value = "ss";
        property.ValueChanged += Raise.EventWith(property, CreateValueChangedArgs(property));
        
        container.DidNotReceiveWithAnyArgs().RaiseErrorsChanged(null);
        Assert.That(propertyArgs == null);
        
        ////From "s" to "ss", no notifications
        propertyArgs = null;
        container.ClearReceivedCalls();
        property.Value = "ss";
        property.ValueChanged += Raise.EventWith(property, CreateValueChangedArgs(property));
        
        container.DidNotReceiveWithAnyArgs().RaiseErrorsChanged(null);
        Assert.That(propertyArgs == null);
        
        ////From "ss" to null, errors replaced
        propertyArgs = null;
        container.ClearReceivedCalls();
        property.Value = null;
        property.ValueChanged += Raise.EventWith(property, CreateValueChangedArgs(property));
        
        container.ReceivedWithAnyArgs().RaiseErrorsChanged(null);
        Assert.That(propertyArgs != null);
        Assert.That(propertyErrors.Errors.First() == NullName);
      }

      private ValueChangedArgs<string> CreateValueChangedArgs(IProperty<string> property)
      {
        return new ValueChangedArgs<string>(property.NameArgs, null, property.Value);
      }
    }
  }
}