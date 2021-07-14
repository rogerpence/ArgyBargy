using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandLineUtility;

namespace TestArgyBargySpike
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_help_shown_event()
        {
            string[] args = new string[] { "--help" };

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");
            cam.HelpShown += HelpShownHandler;

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreEqual(CmdArgManager.ExitCode.HelpShown, result);
        }

        public void HelpShownHandler(object sender, ShowHelpEventArgs e)
        {
            // Show that you can get to the CmdArgManager instance.
            Console.WriteLine(((CmdArgManager)sender).Description);
            // Show that you can get to the arg definition instance.
            Console.WriteLine(((ExporterArgs)e.CmdArgs).blockfactor);
        }

        [TestMethod]
        public void Test_show_help()
        {
            string[] args = new string[] { "--help" };

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreEqual(CmdArgManager.ExitCode.HelpShown, result);
        }

        [TestMethod]
        public void Test_simple_use_with_shorthands_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", "cmastnewL2"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreEqual(CmdArgManager.ExitCode.Success, result);
            Assert.AreEqual(cam.ArgsValuePairs["-d"], args[1]);
            Assert.AreEqual(cam.ArgsValuePairs["-l"], args[3]);
            Assert.AreEqual(cam.ArgsValuePairs["-f"], args[5]);

            Assert.AreEqual(args[1], ea.databasename);
            Assert.AreEqual(args[3], ea.library);
            Assert.AreEqual(args[5], ea.file);
        }

        [TestMethod]
        public void Test_empty_shorthand()
        {
            string[] args = new string[] { "-", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", "cmastnewL2"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }

        [TestMethod]
        public void Test_empty_flag()
        {
            string[] args = new string[] { "--", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", "cmastnewL2"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }




        [TestMethod]
        public void Test_use_all_flags_with_tabdelimiter_error_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "--showprogress",
                                           "tabdelimiter",
                                           "--writeschemafile",
                                           "-l", "examples",
                                           "-f", "cmastnewL2",
                                           "--noheadings"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }

        [TestMethod]
        public void Test_use_all_flags_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "--showprogress",
                                           "--tabdelimiter",
                                           "--writeschemafile",
                                           "-l", "examples",
                                           "-f", "cmastnewL2",
                                           "--noheadings"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreEqual(CmdArgManager.ExitCode.Success, result);
            Assert.AreEqual(args[1], ea.databasename);
            Assert.AreEqual(true, ea.showprogress);
            Assert.AreEqual(true, ea.tabdelimiter);
            Assert.AreEqual(true, ea.writeschemafile);
            Assert.AreEqual(args[6], ea.library);
            Assert.AreEqual(args[8], ea.file);
            Assert.AreEqual(true, ea.noheadings);
        }

        [TestMethod]
        public void Test_simple_use_with_flags_with_exporter_args()
        {
            string[] args = new string[] { "--databasename", "*Public/DG NET Local",
                                           "--library", "examples",
                                           "--file", "cmastnewL2"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreEqual(CmdArgManager.ExitCode.Success, result);
            Assert.AreEqual(cam.ArgsValuePairs["--databasename"], args[1]);
            Assert.AreEqual(cam.ArgsValuePairs["--library"], args[3]);
            Assert.AreEqual(cam.ArgsValuePairs["--file"], args[5]);

            Assert.AreEqual(args[1], ea.databasename);
            Assert.AreEqual(args[3], ea.library);
            Assert.AreEqual(args[5], ea.file);
        }

        [TestMethod]
        public void Test_int_argument()
        {
            string[] args = new string[] { "--databasename", "*Public/DG NET Local",
                                           "--library", "examples",
                                           "--file", "cmastnewL2",
                                           "--blockfactor", "500f",
                                           "--noheadings"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            //Assert.AreEqual(cam.ArgsValuePairs["--databasename"], args[1]);
            //Assert.AreEqual(cam.ArgsValuePairs["--library"], args[3]);
            //Assert.AreEqual(cam.ArgsValuePairs["--file"], args[5]);

            //Assert.AreEqual(args[1], ea.databasename);
            //Assert.AreEqual(args[3], ea.library);
            //Assert.AreEqual(args[5], ea.file);
        }

        [TestMethod]
        public void Test_flag_with_escaped_minus_with_exporter_args()
        {
            string[] args = new string[] { "--databasename", "*Public/DG NET Local",
                                           "--library", "\\-examples",
                                           "--file", "cmastnewL2"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreEqual(CmdArgManager.ExitCode.Success, result);
            Assert.AreEqual(cam.ArgsValuePairs["--databasename"], args[1]);
            Assert.AreEqual(cam.ArgsValuePairs["--library"], args[3]);
            Assert.AreEqual(cam.ArgsValuePairs["--file"], args[5]);

            Assert.AreEqual(args[1], ea.databasename);
            Assert.AreEqual("-examples", ea.library);
            Assert.AreEqual(args[5], ea.file);
        }

        [TestMethod]
        public void Test_flag_must_be_numeric_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", "cmastnewL2",
                                           "--blockfactor", "xxx"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
        }

        [TestMethod]
        public void Test_numeric_flag_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", "cmastnewL2",
                                           "--blockfactor", "450"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreEqual(CmdArgManager.ExitCode.Success, result);
            Assert.AreEqual(450, ea.blockfactor);
        }

        [TestMethod]
        public void Test_a_boolean_flag_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", "cmastnewL2",
                                            "--noheadings"
            };

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreEqual(CmdArgManager.ExitCode.Success, result);
            Assert.AreEqual(cam.ArgsValuePairs["-d"], args[1]);
            Assert.AreEqual(cam.ArgsValuePairs["-l"], args[3]);
            Assert.AreEqual(cam.ArgsValuePairs["-f"], args[5]);

            Assert.AreEqual(args[1], ea.databasename);
            Assert.AreEqual(args[3], ea.library);
            Assert.AreEqual(args[5], ea.file);
            Assert.AreEqual(true, ea.noheadings);
        }

        [TestMethod]
        public void Test_omit_a_flag_value_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", 
                                            "--noheadings"
            };

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }



        [TestMethod]
        public void Test_omit_a_required_arg_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "-l", "examples"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }

        [TestMethod]
        public void Test_for_flag_and_shortand_used_on_command_line_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Lo`cal",
                                           "-l", "examples",
                                           "-f", "cmastnewL2",
                                           "--library", "production" };

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }

        [TestMethod]
        public void Test_value_for_flag_and_shortand_missing_in_middle_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Lo`cal",
                                           "-f",
                                           "-l", "examples" };

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }

        [TestMethod]
        public void Test_value_for_flag_and_shortand_missing_at_end_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Lo`cal",
                                           "-l", "examples" ,
                                           "-f"};

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }

        [TestMethod]
        public void Test_duplicate_flag_and_shorthands_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", "cmastnewL2",
                                           "--library", "production" };

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }

        [TestMethod]
        public void Test_escaped_numeric_value()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", "cmastnewL2",
                                           "--blockfactor", "\\-500" };

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreEqual(CmdArgManager.ExitCode.Success, result);
            Assert.AreEqual(ea.blockfactor, -500);
        }


        [TestMethod]
        public void Test_for_unknown_flag_or_shorthand_on_command_line_with_exporter_args()
        {
            string[] args = new string[] { "-d", "*Public/DG NET Local",
                                           "-l", "examples",
                                           "-f", "cmastnewL2",
                                           "-j", "value",
                                           "--debug", "production" };

            ExporterArgs ea = new ExporterArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
            Assert.AreNotEqual(CmdArgManager.ExitCode.Success, result);
            Console.WriteLine(cam.ErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_for_help_property()
        {
            string[] args = new string[] { "-l", "examples" };

            HelpErrorArgs ea = new HelpErrorArgs();
            CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");

            CmdArgManager.ExitCode result = cam.ParseArgs();
        }
    }
}
