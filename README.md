![ICON](./icon.png) ![LONG_ICON](./long_icon.png)

# SummerFramework

Framework for .net programs

## Figures
- Manage variables through writing configuration file.
- Convenient and simple UnitTest.

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

If you want to create a reference type, you can do like this.(Supposed that we have a class named Person in the namespace named Test, and it has two properties named Name(string) and Age(int))

```json

{
  "type": "Test.Person",
  "identifier": "person"
  "value": ""
}

```

If value is empty, it will be created by constructor without any parameters.

If you want to create with constructor with parameters, you need to do like this.

```json

{
  "type": "Test.Person",
  "identifier": "person"
  "value": [
    {
      "type": "string",
      "value": "Dave"
    },
    {
      "type": "int",
      "value": 18
    }
  ]
}

```

If you want to reuse the variables you created before, you can use "ref(target)" expression

For the value types, thier value will be clone.

And for the reference types, their reference will be clone.

## UnitTest

SummerFramework has built-in attributes to make sure developers test some functions more conveniently

UnitTest need to take place in class with "TestClass" Attribute, and must mark the method(static) with "Test" Attribute.

Finally, manual run TestController.Run() to enable UnitTest.

```c#

using SummerFramework.Core.Test;

[TestClass]
public class Program
{
  static void Main(string[] args)
  {
    TestController.Run();
  }

  [Test]
  public static void Test1()
  {
    // TODO
  }
}

```
