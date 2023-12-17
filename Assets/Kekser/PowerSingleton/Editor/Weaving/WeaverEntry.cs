#if !UNITY_2020_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kekser.PowerSingleton.Attributes;
using Mono.Cecil;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;
using UnityAssembly = UnityEditor.Compilation.Assembly;

namespace Kekser.PowerSingleton.Weaving
{
    public static class WeaverEntry
    {
        private const string Weaved = "PowerSingletonWeaved";
        
        [InitializeOnLoadMethod]
        public static void OnInitializeOnLoad()
        {
            Debug.Log("Weaver: InitializeOnLoad");
            CompilationPipeline.assemblyCompilationFinished += OnCompilationFinished;

            if (SessionState.GetBool(Weaved, false)) 
                return;
            
            SessionState.SetBool(Weaved, true);
            WeaveExistingAssemblies();
        }
        
        [MenuItem("Kekser/PowerSingleton/Weave")]
        public static void Weave()
        {
            Debug.Log("Weaver: Weave");
            SessionState.SetBool(Weaved, true);
            WeaveExistingAssemblies();
        }
        
        public static void WeaveExistingAssemblies()
        {
            foreach (UnityAssembly assembly in CompilationPipeline.GetAssemblies())
            {
                if (!File.Exists(assembly.outputPath)) 
                    continue;
                OnCompilationFinished(assembly.outputPath, new CompilerMessage[0]);
            }

            EditorUtility.RequestScriptReload();
        }

        private static UnityAssembly FindCompilationPipelineAssembly(string assemblyName) =>
            CompilationPipeline.GetAssemblies().First(assembly => assembly.name == assemblyName);

        private static bool CompilerMessagesContainError(CompilerMessage[] messages) =>
            messages.Any(msg => msg.type == CompilerMessageType.Error);

        public static void OnCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            Debug.Log("Weaver: start");
            if (CompilerMessagesContainError(messages))
            {
                Debug.Log("Weaver: stop because compile errors on target");
                return;
            }

            if (assemblyPath.Contains("-Editor") || 
                (assemblyPath.Contains(".Editor") && !assemblyPath.Contains(".Tests")))
                return;

            string unityEngineCoreModuleDLL = UnityEditorInternal.InternalEditorUtility.GetEngineCoreModuleAssemblyPath();
            if (string.IsNullOrEmpty(unityEngineCoreModuleDLL))
            {
                Debug.LogError("Failed to find UnityEngine assembly");
                return;
            }

            HashSet<string> dependencyPaths = GetDependencyPaths(assemblyPath);
            dependencyPaths.Add(Path.GetDirectoryName(unityEngineCoreModuleDLL));

            WeaveFromFile(assemblyPath, dependencyPaths.ToArray()));
        }
        
        private static HashSet<string> GetDependencyPaths(string assemblyPath)
        {
            HashSet<string> dependencyPaths = new HashSet<string>
            {
                Path.GetDirectoryName(assemblyPath)
            };
            foreach (UnityAssembly assembly in CompilationPipeline.GetAssemblies())
            {
                if (assembly.outputPath != assemblyPath)
                    continue;
                foreach (string reference in assembly.compiledAssemblyReferences)
                    dependencyPaths.Add(Path.GetDirectoryName(reference));
            }

            return dependencyPaths;
        }
        
        private static bool WeaveFromFile(string assemblyPath, string[] dependencies)
        {
            // resolve assembly from stream
            using (DefaultAssemblyResolver asmResolver = new DefaultAssemblyResolver())
            using (AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters{ ReadWrite = true, ReadSymbols = true, AssemblyResolver = asmResolver }))
            {
                // add this assembly's path and unity's assembly path
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
                asmResolver.AddSearchDirectory(UnityEngineDllDirectoryName());

                // add dependencies
                if (dependencies != null)
                {
                    foreach (string path in dependencies)
                        asmResolver.AddSearchDirectory(path);
                }

                Type[] types = LookUpAttributes();
                if (types.Length <= 0)
                    return true;
                
                //weave assembly
                Weaver weaver = new Weaver(asmResolver, assembly, types);
                if (!weaver.Weave())
                    return false;
                assembly.Write(new WriterParameters { WriteSymbols = true });
            }
        }
            
        private static string UnityEngineDllDirectoryName()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            return directoryName?.Replace(@"file:\", "");
        }
        
        private static Type[] LookUpAttributes()
        {
            List<Type> types = new List<Type>();
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.FullName.Contains("Editor"))
                    continue;

                foreach (Type type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(PowerSingletonAttribute), false);
                    if (attributes.Length <= 0) continue;
                    
                    if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        Debug.LogErrorFormat("PowerSingletonManager: Type {0} is not a MonoBehaviour", type);
                        continue;
                    }
                    
                    types.Add(type);
                }
            }
            
            return types.ToArray();
        }
    }
}
#endif