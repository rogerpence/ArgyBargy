using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineUtility
{
    public class CmdArgInfo
    {
        public string PropertyName { get; set; }
        public string FlagName { get; set; }
        public string FlagShortHand { get; set; }
        public string PropertyType { get; set; }
        public bool Required { get; set; }
        public string Description { get; set; }
    }

    public class CmdArgInfoLogic
    {
        public List<CmdArgInfo> CmdArgInfoCollection;

        public string ErrorMessage;

        public CmdArgInfoLogic()
        {
        }

        public CmdArgManager.ExitCode SetCmdArgInfo(dynamic o)
        {
            this.CmdArgInfoCollection = new List<CmdArgInfo>();

            Type t = o.GetType();

            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            List<CmdArgInfo> CmdArgInfoCollection = new List<CmdArgInfo>();

            string[] allowableTypes = new string[] { "Int32", "Int64", "String", "Boolean" };
            List<String> flagsUsed = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                string propName = property.Name;
                PropertyInfo pi = t.GetProperty(propName);
                string propType = pi.PropertyType.Name;

                var attributeList = Attribute.GetCustomAttributes(pi);
                if (attributeList.Length > 0)
                {
                    if (!allowableTypes.Contains(propType))
                    {
                        this.ErrorMessage = String.Format("Property [{0}] is an invalid type: [{1}]. Types must be int, long, or string", propName, propType);
                        return CmdArgManager.ExitCode.InvalidTypeInArgsClass;
                    }

                    string flagName1 = ((CmdArgAttribute)attributeList[0]).flag;
                    string flagShorthand1 = ((CmdArgAttribute)attributeList[0]).flagShorthand;
                    string description = ((CmdArgAttribute)attributeList[0]).description;

                    if (flagName1 == "--help" || flagShorthand1 == "-h")
                    {
                        this.ErrorMessage = String.Format("--help.-h are reserved flag/shorthand names.");
                        return CmdArgManager.ExitCode.HelpFlagCannotBeInCmdArgsClass;
                    }

                    if (!flagName1.StartsWith("--") || !flagShorthand1.StartsWith("-"))
                    {
                        this.ErrorMessage = String.Format("Property '{0}' has a flag or shorthand that does not have the '--' or '-' prefix.", propName);
                        return CmdArgManager.ExitCode.FlagOrShortHandDoesNotStartWithDash;
                    }

                    if (flagsUsed.ToArray().Contains(flagName1) || (flagsUsed.ToArray().Contains(flagShorthand1)))
                    {
                        this.ErrorMessage = String.Format("Property [{0}] has flag or shorthand ({1}/{2} already assigned.", propName, flagName1, flagShorthand1);
                        return CmdArgManager.ExitCode.DuplicateFlagsPresentInAttribute;
                    }

                    flagsUsed.Add(flagName1);
                    flagsUsed.Add(flagShorthand1);

                    bool flagRequired1 = ((CmdArgAttribute)attributeList[0]).required;

                    this.CmdArgInfoCollection.Add(new CmdArgInfo()
                    {
                        PropertyName = propName,
                        FlagName = flagName1,
                        FlagShortHand = flagShorthand1,
                        PropertyType = propType,
                        Required = flagRequired1,
                        Description = description
                    });
                }
            }

            return CmdArgManager.ExitCode.Success;
        }
    }
}