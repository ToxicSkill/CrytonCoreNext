﻿using System;
using System.Windows.Input;

namespace CrytonCoreNext.Commands
{
    public class Command : ICommand
    {
        private Action _action;
        private bool _canExecute;

        public Command(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
