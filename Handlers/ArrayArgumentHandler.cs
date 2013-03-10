using System;
using System.Collections.Generic;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class ArrayArgumentHandler : MultipleArgumentHandler
  {
    private readonly object list;

    private readonly Type listType;


    public ArrayArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, int aMin, int aMax)
      : base(aObj, aInfo, aType, aMin, aMax)
    {
      elementType = type.GetElementType();
      listType = typeof(List<>).MakeGenericType(new Type[] { elementType });
      list = listType.GetConstructor(new Type[] { }).Invoke(null);
    }


    public override void Assign(string toAssign)
    {
      CheckAssign();
      listType.GetMethod("Add").Invoke(list, new object[] { InternalConvert(toAssign, elementType) });
      added++;
    }

    public override void Finish()
    {
      CheckFinish();
      if (added == 0) {
        return;
      }
      InternalAssign(listType.GetMethod("ToArray").Invoke(list, null));
      base.Finish();
      listType.GetMethod("Clear").Invoke(list, null);
    }
  }
}
