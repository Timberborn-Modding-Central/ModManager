using System.Threading.Tasks;
using ModManagerUI.EventSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;
using EventBus = ModManagerUI.EventSystem.EventBus;

namespace ModManagerUI.Components.ModManagerPanel
{
    public class ModsScrollView
    {
        private static readonly int RunningShowMoreModsDelayInMs = 200;
        private static readonly int DistanceFromBottomToUpdateInPixels  = 500;
        private static readonly float DistanceFromBottomToUpdateInPercentages  = 0.98f;
        
        public ScrollView Root { get; }

        private bool _showMoreModsDisabled;

        private ModsScrollView(ScrollView root)
        {
            Root = root;
        }
        
        public static ModsScrollView Create(ScrollView root)
        {
            var modsScrollView = new ModsScrollView(root);
            EventBus.Instance.Register(modsScrollView);
            return modsScrollView;
        }
        
        public void Update()
        {
            var nearEndPercentage = Root.scrollOffset.y / Root.verticalScroller.highValue;

            var nearEndAbsolute = Root.verticalScroller.highValue - DistanceFromBottomToUpdateInPixels < Root.scrollOffset.y;
            
            if (_showMoreModsDisabled || (nearEndPercentage < DistanceFromBottomToUpdateInPercentages && !nearEndAbsolute)) 
                return;
            _showMoreModsDisabled = true;
            EventBus.Instance.PostEvent(new ShowMoreModsEvent());
        }
        
        [OnEvent]
        public async Task OnModsRetrieved(ModsRetrievedEvent modsRetrievedEvent)
        {
            await Task.Delay(RunningShowMoreModsDelayInMs);
            if (modsRetrievedEvent.Token.IsCancellationRequested)
            {
                return;
            }
            _showMoreModsDisabled = modsRetrievedEvent.Mods.Count < UiSystem.ModManagerPanel.ModsPerPage;
        }
    }
}