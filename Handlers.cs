using System.Reflection;
using System.ComponentModel;
using System;
using System.Collections.Generic;
namespace NMaier.GetOptNet
{
    abstract internal class ArgumentHandler
    {
        protected object obj;
        protected MemberInfo info;
        protected Type type;
        protected TypeConverter converter;
        private bool isflag;

        public bool IsFlag { get { return isflag; } }
        public string Name { get { return info.Name; } }

        public ArgumentHandler(object aObj, MemberInfo aInfo, Type aType, bool aIsFlag)
        {
            isflag = aIsFlag;
            obj = aObj;
            info = aInfo;
            type = aType;
        }

        protected object InternalConvert(string from)
        {
            try
            {
                return converter.ConvertFromString(from);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(ex.Message);
            }
        }

        protected void InternalAssign(object toAssign)
        {
            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fi = info as FieldInfo;
                    fi.SetValue(obj, toAssign);
                    return;

                case MemberTypes.Property:
                    PropertyInfo pi = info as PropertyInfo;
                    pi.SetValue(obj, toAssign, null);
                    return;

                default:
                    throw new ProgrammingError("w00t?");
            }
        }

        abstract public void Assign(string toAssign);
        abstract public void Finish();
    }

    sealed internal class PlainArgumentHandler : ArgumentHandler
    {
        private bool wasSet = false;
        private ArgumentCollision collision;

        public PlainArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, ArgumentCollision aCollision)
            : base(aObj, aInfo, aType, false)
        {
            collision = aCollision;
            converter = TypeDescriptor.GetConverter(type);
        }

        public override void Assign(string toAssign)
        {
            if (wasSet)
            {
                switch (collision) {
                    case ArgumentCollision.Throw:
                        throw new DuplicateArgumentException(String.Format("Argument {0} is specified more than once", Name));
                    case ArgumentCollision.Ignore:
                        return;
                    default:
                        break;
                }
            }
            wasSet = true;
            InternalAssign(InternalConvert(toAssign));
        }

        public override void Finish()
        {
            wasSet = false;
        }
    }

    sealed internal class FlagArgumentHandler : ArgumentHandler
    {
        private bool wasSet = false;
        private ArgumentCollision collision;

        public FlagArgumentHandler(Object aObj, MemberInfo aInfo, ArgumentCollision aCollision)
            : base(aObj, aInfo, typeof(bool), true)
        {
            collision = aCollision;
            converter = TypeDescriptor.GetConverter(type);
        }
        public override void Assign(string toAssign)
        {
            if (wasSet)
            {
                switch (collision)
                {
                    case ArgumentCollision.Throw:
                        throw new DuplicateArgumentException(String.Format("Argument {0} is specified more than once", Name));
                    case ArgumentCollision.Ignore:
                        return;
                    default:
                        break;
                }
            }
            wasSet = true;
            InternalAssign(true);
        }

        public override void Finish()
        {
            wasSet = false;
        }
    }

    sealed internal class CounterArgumentHandler : ArgumentHandler
    {
        Int64 current = 0;
        public CounterArgumentHandler(Object aObj, MemberInfo aInfo, Type aType)
            : base(aObj, aInfo, aType, true)
        {
        }
        public override void Assign(string toAssign)
        {
            current += 1;
        }
        public override void Finish()
        {
            InternalAssign((current as IConvertible).ToType(type, null));
            current = 0;
        }
    }

    sealed internal class ArrayArgumentHandler : ArgumentHandler
    {
        private Type listType;
        private object list;
        public ArrayArgumentHandler(Object aObj, MemberInfo aInfo, Type aType)
            : base(aObj, aInfo, aType, false)
        {
            converter = TypeDescriptor.GetConverter(type.GetElementType());
            listType = typeof(List<>).MakeGenericType(new Type[] { type.GetElementType() });
            list = listType.GetConstructor(new Type[] { }).Invoke(null);
        }
        public override void Assign(string toAssign)
        {
            listType.GetMethod("Add").Invoke(list, new object[] { InternalConvert(toAssign) });
                    }
        public override void Finish()
        {
            InternalAssign(listType.GetMethod("ToArray").Invoke(list, null));
            listType.GetMethod("Clear").Invoke(list, null);
        }

    }

    sealed internal class IListArgumentHandler : ArgumentHandler
    {
        private object list;
        public IListArgumentHandler(Object aObj, MemberInfo aInfo, Type aType)
            : base(aObj, aInfo, aType, false)
        {
            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    list = (info as FieldInfo).GetValue(obj);
                    break;
                case MemberTypes.Property:
                    list = (info as PropertyInfo).GetValue(obj, null);
                    break;
                default:
                    throw new ProgrammingError("W00t?");
            }
            converter = TypeDescriptor.GetConverter(aType.GetGenericArguments()[0]);
        }

        public override void Assign(string toAssign)
        {
            type.GetMethod("Add").Invoke(list, new object[] { InternalConvert(toAssign) });            
        }
        public override void Finish()
        {
        }
    }
}