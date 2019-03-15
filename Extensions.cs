using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  internal static class Extensions
  {
    internal static T GetAttribute<T>([NotNull] this MemberInfo info)
    {
      return info.GetAttributes<T>().First();
    }

    internal static T[] GetAttributes<T>([NotNull] this MemberInfo info)
    {
      return info.GetCustomAttributes(typeof(T), true) as T[] ?? new T[0];
    }

    internal static bool HasAttribute<T>([NotNull] this MemberInfo info)
    {
      var attr = info.GetCustomAttributes(typeof(T), true);
      return attr.Length != 0;
    }
  }
}
