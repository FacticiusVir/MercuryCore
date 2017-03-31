using System.Collections.Generic;

namespace Keeper.MercuryCore.CommandLoop.Parsing
{
    public struct CommandInfo
    {
        public CommandInfo(string name, IEnumerable<string> parameterValues)
        {
            this.Name = name;
            this.ParameterValues = parameterValues;
            this.IsValid = true;
        }

        public static CommandInfo Invalid => new CommandInfo();

        public string Name
        {
            get;
            private set;
        }

        public IEnumerable<string> ParameterValues
        {
            get;
            private set;
        }

        public bool IsValid
        {
            get;
            private set;
        }
    }
}