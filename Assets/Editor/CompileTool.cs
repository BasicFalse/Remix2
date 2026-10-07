using UnityEditor;
using UnityEditor.Compilation;

public class CompileTool
{
    [MenuItem("Tools/Recompile Scripts")]
    public static void RequestRecompile()
    {
        CompilationPipeline.RequestScriptCompilation();
    }
}
