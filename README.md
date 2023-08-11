![ICON](./showing_image.png)

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/dotnetcore/CAP/master/LICENSE.txt)
[![NuGet](http://img.shields.io/nuget/v/SummerFramework.svg)](https://www.nuget.org/packages/SummerFramework) [![Downloads](https://img.shields.io/nuget/dt/SummerFramework)](#)

# SummerFramework

README Languages: English (Current) | [简体中文 (Simplified Chinese)](./locals/README.zh_cn.md) 

Framework for .NET programs in order to make development easier and faster!

## Features
- Manage obejcts through writing configuration file or attributes.
- Lightweight and easy to use.
- Convenient and simple UnitTest.

## Documents

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

"methods": [
  {
    "type": "s_func",
    "identifier": "add",
    "link": "TestSummer.Math@Add"
  }
]

```

You have to use format like `class_name@method_name` to assgin a link.

When you want to invoke this method in section `objects`, you can write `@add(1,1)` in property `value`. (No space!)

If you want to pass the value of a expression to another expression, you should't nest invoke but by using pipeline operator (`|>`).

```json

"value": "@add(1,1) |> @sub(&, 1) |> @ mul(&, 1)"

```

`ref()`expression is also supported!

How can you do when you want to link a instance method?

Just add a new property whose name is `invoked` and value is a `ref(target)` to your method object.

```json

"methods": [
  {
    "type": "i_func",
    "identifier": "add",
    "link": "TestSummer.Math@Add",
    "invoked": "ref(math_instance)" 
  }
]

```

### 利用属性注入的上下文配置

Even if it's flexible and easy to manage the whole project. But it will be slow, inefficient and unreadable as the project is growing bigger.

So we provided class `AttributiveConfigurationContext` to make you configure more conveniently and efficiently by creating configuration class.

```c#

using SummerFramework.Core.Configuration.Attributes;

public static class Config
{
  [ConfiguredObject("str")]
  public static string STR { get; set; } = "Hello Summer!";
}

```

As you can see, the attribute `ConfiguredObject` can only be used on property. Because property is safe.

Attribute`ConfiguredObject` allow you to add this property into the pool by giving an id.

And actually you can do this to configure reference types with parameterless constructor and value types.

But when you want to configure a reference types with parametric constructor, you must do like this: 

```c#

[ConfiguredObject("name")]
public static string STR { get; set; } = "Mike";

[ConfiguredObject("person")]
[ConfiguredParameter(typeof(string), "ref(name)")]
[ConfiguredParameter(typeof(int), "18")]
public static Person PERSON { get; set; }

```

You can just need to create an instance of `AttributiveConfigurationContext` and use it as other context objects.
```c#

var context = AttributiveConfigurationContext.Create<Config>();
// or var context = AttributiveConfigurationContext.Create(typeof(Config));

var person  = (Person)context.GetObject("perosn");

Console.WriteLine(person.ToString());
//out(formatted): Person[Name: Mike | Age: 18 ]
```

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

### Deferred Task System

Imagine that, if you want to use a object that has't created in a method, meanwhile, you also want to hold a place for it by its property.

Deferred Task System was built to solve this problem.

It allows you to hold a place for the object and provide a propety named `Identifier` to ascertain it by creating a instance of class `TaskManager<T>`.

```c#

using SummerFramework.Core.Task;

...
private TaskManager<object> task_manager;

```

Then you need to know something about class `DeferredTask<T>`:

It has two propeties `Identifier (string)` and `Task (Func<T>)`,  `Identifier` is used to ascertain which place is for this object, `Task` is used to get the value of this object.

```c#

task_manager.AddTask(new DeferredTask<object>("deferred_init_object_a", CreateObject());

```

(`CreateObject()` refers to the method you created for initing this object)

And when you think it's a good time to create it, you can use method `Run` or `RunSpecified` in `TaskManager<T>`.

`TIPS: The tasks is stored in a Stack<T>, that's why the last task you added will invoke firstly when you invoke task_manager.Run()`

`var object_real = task_manager.Run();`

When you hope you can run the task you want, you should use `RunSpecified(string id)`.


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
