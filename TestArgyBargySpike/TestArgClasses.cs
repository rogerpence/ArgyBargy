using CommandLineUtility;

namespace TestArgyBargySpike
{
    public class ExporterArgs
    {
        const bool REQUIRED = true;
        const bool OPTIONAL = false;

        [CmdArg("--databasename", "-d", REQUIRED, "Database name")]
        public string databasename { get; set; }

        [CmdArg("--library", "-l", REQUIRED, "Library")]
        public string library { get; set; }

        [CmdArg("--file", "-f", REQUIRED, "File name")]
        public string file { get; set; }

        [CmdArg("--outputpath", "-p", OPTIONAL, "Output path")]
        public string outputpath { get; set; } = "default output path";

        [CmdArg("--blockfactor", "-b", OPTIONAL, "Recording blocking factor")]
        public int blockfactor { get; set; } = 500;

        [CmdArg("--noheadings", "-nh", OPTIONAL, "Do not include headings row")]
        public bool noheadings { get; set; } = false;

        [CmdArg("--tabdelimiter", "-t", OPTIONAL, "Use tab as field delimiter")]
        public bool tabdelimiter { get; set; } = false;

        [CmdArg("--showprogress", "-x", OPTIONAL, "Show export progress")]
        public bool showprogress { get; set; } = false;

        [CmdArg("--writeschemafile", "-s", OPTIONAL, "Write schema file")]
        public bool writeschemafile { get; set; } = false;


        // A non-argument property just to show that is allowed.
        public string fullname { get; set; }

        // A non-supported property type--to test that this type is ignored by ArgyBargy
        // for properties not decorated with CmdArg attribute.
        public decimal tester {get; set;}
    }

    public class HelpErrorArgs
    {
        const bool REQUIRED = true;
        const bool OPTIONAL = false;

        [CmdArg("--library", "-l", REQUIRED, "Library")]
        public string library { get; set; }

        [CmdArg("--help", "-h", OPTIONAL, "Library")]
        public string help { get; set; }
    }
}
