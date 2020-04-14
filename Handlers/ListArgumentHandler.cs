using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class ListArgumentHandler : MultipleArgumentHandler
  {
    private readonly Delegate add;

    public ListArgumentHandler(object handledObject, MemberInfo memberInfo, Type elementType, int min, int maxArguments)
      : base(handledObject, memberInfo, elementType, min, maxArguments)
    {
      object list;
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (MemberInfo.MemberType) {
      case MemberTypes.Field when MemberInfo is FieldInfo fi:
        list = fi.GetValue(HandledObject);
        break;
      case MemberTypes.Property when MemberInfo is PropertyInfo pi:
        list = pi.GetValue(HandledObject, null);
        break;
      default:
        throw new ProgrammingErrorException("Unsupported member type");
      }

      InternalElementType = elementType.GetGenericArguments()[0];
      var addType = Expression.GetActionType(InternalElementType);
      add = Delegate.CreateDelegate(addType, list, "Add", false, true) ??
            throw new ProgrammingErrorException($"Invalid list type for {memberInfo.Name}");
    }


    internal override void Assign(string toAssign)
    {
      CheckAssign();
      add.DynamicInvoke(InternalConvert(toAssign, InternalElementType));
      Added++;
      WasSet = true;
    }
  }
}
