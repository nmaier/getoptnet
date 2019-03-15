using System;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class CounterArgumentHandler : ArgumentHandler
  {
    private long current;

    public CounterArgumentHandler(object handledObject, MemberInfo memberInfo, Type elementType, bool required)
      : base(handledObject, memberInfo, elementType, true, required)
    {
    }


    internal override void Assign(string toAssign)
    {
      current += 1;
    }

    internal override void Finish()
    {
      InternalAssign((current as IConvertible).ToType(Type, null));
      base.Finish();
      current = 0;
    }
  }
}
