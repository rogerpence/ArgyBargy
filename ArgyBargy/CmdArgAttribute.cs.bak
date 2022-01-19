
namespace CommandLineUtility
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class CmdArgAttribute : System.Attribute
    {
        public string flag;
        public string flagShorthand;
        public bool required;
        public string description;

        public CmdArgAttribute(string flag, string flagShorthand, bool required, string description)
        {
            this.flag = flag;
            this.flagShorthand = flagShorthand;
            this.required = required;
            this.description = description;
        }

        public CmdArgAttribute(string flag, bool required, string description)
        {
            this.flag = flag;
            this.flagShorthand = null;
            this.required = required;
            this.description = description;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class CmdValuesAttribute : System.Attribute
    {
        public string values;

        public CmdValuesAttribute(string values)
        {
            this.values = values;
        }
    }




}