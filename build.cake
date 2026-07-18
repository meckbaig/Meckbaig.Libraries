///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = Argument("packageVersion", "1.0.0");

var root = MakeAbsolute(Directory("."));
var artifactsDir = root.Combine("artifacts");
var nugetDir = artifactsDir.Combine("nuget");

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
.Does(() =>
{
    EnsureDirectoryExists(artifactsDir);
    EnsureDirectoryExists(nugetDir);

    CleanDirectory(nugetDir);

    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");

    Information("Artifacts: {0}", nugetDir);
});

Task("Pack")
.IsDependentOn("Clean")
.Does(() =>
{
    var projects = GetFiles("./**/*.csproj")
        .Where(x =>
            !x.FullPath.Contains("\\Test") &&
            !x.FullPath.Contains("\\Tests") &&
            !x.FullPath.Contains("/Test") &&
            !x.FullPath.Contains("/Tests"));

    foreach (var project in projects)
    {
        Information("Packing {0}", project.GetFilename());

        DotNetPack(project.FullPath, new DotNetPackSettings
        {
            Configuration = configuration,
            OutputDirectory = nugetDir,
            NoBuild = false,
            NoRestore = false,
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("Version", version)
        });
    }
});

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);