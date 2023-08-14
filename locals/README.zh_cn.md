![ICON](../showing_image.png)

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/dotnetcore/CAP/master/LICENSE.txt)
[![NuGet](http://img.shields.io/nuget/v/SummerFramework.svg)](https://www.nuget.org/packages/SummerFramework)
[![Downloads](https://img.shields.io/nuget/dt/SummerFramework)](#)

# SummerFramework

README语言: [English (英语)](../README.md) | 简体中文 (当前) 

Documents: [English](../docs/en_us.md) | [简体中文](../docs/zh_cn.md)

为.NET开发者准备的应用程序框架。

## 扩展包/插件

SummerFramework将会允许并搭理支持SummerFramework相关扩展/插件的开发。

### 官方扩展包/插件

![SFC_ICON](https://github.com/TheWhistlers/SummerFramework.Commandline/blob/main/sfcmd_icon.png)

- #### [SummerFramework.Commandline](https://github.com/TheWhistlers/SummerFramework.Commandline)

    为.NET开发者做准备的命令行工具开发框架，使您的开发更加高效，并且可以脱离SummerFramework使用，不过还是推荐您一同使用。

## 特性
- 允许您通过便携配置文件的方式装配对象。
- 轻量且易用
- 内置了简单易用的单元测试。

## 快速开始
1. 使用Nuget包管理工具下载SummerFramework最新版本。

2. 引入`SummerFramework`命名空间以及其他您所需的(子)命名空间。

3. 创建一个配置文件，

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

使用其路径创建一个基于资源的配置上下文对象，然后使用对象的标识符得到它并打印。

```c#

using SummerFramework.Core.Configuration;
using SummerFramework.Base;

var context = ResourceBasedConfigurationContext.Create("global_configuration.json");
context.GetObject("str")?.Log();

```

4. 在控制台中您应该就可以看到（绿色的）"Hello Summer!"字样了！

![CONSOLE](../docs/assets/console.png)
