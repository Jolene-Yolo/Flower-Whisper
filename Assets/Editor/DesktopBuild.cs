using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class DesktopBuild
{
    private const string MainScenePath = "Assets/Scenes/MainScene.unity";
    private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";
    private const string OutputFolder = "Builds/Windows";
    private const string ExeName = "FlowerWhisper.exe";

    [MenuItem("Build/Build Standalone Desktop (Windows)")]
    public static void BuildWindowsDesktop()
    {
        EnsureBuildScenes();

        string outputDir = Path.GetFullPath(OutputFolder);
        Directory.CreateDirectory(outputDir);

        string outputPath = Path.Combine(outputDir, ExeName);
        string[] buildScenes = GetBuildScenePaths();

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = buildScenes,
            target = BuildTarget.StandaloneWindows64,
            locationPathName = outputPath,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log($"[FlowerWhisper] Build succeeded: {outputPath} ({summary.totalSize / (1024f * 1024f):F2} MB)");
            return;
        }

        throw new BuildFailedException($"Build failed with result: {summary.result}");
    }

    // CLI entry point:
    // Unity -batchmode -quit -projectPath <path> -executeMethod DesktopBuild.BuildWindowsDesktopCI
    public static void BuildWindowsDesktopCI()
    {
        BuildWindowsDesktop();
    }

    private static void EnsureBuildScenes()
    {
        if (!File.Exists(MainScenePath))
            throw new BuildFailedException($"Main scene not found: {MainScenePath}");

        if (!File.Exists(SampleScenePath))
            throw new BuildFailedException($"Sample scene not found: {SampleScenePath}");

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(MainScenePath, true),
            new EditorBuildSettingsScene(SampleScenePath, true)
        };
    }

    private static string[] GetBuildScenePaths()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        string[] enabledScenes = new string[scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            enabledScenes[i] = scenes[i].path;
        }

        return enabledScenes;
    }
}
