using System.Reflection;

namespace NMaier.GetOptNet
{
  internal sealed class FlagArgumentHandler : ArgumentHandler
  {
    private readonly ArgumentCollision collision;

    private readonly bool whenSet;

    public FlagArgumentHandler(object handledObject, MemberInfo memberInfo, ArgumentCollision aCollision, bool required,
      bool whenSet)
      : base(handledObject, memberInfo, typeof(bool), true, required)
    {
      collision = aCollision;
      this.whenSet = whenSet;
    }


    internal override void Assign(string toAssign)
    {
      if (ShouldAssign(collision)) {
        InternalAssign(whenSet);
      }
    }

    internal override void Finish()
    {
      if (!WasSet) {
        InternalAssign(!whenSet);
      }

      base.Finish();
    }
  }
}
