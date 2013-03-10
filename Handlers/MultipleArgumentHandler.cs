using System;
using System.Reflection;

namespace NMaier.GetOptNet
{
  internal abstract class MultipleArgumentHandler : ArgumentHandler
  {
    protected uint added = 0;

    protected int max;

    protected int min;


    public MultipleArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, int aMin, int aMax)
      : base(aObj, aInfo, aType, false, aMin > 0)
    {
      min = aMin;
      max = aMax;
    }


    public int Max
    {
      get {
        return max;
      }
    }
    public int Min
    {
      get {
        return min;
      }
    }


    protected void CheckAssign()
    {
      if (max > 0 && added == max) {
        throw new MultipleArgumentCountException(String.Format("Too many arguments supplied for {0}", Name.ToLower()));
      }
    }

    protected void CheckFinish()
    {
      if (added < min) {
        throw new MultipleArgumentCountException(String.Format("Not enough arguments supplied for {0}", Name.ToLower()));
      }
    }


    public override void Finish()
    {
      CheckFinish();
      if (added == 0) {
        return;
      }
      base.Finish();
    }
  }
}
