using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class EnumArgumentHandler : ArgumentHandler
  {
    private readonly ArgumentCollision collision;
    private readonly Dictionary<string, string> names;

    public EnumArgumentHandler(object handledObject, MemberInfo memberInfo, Type elementType,
      ArgumentCollision collision, bool required)
      : base(handledObject, memberInfo, elementType, false, required)
    {
      this.collision = collision;
      names = (from e in Enum.GetNames(elementType)
               select new { Key = e.ToLowerInvariant(), Value = e }).ToDictionary(v => v.Key, v => v.Value);
    }

    internal override void Assign(string toAssign)
    {
      try {
        if (!ShouldAssign(collision)) {
          return;
        }

        toAssign = names[toAssign.ToLowerInvariant()];
        InternalAssign(Enum.Parse(ElementType, toAssign));
      }
      catch (Exception) {
        var allowed = GetOpt.FormatEnum(ElementType);
        throw new NotSupportedException($"Invalid value {toAssign}, must be one of {allowed}");
      }
    }
  }
}
