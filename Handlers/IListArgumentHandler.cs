using System;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class IListArgumentHandler : MultipleArgumentHandler
  {
    private readonly object list;


    public IListArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, int aMin, int aMax)
      : base(aObj, aInfo, aType, aMin, aMax)
    {
      switch (info.MemberType) {
        case MemberTypes.Field:
          list = (info as FieldInfo).GetValue(obj);
          break;
        case MemberTypes.Property:
          list = (info as PropertyInfo).GetValue(obj, null);
          break;
        default:
          throw new ProgrammingError("W00t?");
      }
      elementType = aType.GetGenericArguments()[0];
    }


    public override void Assign(string toAssign)
    {
      CheckAssign();
      type.GetMethod("Add").Invoke(list, new object[] { InternalConvert(toAssign, elementType) });
      added++;
      wasSet = true;
    }
  }
}
