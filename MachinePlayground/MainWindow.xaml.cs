using System;
using System.Windows;
using MachinePlayground.Actions;
using Machine;
using MachinePlayground.States;

namespace MachinePlayground
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Machine<string>.StateChanged += OnStringMachineStateChanged;
            Machine<string>.InitializeState("s");
            Machine<string>.RegisterAction(typeof(Add), (action, s) => s + s);
            Machine<string>.Dispatch(new Add());
            Machine<string>.Dispatch(new Add());

            Machine<ImmutableState>.StateChanged += OnImmutableStateMachineStateChanged;
            Machine<ImmutableState>.InitializeState(new ImmutableState(1, "lol", new ImmutableState(42, "lulz")));
            Machine<ImmutableState>.RegisterAction(typeof(Add), state => state.Nested.Count, (action, i) => i + i);
        }

        private void OnImmutableStateMachineStateChanged(ImmutableState immutableState)
        {
            Console.WriteLine(immutableState);
        }

        private void OnStringMachineStateChanged(string s)
        {
            Console.WriteLine($@"StringMachine: {s}");
        }
    }
}
