![ICON](./icon.png) ![LONG_ICON](./long_icon.png)

# SummerFramework

README Languages: English (Current) | [简体中文 (Simplified Chinese)](./locals/README.zh_cn.md) 

Framework for .NET programs

## Features
- Manage variables through writing configuration file.
- Convenient and simple UnitTest.

## Usage

### Object Configuration

Firstly,  complete your global configuration file. `(framework and framework_version aren't required, it mean to be seen by developers)`

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

Secondly, get global object from property `"GLOBAL"` in class `"Configuration"`.

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

If value is empty, it will be created by parameterless constructor.

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

If you want to reuse the variables you created before, you can use `ref(target)`.

### Function Configuration

To configrue a function, you should add section named `methods` to your config file.

and assign its identifier and link.

```json

"methods": {
  {
    "type": "function",
    "identifier": "add",
    "link": "TestSummer.Math@Add"
  }
}

```

You have to use format like `class_name@method_name` to assgin a link.

When you want to invoke this method in section `objects`, you can write `@add(1,1)` in property `value`. (No space!)

### Aspect Injection

Suppose a scene, you have to check if your door is close after you go outside.

It looks like this if in code:

```c#

person.OpenDoor();
person.GoOut();

if (!person.CheckDoorClosed())
  person.CloseDoor();

```

As you can see the action `CloseDoor` is more complicated than others.

In a real develop scene, there are full of complicated logic problems that need to be solve, so there are lots of code reusage!

By using Aspect Injection, you can reduce code reusage.

In order to do something after `GoOut()`, we should set the method `GoOut` into virtual method and mark attribute named `After` and assgin a name of the `AfterAction`.

```c#

using SummerFramework.Core.Aop;

public class Person
{
...
  [After("CloseDoor")]
  public virtual GoOut()
  {
    //TODO: Go out
  }
}
```

Don't forget to add `CloseDoor` in class `AspectHandler`!

```c#

AspectHandler.AddAfter("CloseDoor", () => {
  if (!person.CheckDoorClosed())
  person.CloseDoor();
});

```

And then the logic `CloseDoor` can be invoked automatically!

If you want to pass the value of a expression to another expression, you should't nest invoke but by using pipeline operator (`|>`)

```json

"value": "@add(1,1) |> @sub(&, 1) |> @ mul(&, )1"

```

### UnitTest

SummerFramework has built-in attributes to make sure developers test some functions more conveniently

UnitTest need to take place in class with "TestClass" Attribute, and must mark the method(static) with "Test" Attribute.

Finally, manual run TestController.Run() to enable UnitTest.

```c#

using SummerFramework.Core.UnitTest;

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
