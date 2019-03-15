using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  internal sealed class ArrayArgumentHandler : MultipleArgumentHandler
  {
    private readonly Delegate add;
    private readonly Action clear;
    private readonly object list;

    private readonly Type listType;

    internal ArrayArgumentHandler([NotNull] object handledObject, [NotNull] MemberInfo memberInfo, Type elementType,
      int min,
      int maxArguments)
      : base(handledObject, memberInfo, elementType, min, maxArguments)
    {
      InternalElementType = Type.GetElementType() ??
                            throw new ProgrammingErrorException($"Cannot get internal type for {memberInfo.Name}");
      listType = typeof(List<>).MakeGenericType(InternalElementType);
      list = listType.GetConstructor(new Type[] { })?.Invoke(null) ??
             throw new ProgrammingErrorException($"Cannot create list storage for {memberInfo.Name}");
      var addType = Expression.GetActionType(InternalElementType);
      add = Delegate.CreateDelegate(addType, list, "Add", false, true) ??
            throw new ProgrammingErrorException($"Invalid list type for {memberInfo.Name}");
      clear = (Action)Delegate.CreateDelegate(typeof(Action), list, "Clear", false, true) ??
              throw new ProgrammingErrorException($"Invalid list type for {memberInfo.Name}");
    }


    internal override void Assign(string toAssign)
    {
      CheckAssign();
      add.DynamicInvoke(InternalConvert(toAssign, InternalElementType));
      Added++;
    }

    internal override void Finish()
    {
      CheckFinish();
      if (Added == 0) {
        return;
      }

      InternalAssign(listType.GetMethod("ToArray")?.Invoke(list, null));
      base.Finish();
      clear();
    }
  }
}
