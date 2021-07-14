## How does ArgyBargy work


## 1. The command argument definition class.

1ArgyBargy1 (AB) command line arguments are defined in a special-case class that you create. Decorate every public property in the class that is a command line argument with the `CmdArg` attribute.   

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

    Using CommandLineUtility;

    public class ExampleArgs
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
    }

There is also an implicit flag/shorthand, `--help/-h` that causes ArgyBargy to display a brief help screen screen.    

AB's argument definition classes can have other properties without the `CmdArg` attribute and it can have methods and all the other stuff a .NET class can have. It's probably best to not fan out its purpose too broadly, but there be times when there is merit in making the class do more than just define a command line.

After `ArgyBargy` parses the command line arguments, those properties in the command argument definition class are populated with the corresponding argument value. 

## 2. Parsing the command line

Create an instance of your command argument definition class and of AB's `CmdArgManager` class. The `CmdArgManager` constructor needs three arguments:

1. The instance of your command argument definition class.
2. The `args` array of incoming command line arguments.
3. A brief description that is displayed as the header with AB's --help/-h argument 

Example code:

    ExampleArgs ea = new ExampleArgs();
    CmdArgManager cam = new CmdArgManager(ea, args, "Example command line usage");

    CmdArgManager.ExitCode result = cam.ParseArgs();
    if (result == CmdArgManager.ExitCode.Success)
    {
        ... do work here
    }
    else {
        ... handle error here. The `CmdArgManager` class has a public `ErrorMessage` property that contains the reason for the error.  
    }

You can see many examples of using the `ParseArgs` method in AB's [unit test code.](https://github.com/rogerpence/ArgyBargy/blob/main/TestArgyBargySpike/UnitTest1.cs)


The `ParseArgs` method returns a `CmdArgManager.Exit` enumeration. 

Internals

The `ParseArgs` method does two things

1. It uses reflection to interrogate the command definition class and builds a strongly typed collection of `CmdArgInfo` class instances, one for each property decorated with a `CmdArgs` attribute. 

    public class CmdArgInfo
    {
        public string PropertyName { get; set; }
        public string FlagName { get; set; }
        public string FlagShortHand { get; set; }
        public string PropertyType { get; set; }
        public bool Required { get; set; }
        public string Description { get; set; }
    }

2. Using info from the collection of `CmdArgInfo` instances it performs several comparing the command argument definitions against the values provided on the command line. It checks for:

* Unknown `flags` or `shorthands`
* That all required arguments are present
* That a property's `flag` and `shorthand` aren't both used on the command line 
* That there are duplicates of `flags` or `shorthands` 
