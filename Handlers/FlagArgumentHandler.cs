using System;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class FlagArgumentHandler : ArgumentHandler
  {
    private readonly ArgumentCollision collision;

    private readonly bool whenSet = true;


    public FlagArgumentHandler(Object aObj, MemberInfo aInfo, ArgumentCollision aCollision, bool aRequired, Boolean aWhenSet)
      : base(aObj, aInfo, typeof(bool), true, aRequired)
    {
      collision = aCollision;
      whenSet = aWhenSet;
    }


    public override void Assign(string toAssign)
    {
      if (CheckCollision(collision)) {
        InternalAssign(whenSet);
      }
    }

    public override void Finish()
    {
      if (!wasSet) {
        InternalAssign(!whenSet);
      }
      base.Finish();
    }
  }
}
