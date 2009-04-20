using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;

namespace NMaier.GetOptNet
{
    [GetOptOptions()]
    abstract public class GetOpt
    {
        private static Regex regDashesLong = new Regex("^\\s*--([\\w\\d_-]+)(?:=(.+)|\\s*)?$", RegexOptions.Compiled);
        private static Regex regDashesShort = new Regex("^\\s*-([\\w\\d]+)\\s*$", RegexOptions.Compiled);
        private static Regex regDashesDie = new Regex("^\\s*--\\s*$", RegexOptions.Compiled);
        private static Regex regSlashes = new Regex("^\\s*/([\\w\\d_-]+)(?:=(.+)|\\s*)?$", RegexOptions.Compiled);


        private ArgumentHandler parameters;
        private Dictionary<string, ArgumentHandler> longs = new Dictionary<string, ArgumentHandler>();
        private Dictionary<char, ArgumentHandler> shorts = new Dictionary<char, ArgumentHandler>();
        private List<ArgumentHandler> handlers = new List<ArgumentHandler>();
        private GetOptOptions opts;

        private bool isIList(Type aType)
        {
            if (!aType.IsGenericType)
            {
                return false;
            }
            if (aType.ContainsGenericParameters)
            {
                throw new ProgrammingError("Generic type not closed!");
            }
            Type[] gens = aType.GetGenericArguments();
            if (gens.Length == 1) {
                Type genType = typeof(IList<>).MakeGenericType(gens);
                if (aType.GetInterface(genType.Name) != null) {
                    return true;
                }
            }
            return false;
        }

        public GetOpt()
        {
            Type me = GetType();
            opts = me.GetCustomAttributes(typeof(GetOptOptions), true)[0] as GetOptOptions;
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            foreach (MemberInfo[] infoArray in new MemberInfo[][] { me.GetFields(flags), me.GetProperties(flags) })
            {
                foreach (MemberInfo info in infoArray)
                {
                    if (info.GetCustomAttributes(typeof(Parameters), true).Length == 1)
                    {
                        if (parameters != null || info.MemberType != MemberTypes.Field)
                        {
                            throw new ProgrammingError("Duplicate declaration for parameters");
                        }
                        FieldInfo field = info as FieldInfo;
                        if (field == null) {
                            throw new ProgrammingError("W00T?");
                        }
                        if (field.FieldType.IsArray) {
                            parameters = new ArrayArgumentHandler(this, field, field.FieldType);
                        }
                        else if (isIList(field.FieldType)) {
                            parameters = new IListArgumentHandler(this, field, field.FieldType);
                        }
                        else {
                            throw new ProgrammingError("parameters must be an array type or a IList implementation");
                        }
                        handlers.Add(parameters);
                        continue;
                    }
                    Argument[] args = info.GetCustomAttributes(typeof(Argument), true) as Argument[];
                    if (args.Length != 1)
                    {
                        continue;
                    }
                    if (opts.AcceptPrefix == ArgumentPrefixType.None) {
                        throw new ProgrammingError("You used Prefix=None, hence there are no arguments allowed!");
                    }
                    Argument arg = args[0];
                    string name = arg.GetArg();
                    if (String.IsNullOrEmpty(name))
                    {
                        name = info.Name;
                    }
                    if (longs.ContainsKey(name))
                    {
                        throw new ProgrammingError(String.Format("Duplicate argument {0}", name));
                    }

                    PropertyInfo pi = info as PropertyInfo;
                    FieldInfo fi = info as FieldInfo;
                    ArgumentHandler ai;
                    Type memberType;
                    if (pi != null)
                    {
                        if (!pi.CanWrite)
                        {
                            throw new ProgrammingError(String.Format("Property {0} is an argument but not assignable", info.Name));
                        }
                        memberType = pi.PropertyType;
                    }
                    else if (fi != null)
                    {
                        memberType = fi.FieldType;
                    }
                    else
                    {
                        throw new ProgrammingError("WTF?!");
                    }
                    if (memberType.IsArray)
                    {
                        ai = new ArrayArgumentHandler(this, info, memberType);
                    }
                    else if (isIList(memberType))
                    {
                        ai = new IListArgumentHandler(this, info, memberType);
                    }
                    else
                    {
                        if (memberType == typeof(bool) || memberType == typeof(Boolean) || memberType.IsSubclassOf(typeof(Boolean)))
                        {
                            ai = new FlagArgumentHandler(this, info, arg.OnCollision);
                        }
                        else if (info.GetCustomAttributes(typeof(Counted), true).Length != 0)
                        {
                            ai = new CounterArgumentHandler(this, info, memberType);
                        }
                        else
                        {
                            ai = new PlainArgumentHandler(this, info, memberType, arg.OnCollision);
                        }
                    }

                    longs.Add(name, ai);
                    handlers.Add(ai);

                    foreach (ArgumentAlias alias in info.GetCustomAttributes(typeof(ArgumentAlias), true))
                    {
                        string an = alias.GetAlias();
                        if (longs.ContainsKey(an))
                        {
                            throw new ProgrammingError(String.Format("Duplicate alias argument {0}", an));
                        }
                        longs.Add(an, ai);
                    }
                    foreach (ShortArgument sa in info.GetCustomAttributes(typeof(ShortArgument), true))
                    {
                        char an = sa.GetArg();
                        if (shorts.ContainsKey(an))
                        {
                            throw new ProgrammingError(String.Format("Duplicate short argument {0}", an));
                        }
                        shorts.Add(an, ai);
                    }
                }
            }
        }
        public void Parse(string[] args)
        {
            Parse(new List<string>(args));
        }
        public void Parse(IList<string> args)
        {
            Update(args);
            foreach (ArgumentHandler h in handlers)
            {
                try
                {
                    h.Finish();
                }
                catch (NotSupportedException)
                {
                    throw new InvalidValueException("Argument has a different type");
                }
            }
        }
        public void Update(string[] args)
        {
            Update(new List<string>(args));
        }
        private void UpdateHandler(ArgumentHandler handler, string value, string arg)
        {
            try
            {
                handler.Assign(value);
            }
            catch (ArgumentException ex)
            {
                throw new ProgrammingError(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                throw new InvalidValueException(String.Format("Wrong value type for argument \"{0}\": {1}", arg, ex.Message));
            }
            catch (Exception ex)
            {
                throw new ProgrammingError(ex.Message);
            }
        }
        public void Update(IList<string> args)
        {
            if (args == null)
            {
                throw new ArgumentException("args may not be null");
            }
            IEnumerator<string> e = args.GetEnumerator();
            while (e.MoveNext())
            {
                string c = e.Current;
                if ((opts.AcceptPrefix & ArgumentPrefixType.Dashes) != 0)
                {
                    if (regDashesDie.IsMatch(c))
                    {
                        break;
                    }
                    Match m = regDashesLong.Match(c);
                    if (m.Success)
                    {
                        string arg = m.Groups[1].Value;
                        string val = m.Groups[2].Value;
                        if (!longs.ContainsKey(arg))
                        {
                            throw new UnknownAttributeException(String.Format("There is no attribute \"{0}\"", arg));
                        }
                        ArgumentHandler h = longs[arg];
                        if (!String.IsNullOrEmpty(val) && h.IsFlag) {
                            throw new InvalidValueException(String.Format("Argument \"{0}\" does not except a value", arg));
                        }
                        else if (String.IsNullOrEmpty(val) && !h.IsFlag)
                        {
                            throw new InvalidValueException(String.Format("Omitted value for argument \"{0}\"", arg));
                        }
                        UpdateHandler(h, val, arg);

                        continue;
                    }
                    m = regDashesShort.Match(c);
                    if (m.Success) {
                        string arg = m.Groups[1].Value;
                        char[] singles = arg.ToCharArray();
                        for (int i = 0, l = singles.Length; i < l; ++i)
                        {
                            string val = null;
                            if (!shorts.ContainsKey(singles[i]))
                            {
                                throw new UnknownAttributeException(String.Format("There is no attribute \"{0}\"", arg));
                            }
                            ArgumentHandler h = shorts[singles[i]];
                            if (!String.IsNullOrEmpty(val) && h.IsFlag)
                            {
                                throw new InvalidValueException(String.Format("Argument \"{0}\" does not except a value", arg));
                            }
                            else if (String.IsNullOrEmpty(val) && !h.IsFlag)
                            {
                                val = (i != l - 1 || !e.MoveNext()) ? val : e.Current;

                                if (String.IsNullOrEmpty(val))
                                {
                                    throw new InvalidValueException(String.Format("Omitted value for argument \"{0}\"", singles[i]));
                                }
                            }
                            UpdateHandler(h, val, arg);
                            continue;
                        }
                        continue;
                    }
                }
                if ((opts.AcceptPrefix & ArgumentPrefixType.Slashes) != 0)
                {
                    Match m = regSlashes.Match(c);
                    if (m.Success)
                    {
                        string arg = m.Groups[1].Value;
                        string val = m.Groups[2].Value;
                        ArgumentHandler h;
                        if (arg.Length == 1)
                        {
                            char a = arg[0];
                            if (!shorts.ContainsKey(a))
                            {
                                throw new UnknownAttributeException(String.Format("There is no attribute \"{0}\"", arg));
                            }
                            h = shorts[a];
                            if (!String.IsNullOrEmpty(val) && h.IsFlag)
                            {
                                throw new InvalidValueException(String.Format("Argument \"{0}\" does not except a value", arg));
                            }
                            else if (String.IsNullOrEmpty(val) && !h.IsFlag)
                            {
                                val = e.MoveNext() ? e.Current : val;
                                if (String.IsNullOrEmpty(val)) {
                                    throw new InvalidValueException(String.Format("Omitted value for argument \"{0}\"", arg));
                                }
                            }
                            UpdateHandler(h, val, arg);
                            continue;
                        }
                        if (!longs.ContainsKey(arg))
                        {
                            throw new UnknownAttributeException(String.Format("There is no attribute \"{0}\"", arg));
                        }
                        h = longs[arg];
                        if (String.IsNullOrEmpty(val) && !h.IsFlag)
                        {
                            throw new InvalidValueException(String.Format("Argument \"{0}\" does except a value", arg));
                        }
                        UpdateHandler(h, val, arg);
                        continue;
                    }
                }
                UpdateHandler(parameters, c, "<parameters>");
            }
            while (e.MoveNext())
            {
                UpdateHandler(parameters, e.Current, "<parameters>");
            }
        }

    }
}
