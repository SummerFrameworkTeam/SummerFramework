![ICON](../icon.png) ![LONG_ICON](../long_icon.png)

# SummerFramework

README语言: [English (英语)](../README.md) | 简体中文 (当前) 

为.NET程序准备的框架。

## 特性
- 允许您通过便携配置文件的方式装配对象。
- 快捷易用的单元测试。

## 用法

### 对象装配

首先，补充完您的global_configuration.json文件`(其中framework和framework-version不是必须的，它们用于提醒开发者)`

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

然后您需要调用Configuration类中GLOBAL字段的GetObject(string identifier)方法获取装配好的对象。

```c#

using SummerFramework.Core.Configuration;

var str = (string) Configuration.GLOBAL.GetObject("str");

Console.WriteLine(str);

```
如果您想装配一个引用类型的对象，您可以这样做。(假设我们有一个Person类，里面有string类型的属性Name以及int类型的属性Age)

```json

{
  "type": "Test.Person",
  "identifier": "person"
  "value": ""
}

```

如果您留空了value的值，对象将会由其类型的无参构造装配。

如果您想使用有参构造装配对象，您需要像如下一样做(确保参数顺序相同):

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

如果您想重用您之前创建过的变量值，您可以使用`ref(target)`表达式。

别忘了强制转换成您所需的类型。

### 函数装配

为了配置一个函数，您应该在配置文件中新增`methods`区块。

然后指定其标识符和方法链接。

```json

"methods": {
  {
    "type": "function",
    "identifier": "add",
    "link": "TestSummer.Math@Add"
  }
}

```

您需要使用`类名@方法名`这样的格式指定方法链接，

当您想要在`objects`区块中调用这个函数时，您需将`value`属性写成`@add(1,1)`的形式 (不允许空格间隔)。

如果您想从一个表达式向另一个表达式传递值，您不必嵌套式地调用，而是应当使用管道运算符 (`|>`)。

```json

"value": "@add(1,1) |> @sub(&, 1) |> @ mul(&, )1"

```

`ref()`表达式同样支持！

### 切面注入

想想这样一个场景，您需要在您出门之后检查一下门是否关上了。

如果写成代码，会是如下的样子:

```c#

person.OpenDoor();
person.GoOut();

if (!person.CheckDoorClosed())
  person.CloseDoor();

```

如您所见，关门的逻辑要比其他的逻辑更复杂一些。

在真实的开发场景中有数不胜数的复杂逻辑问题待定解决，故而会有很多代码重复情况！

通过使用切面注入，您便可以减少代码重复。

为了在`GoOut()`运行完之后做一些事情，我们需要将GoOut函数设定为虚方法，并且标记上`After`特性，并指定`AfterAction`的名称

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

别忘记将`CloseDoor` 通过`AspectHandler`添加方法!

```c#

AspectHandler.AddAfter("CloseDoor", () => {
  if (!person.CheckDoorClosed())
  person.CloseDoor();
});

```

然后便可以自动地在GoOut()执行完之后自动执行`CloseDoor`了！

### 延迟化任务系统

想想一下，如果您想在一个方法中使用一个暂时未被创建的对象，同时，您还想用这个对象的属性来做一个占位操作。

延迟化任务系统就是用来解决这个问题的

这套系统允许您通过创建`TaskManager<T>`类实例的方式来为对象占位，并提供了`Identifier`等属性来锁定这个对象。
```c#

using SummerFramework.Core.Task;

...
private TaskManager<object> task_manager;

```

然后您需要知道一些关于 `DeferredTask<T>`类的一些事情:

这个类包含了两个属性 `Identifier (string)`和`Task (Func<T>)`，`Identifier`属性用于确定哪个位置是留给这个对象的，而`Task`属性则用于稍后得到此对象的实际值。

```c#

task_manager.AddTask(new DeferreedTask<object>("deferred_init_object_a", CreateObject());

```

(`CreateObject`是您用来初始化此对象的方法)

And when you think it's a good time to create it, you can use method `Run` or `RunSpecified` in `TaskManager<T>`.
而当您认为某个时刻是创建它的好机会时，您就可以使用 `TaskManager<T>`类中的`Run`或`RunSpecified`方法来的真正初始化了。

`TIPS: 任务以Stack<T>的形势被存放，所以您会遇到最后添加的任务最先被执行的情况`

`var object_real = task_manager.Run();`

当您想要执行某个您想执行的任务时，您应当使用`RunSpecified(string id)`方法，并传入其唯一的标识符。

### 单元测试

SummerFramework内置了一些特性用于保证开发者更方便地测试一些功能 

单元测试应当在被标记了`TestClass`的类中进行，并且要测试的方法必须为静态方法且带有`Test`特性。

最后，在程序主入口中执行`TestController.Run()`来启用单元测试

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
