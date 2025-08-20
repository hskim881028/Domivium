using Domivium.Client.Core.UI;

namespace Domivium.Client.Contents.UI
{
    public static class UIPriorities
    {
        #region System

        public static UIPriority Popup = 1;
        public static UIPriority NetworkWaiting = 99;

        #endregion

        #region Static

        public static UIPriority Title = 100;
        public static UIPriority Lobby = 200;
        public static UIPriority Stage = 300;

        #endregion
    }
}