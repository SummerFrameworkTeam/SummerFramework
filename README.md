# SummerFramework

Framework for .net programs

## Figures
- Manage variables through writing configuration file.


## Usage

Firstly,  complete your global configuration file. (framework and framework_version aren't required, it mean to be seen by developers)

```json

{
  "framework": "summerframewrok",
  "framework_version": "alpha-1",
  "objects": [
    {
      "type": "string",
      "identifier": "str",
      "value": "Hello Summer!"
    }
  ]
}

```

Secondly, get global object from property "GLOBAL" in class "Configuration".

Don't forget to convert！

```c#

using SummerFramework.Core.Configuration;

var str = (string) Configuration.GLOBAL.GetObject("str");

Console.WriteLine(str);

```
