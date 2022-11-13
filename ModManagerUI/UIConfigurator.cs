using Bindito.Core;
using ModManager.ModIoSystem;
using TimberApi.ConfiguratorSystem;
using Timberborn.ModsSystemUI;

namespace ModManager
{
    [Configurator(TimberApi.SceneSystem.SceneEntrypoint.MainMenu)]
    public class UIConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<ModsBox>().AsSingleton();
            containerDefinition.Bind<IModService>().To<ModService>().AsSingleton();
            containerDefinition.Bind<ExtractorService>().AsSingleton();
        }
    }
}
