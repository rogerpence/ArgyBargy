## ArgyBargy -- A simple C# command line argument processor

ArgyBarby (AB) is a simple C# command line argument processor. It uses a class definition to define command line arguments. A custom property attribute enables this class to define meta data that describes the command line arguments. 

> Dependency note: AB uses reflection and .NET's `dynamic` type. You may need to install [this Microsoft C# package](https://www.nuget.org/packages/Microsoft.CSharp/) to use AB. It provides some needed help for using the `dynamic` type with reflection. 

The custom attribute provides this information for an argument property: 

| Argument    | Description                        |
|-------------|:-----------------------------------|
| Flag        | A value that starts with two dashes |
| Shorthand   | A shorthand alternative to the Flag value | Required    | true if argument is required otherwise false |
| Required    | True if the arg is required, otherwise false. |
| Description | A brief description that displays with help |

* **Flag** -  A value that identifies a command line argument. Flags always start with two dashes.
* **Shorthand** - A value that provides a shorthand to the Flag value. Shorthands always start with one dash and is usually one or two characters long. 
* **Required** -- True if the argument is required, otherwise false.
* **Description**  -- A brief description of the argument that is shown when the special case `--help` or `-h` argument is provided. 

The class below defines two required and three optional command line arguments. Types supported are `int`, long `int`, `string`, and `Booleaan`. Beyond defining the command-line arguments, AB populates this class's properties with the command line arguments. 

    using CommandLineUtility;

    public class ExampleArgs
    {
        const bool REQUIRED = true;
        const bool OPTIONAL = false;

        [CmdArg("--databasename", "-d", REQUIRED, "Database name")]
        public string databasename { get; set; }

        [CmdArg("--file", "-f", REQUIRED, "File name")]
        public string file { get; set; }

        [CmdArg("--outputpath", "-p", OPTIONAL, "Output path")]
        public string outputpath { get; set; } = "default output path";

        [CmdArg("--blockfactor", "-b", OPTIONAL, "Recording blocking factor")]
        public int blockfactor { get; set; } = 500;

        [CmdArg("--noheadings", "-nh", OPTIONAL, "Do not include headings row")]
        public bool noheadings { get; set; } = false;
    }

AB's argument definition classes can have other properties without the `CmdArg` attribute and it can have methods and all the other stuff a .NET class can have. It's probably best to not fan out its purpose too broadly, but there be times when there is merit in making the class do more than just define a command line. 

The code below shows how to use the `ExampleArgs` class. If the command arguments parse without errors, after calling the `CmdArgManager` instance's `ParseArgs` method the `ExampleArg's` argument properties are populated. If an error occurred parsing the arguments, `ParseArgs` returns an error value with `ExitCode`. It also puts a brief error message indicating what went wrong in its `ErrorMessage` property

    ExampleArgs ea = new ExampleArgs();
    CmdArgManager cam = new CmdArgManager(ea, args, "Example command line usage");

    CmdArgManager.ExitCode result = cam.ParseArgs();
    if (result == CmdArgManager.ExitCode.Success) {
        .. do your work here.
    }
    else {
        Console.WriteLine(cam.ErrorMessage)
    }

The command line to run a program using the `ExampleArgs` command line argument class is shown below:

    >ArgyBargyExample01.exe --databasename "Production" --file MyFile"
                             --outputpath "c:\output" --blockfactor 500
                             --noheadings 

>Command lines are continued for readability. There are many more command line examples in [AB's unit test file.](https://github.com/rogerpence/ArgyBargy/blob/main/TestArgyBargySpike/UnitTest1.cs)

The command line could have also used the `shorthand` argument identifiers instead of the full `--flag` name, as shown below. You can mix and match `shorthand` and `flag` argument identifiers. 

    >ArgyBargyExample01.exe -d "Production" -f "MyFile" -p "c:\output" 
                            -b 500 -nh

In this example, `--databasename` (`-d`) and `--file` (`-f`) arguments were the two arguments required; the others could have been omitted.

> In the `ExampleArgs` class the property names are the same as the `flag` identifier (except that property names don't start with `--`). It probably usually makes sense to make property names match `flag` names, but it's necessary. The property name can be any legal property value.

Integer and string `flag/shorthand` identifiers must be followed by a value. Boolean `flag/shorthand` identifiers always resolve to `true`. For example, specifying `--noheadings` means the 

## Showing help

There is a `--help/-h` built-in flag/shorthand that displays command line help. An example help panel is shown below:

![](https://asna.com/filebin/marketing/cmd_KJYo4nmvZl.png)

This is help is automatically created from the meta data that AB knows about the command line. 

### The `HelpShown` event

Beyond that canned help, AB also has a `HelpShown` event that you can use to provide additional, application-specific help. 

To use this event, provide an event handler that has `ShowHelpEventArgs` as its second argument. The current `CmdArgManager` instance is avialable by casting the `sender` argument as `CmdArgManager` and the current argument definition instance is available by casting the `e.CmdArgs` to the argument definition class (in the example below `ExporterArgs`). You can do any console output needed in your `HelpShown` event handler.

> Remember that command line arguments haven't yet been processed when the `HelpShown` event is raised. 

The code below assumes the command argument definition class is named `ExportArgs` and it has a public `blockfactor` property.

The code below shows an example C# `HelpShown` event handler:

    public void HelpShownHandler(object sender, ShowHelpEventArgs e)
    {
        // Show that you can get to the CmdArgManager instance.
        Console.WriteLine(((CmdArgManager)sender).Description);

        // Show that you can get to the arg definition instance.
        Console.WriteLine(((ExporterArgs)e.CmdArgs).blockfactor);
    }

To wire up the event handler in C#, assign it to the `HelpShown` property after instancing the `CmdArgManager` class:

    CmdArgManager cam = new CmdArgManager(ea, args, "Test command line parser");
    cam.HelpShown += HelpShownHandler;

The code below shows an example AVR event handler:

    BegSr HelpShownHandler 
        DclSrParm Sender Type(*Object) 
        DclSrparm e Type(ShowHelpEventArgs) 

        // Show that you can get to the CmdArgManager instance.
        Console.WriteLine((sender *As CmdArgManager).Description);

        // Show that you can get to the arg definition instance.
        Console.WriteLine((e.CmdArgs *As ExportArgs).blockfactor);
    EndSr 

To wire up the event handler in AVR you use the AddHandler operation after instancing the `CmdArgManager` class:

    DclFld cam Type(CmdArgManager)
    cam = *New CmdArgManager(ea, args, "Test command line parser")

    AddHandler SourceObject(cam) SourceEvent(HelpShown) +
               HandlerObject(*This) HandlerSr(HelpShownHandler) 

Here is an example detailed help screen: 

![](https://asna.com/filebin/marketing/cmd_qnNRoYeLsB.png)

It uses this AVR handler (the text has been truncated for publication purposes)

    BegSr HelpShownHandler 
        DclSrParm Sender Type(*Object) 
        DclSrparm e Type(ShowHelpEventArgs) 

        DclFld CmdLine Type(CommandLineArgs) 
        DclFld RootPath Type(*String) 

        CmdLine = e.CmdArgs *As CommandLineArgs 

        RootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        CustomConsole.WriteLineInfo(String.Empty)
        CustomConsole.WriteLineInfo("The default schema output path is:") 
        CustomConsole.WriteLineInfo("  " + PathJoin(RootPath, CmdLine.OutputPath))
        CustomConsole.WriteLineInfo("The default output path is the ...")
        CustomConsole.WriteLineInfo("To change the output path at ...")
        CustomConsole.WriteLineInfo("The runtime output path must exist.")
    EndSr 

and the handler is wired up like this:

    cam = *New CommandLineUtility.CmdArgManager(CmdLine, args, +
            "Generate DataGate file schemas for a library")      
    AddHandler SourceObject(cam) SourceEvent(HelpShown) +
            HandlerObject(*This) HandlerSr(HelpShownHandler) 

The AVR code above uses this `CommandLineArgs` command argument definition class:            

    BegClass CommandLineArgs Access(*Public) 

        DclProp DatabaseName Type(*String) Access(*Public) +
                            Attributes(CmdArg("--databasename", +
                                "-d", *True,  "Database Name"))

        DclProp LibraryName Type(*String)  Access(*Public) +
                            Attributes(CmdArg("--library", +
                                "-l", *True, +
                                "Library name (or *LIBL if using a library list)"))

        DclProp OutputPath Type(*String)  Access(*Public) +
                            Attributes(CmdArg("--outputpath", +
                                "-op", *False, +
                                "output path (appended to the user's Documents folder)"))

        DclProp YAML Type(*Boolean)  Access(*Public) +
                            Attributes(CmdArg("--yaml", "-y", +
                                *False, "Write schema in YAML instead of Json"))       

        DclProp Pause Type(*Boolean)  Access(*Public) +
                            Attributes(CmdArg("--pause", "-p",+
                                *False, "Pause the screen after processing"))
                            
        BegConstructor Access(*Public) 
            *This.Pause = *False 
            *This.OutputPath = +
                System.Configuration.ConfigurationManager.AppSettings['defaultOutputPath']
        EndConstructor 
    EndClass


### Miscellaneous notes

* **Escaped minus signs**. As AB parses the command line, it recognizes flags and shorthand identifiers because they start with a dash (-). Therefore if a value needs to start with minus sign, the minus sign needs to be escaped with a backslash (\). For example, after using the command line below:

    >ArgyBargyExample01.exe -d "Production" -f "MyFile" -p "c:\output" 
                            -b \-500 -nh

    the `blockfactor` value will be -500. Escaping minus signs applies to string and integer values. 

* **Quote values with embedded blanks**. When values have embedded blanks, be sure to delimit them with double quotation marks. 

    ArgyBargyExample01.exe -d "Speed Racer" -f "MyFile" -p "c:\output" 

If a quoted string needs an embedded double-quote, escape that double quote with a backslash (\\). For example, the following line 

    ArgyBargyExample01.exe -d "\"Smokey\" Yunick" -f "MyFile" -p "c:\output" 

resolves `-d's` value to '"Smokey" Yunick'.

### Colorized output 

For consistent console coloring, there is a [CustomConsole class](https://github.com/rogerpence/ArgyBargy/blob/main/ArgyBargy/ConsoleColors.cs) available that provides these static methods: 

    WriteLineError()          // Write red text
    WriteLineSuccess()        // Write green text  
    WriteLineInfo()           // White cyan text

Like the `Console` object's `WritLine` these methods have overloads to directly support using `String.Format` with them. For example:

    CustomConsole.WriteLineError('Hello, {0}', 'world'})

prints "Hello, world" to the console in the error color.     

These methods are available to you to use in your apps that use the `CommandLineUtility` namespace. 

### .NET Framework version. 

This library requires at a minimum .NET Framework version 4.6.1. 



