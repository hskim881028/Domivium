using UnitGenerator;

namespace Domivium.Client.Core.UI
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.Comparable | UnitGenerateOptions.ArithmeticOperator)]
    public readonly partial struct UIPriority
    {
        public static UIPriority Default = 0;

        #region System

        public static UIPriority Popup = 1;
        public static UIPriority NetworkWaiting = 99;

        #endregion

        #region Static

        public static UIPriority UserInformation = 1;
        public static UIPriority Login = 10;
        public static UIPriority QuickSlot = 100;
        public static UIPriority Minimap = 101;

        #endregion
    }
}