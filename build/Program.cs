using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static Bullseye.Targets;
using static SimpleExec.Command;

namespace targets;

internal static class Program
{
  private const string solution = "openiddict-ui.sln";
  private const string packOutput = "./artifacts";
  private const string nugetSource = "https://api.nuget.org/v3/index.json";
  private const string envVarMissing = " environment variable is missing. Aborting.";

  private static readonly IList<string> packableProjects = new List<string>{
    "OpenIddict.UI.Suite.Core",
    "OpenIddict.UI.Suite.Api",
    "OpenIddict.UI.Infrastructure",
    "OpenIddict.UI.Api",
    "OpenIddict.UI.Identity.Core",
    "OpenIddict.UI.Identity.Infrastructure",
    "OpenIddict.UI.Identity.Api"
  };

  private static class Targets
  {
    public const string RestoreTools = "restore-tools";
    public const string AddChangelog = "add-changelog";
    public const string InstallDependencies = "install-dependencies";
    public const string CleanBuildOutput = "clean-build-output";
    public const string CleanPackOutput = "clean-pack-output";
    public const string Build = "build";
    public const string Test = "test";
    public const string UpdateChangelog = "update-changelog";
    public const string Release = "release";
    public const string Pack = "pack";
    public const string Deploy = "deploy";
    public const string DeployClient = "deploy-client";
  }

  public static async Task Main(string[] args)
  {
    // TODO: encapsulate with sth. like McMaster.Extensions.CommandLineUtils
    var version = string.Empty;
    var key = string.Empty;

    if (args[0].Contains("--"))
    {
      var firstArg = args[0].Split("--")[0].Trim();
      var customArgs = args[0].Split("--")[1].Trim().Split("&");

      if (customArgs.Any(x =>
      {
        return x.Contains("version");
      }))
      {
        var versionArgs = customArgs.First(x =>
        {
          return x.Contains("version");
        });
        version = versionArgs.Split('=')[1].Trim();
      }

      if (customArgs.Any(x =>
      {
        return x.Contains("key");
      }))
      {
        var keyArgs = customArgs.First(x =>
        {
          return x.Contains("key");
        });
        key = keyArgs.Split('=')[1].Trim();
      }

      args[0] = firstArg;
    }

    if (!string.IsNullOrWhiteSpace(version))
    {
      Console.WriteLine($"Using version: '{version}'" + Environment.NewLine);
    }

    #region Local build targets
    Target(Targets.RestoreTools, () =>
    {
      Run("dotnet", "tool restore");
    });

    Target(Targets.AddChangelog, () =>
    {
      Run("dotnet", "tool run releasy add-changelog", "changelogs");
    });

    Target(Targets.CleanBuildOutput, () =>
    {
      Run("dotnet", $"clean {solution} -c Release -v m --nologo");
    });

    Target(Targets.Build, DependsOn(Targets.CleanBuildOutput), () =>
    {
      Run("dotnet", $"build {solution} -c Release --nologo");
    });

    Target(Targets.Test, DependsOn(Targets.Build), () =>
    {
      Run("dotnet", $"test {solution} -c Release --no-build --nologo");
    });

    Target(Targets.UpdateChangelog, () =>
    {
      if (string.IsNullOrWhiteSpace(version))
      {
        throw new Bullseye.TargetFailedException("Version for updating changelog is missing!");
      }

      // updating the changelog
      Run("dotnet", $"tool run releasy update-changelog -v {version} -p https://github.com/thomasduft/openiddict-ui/issues/");

      // committing the changelog changes
      Run("git", $"commit -am \"Committing changelog changes for v'{version}'\"");
    });

    Target(Targets.Release, DependsOn(Targets.RestoreTools, Targets.Test, Targets.UpdateChangelog), () =>
    {
      if (string.IsNullOrWhiteSpace(version))
      {
        throw new Bullseye.TargetFailedException("Version for updating changelog is missing!");
      }

      // applying the tag
      Run("git", $"tag -a v{version} -m \"version '{version}'\"");

      // pushing
      Run("git", $"push origin v{version}");
    });
    #endregion

    #region Deployment targets
    Target(Targets.CleanPackOutput, () =>
    {
      if (Directory.Exists(packOutput))
      {
        Directory.Delete(packOutput, true);
      }
    });

    Target(Targets.Pack, DependsOn(Targets.Build, Targets.CleanPackOutput), () =>
    {
      if (string.IsNullOrWhiteSpace(version))
      {
        throw new Bullseye.TargetFailedException("Version for packaging is missing!");
      }

      // pack packages
      var directory = Directory.CreateDirectory(packOutput).FullName;
      var projects = GetFiles("src", $"*.csproj");
      foreach (var project in projects)
      {
        if (project.Contains(".Tests"))
        {
          continue;
        }

        if (packableProjects.Any(m =>
        {
          return project.Contains(m);
        }))
        {
          Run("dotnet", $"pack {project} -c Release -p:PackageVersion={version} -p:Version={version} -o {directory} --no-build --nologo");
        }
      }
    });

    Target(Targets.Deploy, DependsOn(Targets.RestoreTools, Targets.Test, Targets.Pack), () =>
    {
      if (string.IsNullOrWhiteSpace(key))
      {
        throw new Bullseye.TargetFailedException("Key for deploying is missing!");
      }

      // push packages
      var directory = Directory.CreateDirectory(packOutput).FullName;
      var packages = GetFiles(directory, $"*.nupkg");
      foreach (var package in packages)
      {
        Run("dotnet", $"nuget push {package} -s {nugetSource} -k {key}");
      }
    });
    #endregion

    #region Make life easier targets
    Target(Targets.InstallDependencies, () =>
    {
      Run("dotnet", "tool restore");
      Run("npm", "install", "samples/Client");
    });

    Target(Targets.DeployClient, () =>
    {
      Run("npm", "install", "samples/Client");
      Run("npm", "run publish", "samples/Client");

      // delete all files, not directories in samples/Server/wwwroot
      var files = Directory.GetFiles("samples/Server/wwwroot");
      foreach (var file in files)
      {
        File.Delete(file);
      }

      // copy files of samples/Client/dist to samples/Server/wwwroot
      CopyDirectory("samples/Client/dist", "samples/Server/wwwroot/");
    });
    #endregion

    await RunTargetsAndExitAsync(
      args,
      ex =>
      {
        return ex is SimpleExec.ExitCodeException
            || ex.Message.EndsWith(envVarMissing, StringComparison.InvariantCultureIgnoreCase);
      });
  }

  private static List<string> GetFiles(
      string directoryToScan,
      string filter
    )
  {
    var files = new List<string>();

    files.AddRange(Directory.GetFiles(
      directoryToScan,
      filter,
      SearchOption.AllDirectories
    ));

    return files;
  }

  private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = false)
  {
    var dir = new DirectoryInfo(sourceDir);
    if (!dir.Exists)
    {
      throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
    }

    foreach (var file in dir.GetFiles())
    {
      Directory.CreateDirectory(destinationDir);
      var targetFilePath = Path.Combine(destinationDir, file.Name);
      file.CopyTo(targetFilePath, true);
    }

    if (recursive)
    {
      foreach (var subDir in dir.GetDirectories())
      {
        var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
        CopyDirectory(subDir.FullName, newDestinationDir, true);
      }
    }
  }
}
