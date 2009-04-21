using System;
using System.Text;

namespace NMaier.GetOptNet
{
    abstract public partial class GetOpt
    {
        public void PrintUsage()
        {
            int ww = 80;
            try
            {
                if (Console.WindowWidth > 0)
                {
                    ww = Console.WindowWidth;
                }
            }
            catch (Exception)
            {
                // no op
            }
            Console.Write(AssembleUsage(ww));
        }
        public string AssembleUsage(int width)
        {
            string nl = Environment.NewLine;
            StringBuilder s = new StringBuilder();

            s.Append("Usage:");
            s.Append(nl);


            s.Append(nl);
            return s.ToString();
        }
    }
}