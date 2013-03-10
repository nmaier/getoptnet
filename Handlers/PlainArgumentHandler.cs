using System;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class PlainArgumentHandler : ArgumentHandler
  {
    private readonly ArgumentCollision collision;


    public PlainArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, ArgumentCollision aCollision, bool aRequired)
      : base(aObj, aInfo, aType, false, aRequired)
    {
      collision = aCollision;
    }


    public override void Assign(string toAssign)
    {
      if (CheckCollision(collision)) {
        InternalAssign(InternalConvert(toAssign));
      }
    }
  }
}
