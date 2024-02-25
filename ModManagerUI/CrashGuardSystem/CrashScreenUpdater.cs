using Timberborn.BaseComponentSystem;

namespace ModManagerUI.CrashGuardSystem
{
    public class CrashScreenUpdater : BaseComponent
    {
        public void Update()
        {
            CrashScreenBox.UpdateSingleton();
        }
    }
}