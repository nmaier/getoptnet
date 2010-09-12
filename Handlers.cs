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

        protected TypeConverter converter;
        private bool isflag;
        private bool acceptsMultiple;
        private bool required = false;

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

        public PlainArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, ArgumentCollision aCollision, bool aRequired)
            : base(aObj, aInfo, aType, false, false, aRequired)
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
        private bool whenSet = true;
        private ArgumentCollision collision;

        public FlagArgumentHandler(Object aObj, MemberInfo aInfo, ArgumentCollision aCollision, bool aRequired, Boolean aWhenSet)
            : base(aObj, aInfo, typeof(bool), true, false, aRequired)
        {
            collision = aCollision;
            converter = TypeDescriptor.GetConverter(type);
            whenSet = aWhenSet;
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
            InternalAssign(whenSet);
        }

        public override void Finish()
        {
            wasSet = false;
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
            current = 0;
        }
    }

    sealed internal class ArrayArgumentHandler : ArgumentHandler
    {
        private Type listType;
        private object list;
        public ArrayArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, bool aRequired)
            : base(aObj, aInfo, aType, false, true, aRequired)
        {
            elementType = type.GetElementType();
            converter = TypeDescriptor.GetConverter(elementType);
            listType = typeof(List<>).MakeGenericType(new Type[] { elementType });
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
        public IListArgumentHandler(Object aObj, MemberInfo aInfo, Type aType, bool aRequired)
            : base(aObj, aInfo, aType, false, true, aRequired)
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
            converter = TypeDescriptor.GetConverter(elementType);
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