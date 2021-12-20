using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HeltecTextLoggerApp.Mvvm
{
	public class RelayCommand : ICommand
	{
		public RelayCommand(Action<object> exec)
		{
			_exec = exec;
		}

		public RelayCommand(Action<object> exec, Func<object, bool> canExec)
		{
			_exec = exec;
			_canExec = canExec;
		}

		private Action<object> _exec;
		private Func<object, bool> _canExec;

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			if (_canExec != null)
			{
				return _canExec.Invoke(parameter);
			}
			else
			{
				return true;
			}

		}

		public void Execute(object parameter)
		{
			_exec.Invoke(parameter);
		}
	}
}
