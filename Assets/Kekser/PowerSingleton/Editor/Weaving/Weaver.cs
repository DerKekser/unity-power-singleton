using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class Weaver
{
    private readonly DefaultAssemblyResolver _assemblyResolver;
    private readonly AssemblyDefinition _assemblyDefinition;
    
    private readonly Type[] _types;

    public Weaver(DefaultAssemblyResolver assemblyResolver, AssemblyDefinition assemblyDefinition, Type[] types)
    {
        _assemblyResolver = assemblyResolver ?? throw new ArgumentNullException(nameof(assemblyResolver));
        _assemblyDefinition = assemblyDefinition ?? throw new ArgumentNullException(nameof(assemblyDefinition));

        _types = types;
        
        if (types == null || types.Length == 0)
            throw new ArgumentException("Types array cannot be null or empty.");
    }
    
    public bool Weave()
    {
        if (_assemblyDefinition == null)
            return false;

        return WeaveTypesIntoAssembly(_types);
    }

    private bool WeaveTypesIntoAssembly(Type[] types)
    {
        TypeReference typeReference = _assemblyDefinition.MainModule.ImportReference(typeof(Type));
        var typesArray = new TypeReference[types.Length];

        for (int i = 0; i < types.Length; i++)
            typesArray[i] = _assemblyDefinition.MainModule.ImportReference(types[i]);
        
        
        TypeDefinition managerType = _assemblyDefinition.MainModule.Types.FirstOrDefault(t => t.Name == "PowerSingletonManager");
        if (managerType == null)
            return false;
            
        MethodDefinition cctor = managerType.Methods.FirstOrDefault(m => m.IsConstructor && m.IsStatic);
        if (cctor == null)
            return false;
        
        var ilProcessor = cctor.Body.GetILProcessor();
        
        List<Instruction> instructions = new List<Instruction>();
        instructions.Add(ilProcessor.Create(OpCodes.Ldc_I4, typesArray.Length));
        instructions.Add(ilProcessor.Create(OpCodes.Newarr, typeReference));
        
        for (int i = 0; i < typesArray.Length; i++)
        {
            instructions.Add(ilProcessor.Create(OpCodes.Dup));
            instructions.Add(ilProcessor.Create(OpCodes.Ldc_I4, i));
            instructions.Add(ilProcessor.Create(OpCodes.Ldtoken, typesArray[i]));
            instructions.Add(ilProcessor.Create(OpCodes.Call, _assemblyDefinition.MainModule.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"))));
            instructions.Add(ilProcessor.Create(OpCodes.Stelem_Ref));
        }
        
        instructions.Add(ilProcessor.Create(OpCodes.Stsfld, managerType.Fields.First(f => f.Name == "_types")));
        instructions.Add(ilProcessor.Create(OpCodes.Ret));
        
        ilProcessor.InsertBefore(ilProcessor.Body.Instructions[ilProcessor.Body.Instructions.Count - 1], instructions[0]);
        Instruction lastInstruction = instructions[0];
        for (int i = 1; i < instructions.Count; i++)
        {
            ilProcessor.InsertAfter(lastInstruction, instructions[i]);
            lastInstruction = instructions[i];
        }
        
        return true;
    }
}
