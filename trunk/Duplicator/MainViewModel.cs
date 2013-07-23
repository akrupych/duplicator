using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;

namespace Duplicator
{
    public class MainViewModel: ViewModelBase
    {
        string path;
        ICommand _browseCommand;
        ICommand _startCommand;
        ICommand _cancelCommand;

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                RaisePropertyChanged("Path");
            }
        }
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
            var dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = false;
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
                Path = dlg.SelectedPath;
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
