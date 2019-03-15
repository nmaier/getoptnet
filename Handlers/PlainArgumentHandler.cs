using System;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class PlainArgumentHandler : ArgumentHandler
  {
    private readonly ArgumentCollision collision;

    public PlainArgumentHandler(object handledObject, MemberInfo memberInternalInfo, Type elementType,
      ArgumentCollision collision, bool required)
      : base(handledObject, memberInternalInfo, elementType, false, required)
    {
      this.collision = collision;
    }


    internal override void Assign(string toAssign)
    {
      if (ShouldAssign(collision)) {
        InternalAssign(InternalConvert(toAssign));
      }
    }
  }
}
