using System;
using System.Windows;
using System.Windows.Input;
using ReduxPlayground.BLO;
using ReduxPlayground.BLO.Redux.Actions;
using ReduxPlayground.BLO.Redux.Store;

namespace ReduxPlayground.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            RegisterCommands();
        }

        #endregion

        #region Methods

        private void RegisterCommands()
        {
            CommandBindings.Add(new CommandBinding(Commands.LongRunningCommand, ExecutedLongRunningCommand));
        }

        private void ExecutedLongRunningCommand(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            AppStore.Dispatch(new LongRunning());
        }

        #endregion
    }
}
