using System.Windows.Input;

namespace ReduxPlayground.BLO
{
    public static class Commands
    {
        public static RoutedUICommand LongRunningCommand = new RoutedUICommand(nameof(LongRunningCommand), nameof(LongRunningCommand), typeof(Commands));
    }
}
