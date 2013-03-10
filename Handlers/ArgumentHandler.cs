using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal abstract class ArgumentHandler
  {
    private readonly bool isflag;

    private readonly bool required = false;


    protected Type elementType;

    protected MemberInfo info;

    protected object obj;

    protected Type type;

    protected bool wasSet = false;


    public ArgumentHandler(object aObj, MemberInfo aInfo, Type aType, bool aIsFlag, bool aRequired)
    {
      isflag = aIsFlag;
      obj = aObj;
      info = aInfo;
      type = aType;
      elementType = type;
      required = aRequired;
    }


    public Type ElementType
    {
      get {
        return elementType;
      }
    }
    public MemberInfo Info
    {
      get {
        return info;
      }
    }
    public bool IsFlag
    {
      get {
        return isflag;
      }
    }
    public string Name
    {
      get {
        return info.Name;
      }
    }


    protected bool CheckCollision(ArgumentCollision aCollision)
    {
      if (wasSet) {
        switch (aCollision) {
          case ArgumentCollision.Throw:
            throw new DuplicateArgumentException(String.Format(CultureInfo.CurrentCulture, "Option {0} is specified more than once", Name));
          case ArgumentCollision.Ignore:
            return false;
          default:
            return true;
        }
      }

      return true;
    }

    protected void InternalAssign(object toAssign)
    {
      try {
        switch (info.MemberType) {
          case MemberTypes.Field:
            var fi = info as FieldInfo;
            fi.SetValue(obj, toAssign);
            break;

          case MemberTypes.Property:
            var pi = info as PropertyInfo;
            pi.SetValue(obj, toAssign, null);
            break;

          default:
            throw new ProgrammingError("w00t?");
        }
      }
      catch (TargetInvocationException ex) {
        throw ex.InnerException;
      }
      wasSet = true;
    }

    protected object InternalConvert(string from)
    {
      return InternalConvert(from, type);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    protected static object InternalConvert(string from, Type type)
    {
      try {
        return TypeDescriptor.GetConverter(type).ConvertFromString(from);
      }
      catch (Exception ex) {
        try {
          return type.GetConstructor(new Type[] { from.GetType() }).Invoke(new object[] { from });
        }
        catch (Exception) {
          throw new NotSupportedException(ex.Message);
        }
      }
    }


    public abstract void Assign(string toAssign);

    public virtual void Finish()
    {
      if (required && !wasSet) {
        throw new RequiredOptionMissingException(this);
      }
      wasSet = false;
    }
  }
}
