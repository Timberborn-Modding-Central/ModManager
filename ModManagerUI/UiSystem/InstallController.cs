using System;
using System.IO;
using System.Threading.Tasks;
using Modio.Models;
using ModManager;
using ModManager.AddonSystem;
using ModManager.MapSystem;
using ModManager.ModIoSystem;
using File = Modio.Models.File;

namespace ModManagerUI.UiSystem
{
    public class InstallController : Singleton<InstallController>
    {
        private static readonly IAddonService AddonService = ModManager.AddonSystem.AddonService.Instance;
        
        public static async Task DownloadAndExtract(Mod mod, File? file)
        {
            var modCard = ModCardRegistry.Get(mod);
            modCard?.ModActionStarted();
            try
            {
                file ??= mod.Modfile!;
                var downloadedMod = await ModManager.AddonSystem.AddonService.Instance.Download(mod, file);
                TryInstall(downloadedMod);
            }
            catch (MapException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
            catch (AddonException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
            catch (IOException ex)
            {
                ModManagerUIPlugin.Log.LogError($"{ex.Message}");
            }
            modCard?.ModActionStopped();
        }
        
        public static async Task DownloadAndExtractWithDependencies(Mod mod)
        {
            var file = await AddonService.TryGetCompatibleVersion(mod.Id, ModManagerPanel.CheckForHighestInsteadOfLive);
            await DownloadAndExtract(mod, file);
            await foreach (var dependency in AddonService.GetDependencies(mod))
            {
                var dependencyFile = await AddonService.TryGetCompatibleVersion(dependency.ModId, ModManagerPanel.CheckForHighestInsteadOfLive);
                var dependencyMod = ModIoModRegistry.Get(dependency.ModId);
                await DownloadAndExtract(dependencyMod, dependencyFile);
            }
        }
        
        public static void Uninstall(Mod mod)
        {
            var modCard = ModCardRegistry.Get(mod);
            modCard?.ModActionStarted();
            
            try
            {
                AddonService.Uninstall(mod.Id);
            }
            catch (IOException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }
            catch (AddonException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
            }

            modCard?.ModActionStopped();
        }
        
        private static void TryInstall((string location, Mod Mod) mod)
        {
            try
            {
                if (InstalledAddonRepository.Instance.TryGet(mod.Mod.Id, out var manifest) && manifest.Version != mod.Mod.Modfile.Version)
                {
                    AddonService.ChangeVersion(mod.Mod, mod.Mod.Modfile, mod.location);
                }
                else
                {
                    AddonService.Install(mod.Mod, mod.location);
                }
            }
            catch (MapException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
                ModManagerUIPlugin.Log.LogWarning(ex.StackTrace);
            }
            catch (AddonException ex)
            {
                ModManagerUIPlugin.Log.LogWarning(ex.Message);
                ModManagerUIPlugin.Log.LogWarning(ex.StackTrace);
            }
            catch (Exception ex)
            {
                ModManagerUIPlugin.Log.LogError(ex.StackTrace);
                throw ex;
            }
        }
    }
}