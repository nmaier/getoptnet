using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using static System.String;

namespace NMaier.GetOptNet
{
  internal abstract class ArgumentHandler
  {
    private static readonly Dictionary<string, bool> booleans =
      new Dictionary<string, bool>(StringComparer.CurrentCultureIgnoreCase)
      {
        { "yes", true },
        { "on", true },
        { "true", true },
        { "1", true },
        { "no", false },
        { "off", false },
        { "false", false },
        { "0", false }
      };

    protected static object InternalConvert([NotNull] string from, [NotNull] Type type)
    {
      if (from == null) {
        throw new ArgumentNullException(nameof(from));
      }

      if (type == null) {
        throw new ArgumentNullException(nameof(type));
      }

      switch (type) {
      case Type t when t == typeof(string):
        return from;
      case Type t when t == typeof(bool):
        return booleans.TryGetValue(from.Trim(), out var b) ? b : bool.Parse(from);
      case Type t when t == typeof(byte):
        return byte.Parse(from);
      case Type t when t == typeof(sbyte):
        return sbyte.Parse(from);
      case Type t when t == typeof(char):
        return char.Parse(from);
      case Type t when t == typeof(short):
        return short.Parse(from);
      case Type t when t == typeof(ushort):
        return ushort.Parse(from);
      case Type t when t == typeof(int):
        return int.Parse(from);
      case Type t when t == typeof(uint):
        return uint.Parse(from);
      case Type t when t == typeof(long):
        return long.Parse(from);
      case Type t when t == typeof(ulong):
        return ulong.Parse(from);
      case Type t when t == typeof(DirectoryInfo):
        return ToDirectoryInfo(from);
      case Type t when t == typeof(FileInfo):
        return ToFileInfo(from);
      }

      try {
        return TypeDescriptor.GetConverter(type).ConvertFromString(from);
      }
      catch (Exception ex) {
        try {
          var ctor = type.GetConstructor(new[]
          {
            typeof(string)
          }) ?? throw new ProgrammingErrorException("No constructor for type");
          return ctor.Invoke(new object[] { from });
        }
        catch (Exception) {
          throw new NotSupportedException(ex.Message);
        }
      }
    }

    private static DirectoryInfo ToDirectoryInfo(string from)
    {
      return new DirectoryInfo(from.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    private static FileInfo ToFileInfo(string from)
    {
      return new FileInfo(from);
    }

    protected readonly object HandledObject;

    protected readonly MemberInfo MemberInfo;

    private readonly bool required;

    protected readonly Type Type;

    protected Type InternalElementType;

    protected bool WasSet;

    protected ArgumentHandler(object handledObject, MemberInfo memberInfo, Type elementType, bool isFlag, bool required)
    {
      IsFlag = isFlag;
      HandledObject = handledObject;
      MemberInfo = memberInfo;
      Type = elementType;
      InternalElementType = Type;
      this.required = required;
    }


    internal Type ElementType => InternalElementType;

    internal MemberInfo Info => MemberInfo;

    internal bool IsFlag { get; }

    internal string Name => MemberInfo.Name;

    protected object InternalConvert(string from)
    {
      return InternalConvert(from, Type);
    }

    internal abstract void Assign(string toAssign);

    internal virtual void Finish()
    {
      if (required && !WasSet) {
        throw new RequiredOptionMissingException(this);
      }

      WasSet = false;
    }

    internal void InternalAssign(object toAssign)
    {
      try {
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (MemberInfo.MemberType) {
        case MemberTypes.Field when MemberInfo is FieldInfo fi:
          fi.SetValue(HandledObject, toAssign);
          break;

        case MemberTypes.Property when MemberInfo is PropertyInfo pi:
          pi.SetValue(HandledObject, toAssign, null);
          break;

        default:
          throw new ProgrammingErrorException("Unsupported member type");
        }
      }
      catch (TargetInvocationException ex) {
        if (ex.InnerException != null) {
          throw ex.InnerException;
        }

        throw;
      }

      WasSet = true;
    }

    internal bool ShouldAssign(ArgumentCollision collision)
    {
      if (!WasSet) {
        return true;
      }

      switch (collision) {
      case ArgumentCollision.Throw:
        throw new DuplicateArgumentException(
          Format(
            CultureInfo.CurrentCulture,
            "Option {0} is specified more than once",
            Name));
      case ArgumentCollision.Ignore:
        return false;
      case ArgumentCollision.Overwrite:
        return true;
      default:
        throw new ArgumentOutOfRangeException(nameof(collision), collision, null);
      }
    }
  }
}
