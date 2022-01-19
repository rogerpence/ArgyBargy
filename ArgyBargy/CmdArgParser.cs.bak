using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineUtility
{ 
    public class CommandLineArgParser
    {
        public Dictionary<string, string> ValuePairs = new Dictionary<string, string>();

        private List<CmdArgInfo> CmdArgInfoCollection;
        private string[] args;

        public string ErrorMessage { get; set; }

        const string TRUESTRING = "true";


        public CommandLineArgParser(List<CmdArgInfo> CmdArgInfoCollection)
        {
            this.CmdArgInfoCollection = CmdArgInfoCollection;
        }

        public CmdArgManager.ExitCode Parse(string[] args)
        {
            this.args = args;
            CmdArgManager.ExitCode result;
            List<string> id = new List<string>();
            List<string> value = new List<string>();

            int firstElement = 0;
            int lastElement = args.Length - 1;

            for (int i = 0; i <= args.Length - 1; i++)
            {
                string currentArg = args[i];
                int currentElement = i;
                int nextElement = i + 1;

                if (!args[firstElement].StartsWith("-"))
                {
                    this.ErrorMessage = String.Format("The first argument '{0}' doesn't start with '-' or '--'", args[firstElement]);
                    return CmdArgManager.ExitCode.FirstArgIsNotAnId;
                }

                if (currentElement == lastElement)
                {
                    result = processLastArgument(id, value, args[currentElement]);
                    if (result != CmdArgManager.ExitCode.Success) return result;
                    break;
                }

                if (args[currentElement].StartsWith("-")) 
                {
                    result = processPotentialImplicitBooleanArgument(id, value, args[currentElement]);
                    if (result != CmdArgManager.ExitCode.Success) return result;
                    continue;
                }

                result = processCurrentArgument(id, value, args[currentElement]);
                if (result != CmdArgManager.ExitCode.Success) return result;
            }

            if (id.Count != value.Count)
            {
                this.ErrorMessage = "Something is very wrong when the ID and value counts don't match.";
                return CmdArgManager.ExitCode.IDAndValueCountDoNotMatch;
            }

            margeArgsIntoDictionary(id, value);

            return CmdArgManager.ExitCode.Success;
        }

        public bool IsArgBool(string flagOrShorthand)
        {
            CmdArgInfo ci = getCmdArgInfoByFlagOrShortHand(flagOrShorthand);

            return ci.PropertyType == "Boolean";
        }

        // If id.Count > value.Count then an ID has been registered and 
        // the next argument should provide its value. 
        public bool readyForNextValue(List<string> id, List<string> value)
        {
            return id.Count > value.Count;
        }

        // If id.Count == value.Count all flags/shorthands have been provided
        // values so far. The next argument read should be an id (a flag or a shorthand). 
        public bool readyForNextId(List<string> id, List<string> value)
        {
            return id.Count == value.Count;
        }

        public CmdArgManager.ExitCode processCurrentArgument(List<string> id, List<string> value, string argument)
        {
            if (argument.StartsWith("-"))
            {
                id.Add(argument);
                return CmdArgManager.ExitCode.Success;
            }
            else
            {
                if (readyForNextValue(id, value))
                {
                    value.Add(argument);
                    return CmdArgManager.ExitCode.Success;
                }
                else
                {
                    this.ErrorMessage = String.Format("The value [{0}] is present without a preceding flag/shorthand identifer", argument);
                    return CmdArgManager.ExitCode.FlagOrShorthandMissing;
                }
            }
        }

        public CmdArgManager.ExitCode processLastArgument(List<string> id, List<string> value, string argument)
        {
            if (argument.StartsWith("-"))
            {
                return processPotentialImplicitBooleanArgument(id, value, argument);
            }
            else
            {
                if (readyForNextValue(id, value))
                {
                    value.Add(argument);
                    return CmdArgManager.ExitCode.Success;
                }
            }
            this.ErrorMessage = String.Format("Error in processLastArgument with argument [{0}]", argument);
            return CmdArgManager.ExitCode.FlagOrShorthandMissing;
        }

        public CmdArgManager.ExitCode processPotentialImplicitBooleanArgument(List<string> id, List<string> value, string argument)
        {
            if (IsArgBool(argument))
            {
                id.Add(argument);
                value.Add(TRUESTRING);
                return CmdArgManager.ExitCode.Success;
            }
            else
            {
                if (readyForNextId(id, value))
                {
                    id.Add(argument);
                    return CmdArgManager.ExitCode.Success;
                } 

                this.ErrorMessage = String.Format("Value is missing for {0}", argument);
                return CmdArgManager.ExitCode.ValueMissing;
            }
        }

        private void margeArgsIntoDictionary(List<string> id, List<string> value)
        {
            for (int i = 0; i <= id.Count - 1; i++)
            {
                ValuePairs.Add(id[i], value[i]);
            }
        }

        public CmdArgInfo GetCmdArgInfoByPropertyName(string propertyName)
        {
            foreach (CmdArgInfo cmdArgInfo in this.CmdArgInfoCollection)
            {
                if (cmdArgInfo.PropertyName == propertyName)
                {
                    return cmdArgInfo;
                }
            }
            return null;
        }

        private CmdArgInfo getCmdArgInfoByFlagOrShortHand(string flagOrShorthand)
        {
            foreach (CmdArgInfo cmdArgInfo in this.CmdArgInfoCollection)
            {
                if (flagOrShorthand == cmdArgInfo.FlagName || flagOrShorthand == cmdArgInfo.FlagShortHand)
                {
                    return cmdArgInfo;
                }
            }

            return null;
        }

        public CmdArgManager.ExitCode CheckForDuplicateFlagOrShorthandOnCommandLine()
        {
            List<string> flagsUsed = new List<string>();

            foreach (string arg in this.args)
            {
                if (arg.StartsWith("-"))
                {
                    if (flagsUsed.ToArray().Contains(arg))
                    {
                        this.ErrorMessage = String.Format("Duplicate flag or shorthand ({0}) used on command line.", arg);
                        return CmdArgManager.ExitCode.DuplicateFlagOrShortPresentOnCommandLine;
                    }
                    flagsUsed.Add(arg);
                }
            }

            return CmdArgManager.ExitCode.Success;
        }

        public CmdArgManager.ExitCode CheckForRequiredFlagsOnCommandLine()
        {
            List<string> missingArgs = new List<string>();

            foreach (CmdArgInfo cmdArgInfo in this.CmdArgInfoCollection)
            {
                if (cmdArgInfo.Required)
                {
                    if (Array.Find(args, element => element == cmdArgInfo.FlagName) == null &&
                        Array.Find(args, element => element == cmdArgInfo.FlagShortHand) == null)
                    {
                        missingArgs.Add(String.Format("{0} or {1}", cmdArgInfo.FlagName, cmdArgInfo.FlagShortHand));
                    }
                }
            }

            if (missingArgs.Count > 0)
            {
                this.ErrorMessage = "These required flags aren't provided: " + String.Join(" and ", missingArgs);
                return CmdArgManager.ExitCode.RequiredFlagNotProvided;
            }

            return CmdArgManager.ExitCode.Success;
        }
    }

}
