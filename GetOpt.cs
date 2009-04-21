using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NMaier.GetOptNet
{
    [GetOptOptions()]
    abstract public partial class GetOpt
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

        public GetOpt()
        {
            Initialize();
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
    }
}
