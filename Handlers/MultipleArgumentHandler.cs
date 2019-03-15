using System;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal abstract class MultipleArgumentHandler : ArgumentHandler
  {
    protected uint Added = 0;


    protected MultipleArgumentHandler(object handledObject, MemberInfo memberInternalInfo, Type elementType, int min,
      int max)
      : base(handledObject, memberInternalInfo, elementType, false, min > 0)
    {
      Min = min;
      Max = max;
    }


    internal int Max { get; }

    internal int Min { get; }


    protected void CheckAssign()
    {
      if (Max > 0 && Added == Max) {
        throw new MultipleArgumentCountException(
          $"Too many arguments supplied for {Name.ToLower()}");
      }
    }

    protected void CheckFinish()
    {
      if (Added < Min) {
        throw new MultipleArgumentCountException(
          $"Not enough arguments supplied for {Name.ToLower()}");
      }
    }


    internal override void Finish()
    {
      CheckFinish();
      if (Added == 0) {
        return;
      }

      base.Finish();
    }
  }
}
