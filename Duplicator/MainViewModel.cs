using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace Duplicator
{
    public class MainViewModel: ViewModelBase
    {
        ICommand _browseCommand;
        ICommand _startCommand;
        ICommand _cancelCommand;

        public ICommand BrowseCommand
        {
            get
            {
                if (_browseCommand == null)
                {
                    _browseCommand = new RelayCommand(BrowseExecute);
                }
                return _browseCommand;
            }
        }

        public ICommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                {
                    _startCommand = new RelayCommand(StartExecute);
                }
                return _startCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(CancelExecute);
                }
                return _cancelCommand;
            }
        }

        /// <summary>
        /// Defines the method to call when the BrowseCommand is invoked
        /// </summary>
        public void BrowseExecute()
        {
            
        }

        /// <summary>
        /// Defines the method to call when the StartCommand is invoked
        /// </summary>
        public void StartExecute()
        {

        }

        /// <summary>
        /// Defines the method to call when the CancelCommand is invoked
        /// </summary>
        public void CancelExecute()
        {

        }
    }
}
