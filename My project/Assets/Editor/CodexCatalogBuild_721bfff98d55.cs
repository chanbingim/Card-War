using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class CodexCatalogBuild_721bfff98d55
{
    public static void Build()
    {
        var output = Environment.GetEnvironmentVariable("CATALOG_TOOL_BUILD_OUTPUT");
        var targetName = Environment.GetEnvironmentVariable("CATALOG_TOOL_BUILD_TARGET");
        if (String.IsNullOrEmpty(output) || !Enum.TryParse(targetName, out BuildTarget target))
        {
            UnityEngine.Debug.LogError("Catalog tool build arguments are invalid.");
            EditorApplication.Exit(2);
            return;
        }
        var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
        if (scenes.Length == 0)
        {
            UnityEngine.Debug.LogError("No enabled scenes are configured in Build Settings.");
            EditorApplication.Exit(2);
            return;
        }
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = output,
            target = target,
            options = BuildOptions.None
        });
        var code = report.summary.result == BuildResult.Succeeded ? 0 : 1;
        UnityEngine.Debug.Log("Catalog tool build result: " + report.summary.result + ", output=" + output);
        EditorApplication.Exit(code);
    }
}