using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace CommandLineUtility
{
    public class CmdArgManager
    {
        private dynamic o;
        private string[] args;

        public string Description = null;
        public string Url = null;
        public Dictionary<string, string> ArgsValuePairs = new Dictionary<string, string>();
        public List<CmdArgInfo> CmdArgInfoCollection;
        public string ErrorMessage { get; set; }

        const string TRUESTRING = "true";

        public enum ExitCode : int
        {
            DuplicateFlagsPresentInAttribute,
            DuplicateFlagOrShortPresentOnCommandLine,
            FirstArgIsNotAnId,
            FlagOrShortHandDoesNotStartWithDash,
            FlagOrShorthandMissing,
            FlagAndShorthandPresentOnCommandLine,
            HelpShown,
            HelpFlagCannotBeInCmdArgsClass,
            IDAndValueCountDoNotMatch,
            InvalidTypeInArgsClass,
            RequiredFlagNotProvided,
            Success,
            UnknownFlagsOrShorthandPresent,
            ValueMissing,
            ValueMustBeANumber,
            ExceptionOccurred
        }

        public CmdArgManager(dynamic o, string[] args, string description) : this((object)o, args, description, null)
        {
        }

        public CmdArgManager(dynamic o, string[] args, string description, string url)
        {
            CmdArgInfoLogic cail = new CmdArgInfoLogic();

            if (cail.SetCmdArgInfo(o) != ExitCode.Success)
            {
                throw new System.ArgumentException(cail.ErrorMessage);
            }

            this.CmdArgInfoCollection = cail.CmdArgInfoCollection;
            this.o = o;
            this.args = args;
            this.Description = description;
            this.Url = url;
        }

        public ExitCode ParseArgs()
        {
            ExitCode result;

            if (this.args.Length == 0 ||
                this.args[0].ToLower() == "--help" ||
                this.args[0].ToLower() == "-h")
            {

                return ShowHelp();
            }

            result = CheckForUnknownFlagsOnCommandLine();
            if (result != ExitCode.Success) return result;

            result = CheckForRequiredFlagsOnCommandLine();
            if (result != ExitCode.Success) return result;

            result = CheckForFlagAndShorthandOnCommandLine();
            if (result != ExitCode.Success) return result;

            result = CheckForDuplicateFlagOrShorthandOnCommandLine();
            if (result != ExitCode.Success) return result;

            CommandLineArgParser clap = new CommandLineArgParser(this.CmdArgInfoCollection);
            result = clap.Parse(this.args);
            if (result == ExitCode.Success) {
                this.ArgsValuePairs = clap.ValuePairs;
            }
            else
            {
                this.ErrorMessage = clap.ErrorMessage;
                return result;
            }

            result = this.SetArgumentValues();
            return result;
        }

        public ExitCode CheckForRequiredFlagsOnCommandLine()
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
                return ExitCode.RequiredFlagNotProvided;
            }

            return ExitCode.Success;
        }

        public ExitCode CheckForDuplicateFlagOrShorthandOnCommandLine()
        {
            List<string> flagsUsed = new List<string>();

            foreach (string arg in this.args)
            {
                if (arg.StartsWith("-"))
                {
                    if (flagsUsed.ToArray().Contains(arg))
                    {
                        this.ErrorMessage = String.Format("Duplicate flag or shorthand ({0}) used on command line.", arg);
                        return ExitCode.DuplicateFlagOrShortPresentOnCommandLine;
                    }
                    flagsUsed.Add(arg);
                }
            }

            return ExitCode.Success;
        }

        public ExitCode CheckForFlagAndShorthandOnCommandLine()
        {
            List<string> duplicateArgs = new List<string>();

            foreach (CmdArgInfo cmdArgInfo in this.CmdArgInfoCollection)
            {
                if (Array.Find(args, element => element == cmdArgInfo.FlagName) != null &&
                    Array.Find(args, element => element == cmdArgInfo.FlagShortHand) != null)
                {
                    duplicateArgs.Add(String.Format("{0} and {1}", cmdArgInfo.FlagName, cmdArgInfo.FlagShortHand));
                }
            }
            if (duplicateArgs.Count > 0)
            {
                this.ErrorMessage = "Both a flag and a shortcut were provided: " + String.Join(", ", duplicateArgs);
                return ExitCode.FlagAndShorthandPresentOnCommandLine;
            }

            return ExitCode.Success;
        }

        public ExitCode CheckForUnknownFlagsOnCommandLine()
        {
            List<string> unknownArgs = new List<string>();

            StringBuilder sb = new StringBuilder();
            List<string> flagsAndShorthands = new List<string>();

            foreach (CmdArgInfo cmdArgInfo in this.CmdArgInfoCollection)
            {
                flagsAndShorthands.Add(cmdArgInfo.FlagName);
                flagsAndShorthands.Add(cmdArgInfo.FlagShortHand);
            }

            string[] possibleArgs = flagsAndShorthands.ToArray();

            foreach (string arg in this.args)
            {
                if (arg.StartsWith("-") && !possibleArgs.Contains(arg) || arg.Trim() == "-" || arg.Trim() == "--")
                {
                    unknownArgs.Add(arg);
                }
            }

            if (unknownArgs.Count > 0)
            {
                this.ErrorMessage = "These flags/shorthands are unknown: " + String.Join(", ", unknownArgs);
                return ExitCode.UnknownFlagsOrShorthandPresent;
            }

            return ExitCode.Success;
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

        private string removeLeadingMinusSignEscape(string str)
        {
            // Swap out leading "\-" for "-". This is a confusing
            // regex because the first backslash escapes the second backslash.
            return Regex.Replace(str, @"^\\-", "-");
        }

        private bool isNumericWithOptionalLeadingMinusSign(string str)
        {
            return Regex.Match(str, @"^(-)?[0-9]+$").Success;
        }

        public ExitCode SetArgumentValues()
        {
            foreach (KeyValuePair<string, string> entry in ArgsValuePairs)
            {
                CmdArgInfo ci = getCmdArgInfoByFlagOrShortHand(entry.Key);
                string value = entry.Value;
                value = removeLeadingMinusSignEscape(value);

                Type t = o.GetType();
                PropertyInfo pi = t.GetProperty(ci.PropertyName);

                switch (ci.PropertyType)
                {
                    case "Int32":
                        if (isNumericWithOptionalLeadingMinusSign(value))
                        {
                            pi.SetValue(this.o, Int32.Parse(value), null);
                            break;
                        }
                        this.ErrorMessage = String.Format("Value for {0}/{1} must be a number.", ci.FlagName, ci.FlagShortHand);
                        return ExitCode.ValueMustBeANumber;

                    case "Int64":
                        if (isNumericWithOptionalLeadingMinusSign(value))
                        {
                            pi.SetValue(this.o, long.Parse(value), null);
                            break;
                        }
                        this.ErrorMessage = String.Format("Value for {0}/{1} must be a number.", ci.FlagName, ci.FlagShortHand);
                        return ExitCode.ValueMustBeANumber;

                    case "String":
                        string str = (string)value;
                        // Fix escaped leading minus sign.
                        str = Regex.Replace(str, @"^\\\-", "-");
                        pi.SetValue(this.o, str, null);
                        break;

                    case "Boolean":
                        pi.SetValue(this.o, value.ToLower() == "true", null);
                        break;
                }
            }

            return CmdArgManager.ExitCode.Success;
        }

        public event EventHandler<ShowHelpEventArgs> HelpShown;

        protected virtual void OnHelpShown(ShowHelpEventArgs e)
        {
            HelpShown.Invoke(this, e);
        }

        public ExitCode ShowHelp()
        {
            Console.Clear();
            if (!string.IsNullOrEmpty(this.Description))
            {
                CustomConsole.WriteLineInfo(Description);
                CustomConsole.WriteLineInfo(String.Empty);
            }

            CustomConsole.WriteLineInfo("Flag                  ShortHand  Required  Description");
            CustomConsole.WriteLineInfo("--------------------  ---------  --------  ---------------------------------------------");

            foreach (CmdArgInfo ai in CmdArgInfoCollection)
            {
                CustomConsole.WriteLineInfo("{0,-24} {1,-7}   {2,-6}  {3}",
                                             ai.FlagName, ai.FlagShortHand, ai.Required.ToString(), ai.Description);
            }
            CustomConsole.WriteLineInfo(String.Format("{0,-24} {1,-7}   {2,-6}  {3}", "--help", "-h", "", "Show this help"));

            if (!string.IsNullOrEmpty(this.Url))
            {
                CustomConsole.WriteLineInfo(String.Empty);
                CustomConsole.WriteLineInfo("See this URL for more help: {0}", Url);
            }

            OnHelpShown(new ShowHelpEventArgs(this.o));

            return ExitCode.HelpShown;
        }
    }

    public class ShowHelpEventArgs : EventArgs
    {
        public dynamic CmdArgs { get; set; }

        public ShowHelpEventArgs(object CmdArgs)
        {
            this.CmdArgs = CmdArgs;
        }
    }

}

