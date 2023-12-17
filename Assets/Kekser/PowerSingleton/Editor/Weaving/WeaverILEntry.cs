#if UNITY_2020_3_OR_NEWER
using Unity.CompilationPipeline.Common.ILPostProcessing;
using UnityEditor;

namespace Kekser.PowerSingleton.Editor.Weaving
{
    public class WeaverILEntry : ILPostProcessor
    {
        [MenuItem("Kekser/PowerSingleton/Weave")]
        public static void Weave()
        {
            
        }
        
        public override ILPostProcessor GetInstance() => this;

        public override bool WillProcess(ICompiledAssembly compiledAssembly)
        {
            return false;
        }

        public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            return new ILPostProcessResult(null, null);
        }
    }
}
#endif