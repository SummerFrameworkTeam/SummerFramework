using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using SummerFramework.Core.Aop;

namespace SummerFramework.Base.DynamicProxy;

public class DynamicProxyBuilder<T> where T : IInterceptor, new()
{
    public static readonly AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicProxys"), AssemblyBuilderAccess.Run);
    public static readonly ModuleBuilder module = assembly.DefineDynamicModule("DynamicProxys");

    public static Type? Build(Type tt)
    {
        if (tt.IsInterface)
            return null;

        // Define proxy class
        TypeBuilder rt = module.DefineType($"{tt.Name}_{Guid.NewGuid():N}_DyProxy", TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit, tt, Array.Empty<Type>());

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        foreach (var tm in tt.GetMethods(flags))
        {
            if (tm.IsVirtual && !tm.IsStatic && !tm.IsFinal && !tm.IsAssembly && 
                tm.GetCustomAttribute<AspectAttribute>() != null)
            {
                var dt = TypeExtractor.GetDelegateType(tm, out var paramTypes, out var returnType, out var paramInfo);
                var it = typeof(T);

                var rm = rt.DefineMethod(tm.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig, returnType, paramTypes);

                for (var i = 0; i < paramInfo.Length; i++)
                {
                    var p = rm.DefineParameter(i + 1, paramInfo[i].Attributes, paramInfo[i].Name);

                    if (paramInfo[i].HasDefaultValue)
                    {
                        p.SetConstant(paramInfo[i].DefaultValue);
                    }
                }

                var il = rm.GetILGenerator();

                Label label = il.DefineLabel();

                //0-: object[] Parameter;
                il.DeclareLocal(typeof(object[]));
                //1-: Type delegate;
                il.DeclareLocal(dt);
                //2-: Invocation invocation;
                il.DeclareLocal(typeof(Invocation));
                //3-: T Interceptor interceptor
                il.DeclareLocal(it);

                // Define return type
                LocalBuilder re = null;

                if (returnType != typeof(void))
                    re = il.DeclareLocal(returnType);

                // Push a typeof Int32, whose value is paramTypes.Length
                il.Emit(OpCodes.Ldc_I4, paramTypes.Length); 
                // Push a typeof new array of object
                il.Emit(OpCodes.Newarr, typeof(object)); 
                // Set the value of loc[0]"parameters" with above mentioned
                il.Emit(OpCodes.Stloc, 0);

                for (var i = 0; i < paramTypes.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, 0);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldarg, i + 1);

                    if (paramTypes[i].IsValueType)
                        il.Emit(OpCodes.Box, paramTypes[i]);

                    il.Emit(OpCodes.Stelem_Ref);
                }

                // Load function"rm" and load its agruments
                il.Emit(OpCodes.Ldarg, 0);
                il.Emit(OpCodes.Ldftn, tm);
                // loc[1]"delegate" = new(); 
                il.Emit(OpCodes.Newobj, dt.GetConstructors()[0]);
                il.Emit(OpCodes.Stloc, 1);
                // loc[2]"invocation" = new(); 
                il.Emit(OpCodes.Newobj, typeof(Invocation).GetParameterlessConstructor());
                il.Emit(OpCodes.Stloc, 2);
                // loc[2]"invocation".set_Parameters(loc[0]"parameters");
                // Or: loc[2]"invocation".Parameters = loc[0]"parameters";
                il.Emit(OpCodes.Ldloc, 2);
                il.Emit(OpCodes.Ldloc, 0);
                il.Emit(OpCodes.Callvirt, typeof(Invocation).GetMethod("set_Parameters")!);
                // loc[2]"invocation".set_Callee(loc[1]"delegate");
                // Or: loc[2]"invocation".Callee = loc[1]"delegate";
                il.Emit(OpCodes.Ldloc, 2);
                il.Emit(OpCodes.Ldloc, 1);
                il.Emit(OpCodes.Callvirt, typeof(Invocation).GetMethod("set_Callee")!);
                // interceptor = new AspectInterceptor();
                il.Emit(OpCodes.Newobj, it.GetParameterlessConstructor());
                il.Emit(OpCodes.Stloc, 3);
                // loc[3]"invocation".Intercept(loc[2]"invocation");
                il.Emit(OpCodes.Ldloc, 3);
                il.Emit(OpCodes.Ldloc, 2);
                il.Emit(OpCodes.Callvirt, it.GetMethod("Intercept")!);

                if (returnType != typeof(void))
                {
                    il.Emit(OpCodes.Castclass, returnType);
                    il.Emit(OpCodes.Stloc_S, re!);
                    il.Emit(OpCodes.Br_S, label);
                    il.MarkLabel(label);
                    il.Emit(OpCodes.Ldloc_S, re!);
                }
                else
                {
                    il.Emit(OpCodes.Pop);
                }
                // Return value
                il.Emit(OpCodes.Ret);
            }
        }
        var result = rt.CreateType();
        return result;
    }
}
