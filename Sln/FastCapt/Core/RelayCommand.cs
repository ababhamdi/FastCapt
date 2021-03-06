﻿using System;
using System.Windows.Input;

namespace FastCapt.Core
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Initializes instance members of the <see cref="RelayCommand"/> class.
        /// </summary>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Initializes instance members of the <see cref="RelayCommand"/> class.
        /// </summary>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {

        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
