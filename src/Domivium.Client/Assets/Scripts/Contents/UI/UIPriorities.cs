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

        public static UIPriority Login = 100;
        public static UIPriority Bootstrap = 200;
        public static UIPriority Lobby = 300;
        public static UIPriority Stage = 400;

        #endregion
    }
}