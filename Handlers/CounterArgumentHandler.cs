using System;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class CounterArgumentHandler : ArgumentHandler
  {
    private Int64 current = 0;


    public CounterArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, bool aRequired)
      : base(aObj, aInfo, aType, true, aRequired)
    {
    }


    public override void Assign(string toAssign)
    {
      current += 1;
    }

    public override void Finish()
    {
      InternalAssign((current as IConvertible).ToType(type, null));
      base.Finish();
      current = 0;
    }
  }
}
