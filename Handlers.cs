/*
 * Copyright (c) 2009 Nils Maier
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

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
        protected Type type, elementType;

        protected bool wasSet = false;
        private bool isflag;
        private bool acceptsMultiple;
        private bool required = false;

        public MemberInfo Info { get { return info; } }
        public bool IsFlag { get { return isflag; } }
        public bool AcceptsMultiple { get { return acceptsMultiple; } }
        public string Name { get { return info.Name; } }
        public Type ElementType { get { return elementType; } }

        public ArgumentHandler(object aObj, MemberInfo aInfo, Type aType, bool aIsFlag, bool aAcceptsMultiple, bool aRequired)
        {
            isflag = aIsFlag;
            obj = aObj;
            info = aInfo;
            type = aType;
            elementType = type;
            acceptsMultiple = aAcceptsMultiple;
            required = aRequired;
        }
        protected object InternalConvert(string from)
        {
            return InternalConvert(from, type);
        }
        protected object InternalConvert(string from, Type type)
        {
            try
            {
                return TypeDescriptor.GetConverter(type).ConvertFromString(from);
            }
            catch (Exception ex)
            {
                try
                {
                    return type.GetConstructor(new Type[] { from.GetType() }).Invoke(new object[] { from });
                }
                catch (Exception)
                {
                    throw new NotSupportedException(ex.Message);
                }
            }
        }

        protected void InternalAssign(object toAssign)
        {
            try
            {
                switch (info.MemberType)
                {
                    case MemberTypes.Field:
                        FieldInfo fi = info as FieldInfo;
                        fi.SetValue(obj, toAssign);
                        break;

                    case MemberTypes.Property:
                        PropertyInfo pi = info as PropertyInfo;
                        pi.SetValue(obj, toAssign, null);
                        break;

                    default:
                        throw new ProgrammingError("w00t?");
                }
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            wasSet = true;
        }

        abstract public void Assign(string toAssign);
        virtual public void Finish()
        {
            if (required && !wasSet)
            {
                throw new RequiredOptionMissingException(this);
            }
            wasSet = false;
        }
        protected bool CheckCollision(ArgumentCollision aCollision)
        {
            if (wasSet)
            {
                switch (aCollision)
                {
                    case ArgumentCollision.Throw:
                        throw new DuplicateArgumentException(String.Format("Option {0} is specified more than once", Name));
                    case ArgumentCollision.Ignore:
                        return false;
                }
            }
            return true;
        }
    }

    sealed internal class PlainArgumentHandler : ArgumentHandler
    {
        private ArgumentCollision collision;

        public PlainArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, ArgumentCollision aCollision, bool aRequired)
            : base(aObj, aInfo, aType, false, false, aRequired)
        {
            collision = aCollision;
        }

        public override void Assign(string toAssign)
        {
            if (CheckCollision(collision))
            {
                InternalAssign(InternalConvert(toAssign));
            }
        }
    }

    sealed internal class FlagArgumentHandler : ArgumentHandler
    {
        private bool whenSet = true;
        private ArgumentCollision collision;

        public FlagArgumentHandler(Object aObj, MemberInfo aInfo, ArgumentCollision aCollision, bool aRequired, Boolean aWhenSet)
            : base(aObj, aInfo, typeof(bool), true, false, aRequired)
        {
            collision = aCollision;
            whenSet = aWhenSet;
        }
        public override void Assign(string toAssign)
        {
            if (CheckCollision(collision))
            {
                InternalAssign(whenSet);
            }
        }
        public override void Finish()
        {
            if (!wasSet)
            {
                InternalAssign(!whenSet);
            }
            base.Finish();
        }
    }

    sealed internal class CounterArgumentHandler : ArgumentHandler
    {
        Int64 current = 0;
        public CounterArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, bool aRequired)
            : base(aObj, aInfo, aType, true, true, aRequired)
        {
        }
        public override void Assign(string toAssign)
        {
            current += 1;
        }
        public override void Finish()
        {
            InternalAssign((current as IConvertible).ToType(type, null));
            base.Finish();
            current = 0;
        }
    }

    internal abstract class MultipleArgumentHandler : ArgumentHandler
    {
        protected uint min;
        protected uint max;
        protected uint added = 0;
        public MultipleArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, uint aMin, uint aMax)
            : base(aObj, aInfo, aType, false, true, aMin > 0)
        {
            min = aMin;
            max = aMax;
        }
        public override void Finish()
        {
            if (added < min)
            {
                throw new MultipleArgumentCountException(String.Format("Not enough arguments supplied for {0}", Name.ToUpper()));
            }
            base.Finish();
        }

        public uint Min { get { return min; } }
        public uint Max { get { return max; } }

        protected void CheckAssign()
        {
            if (max > 0 && added == max)
            {
                throw new MultipleArgumentCountException(String.Format("Too many arguments supplied for {0}", Name.ToUpper()));
            }
        }
    }

    sealed internal class ArrayArgumentHandler : MultipleArgumentHandler
    {
        private Type listType;
        private object list;
        public ArrayArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, uint aMin, uint aMax)
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
            InternalAssign(listType.GetMethod("ToArray").Invoke(list, null));
            base.Finish();
            listType.GetMethod("Clear").Invoke(list, null);
        }

    }

    sealed internal class IListArgumentHandler : MultipleArgumentHandler
    {
        private object list;
        public IListArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, uint aMin, uint aMax)
            : base(aObj, aInfo, aType, aMin, aMax)
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