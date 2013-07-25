using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace Duplicator
{
    public class MainViewModel: ViewModelBase, IWorkerObserver
    {
        #region Fields

        private ObservableCollection<string> collection;
        private string path;
        private int percents;
        private bool isCancelEnabled = false;
        private bool isProgressIndeterminate = false;
        private bool isProgressBarEnabled = false;

        private ICommand _browseCommand;
        private ICommand _startCommand;
        private ICommand _cancelCommand;

        private DuplicatesFinder Worker { get; set; }
        
        #endregion

        #region Properties

        public ObservableCollection<string> Collection
        {
            get 
            {
                return collection;
            }
            set
            {
                collection = value;
                RaisePropertyChanged("Collection");
            }
        }

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

        public int Percents
        {
            get
            {
                return percents;
            }
            set
            {
                percents = value;
                RaisePropertyChanged("Percents");
            }
        }

        public bool IsCancelEnabled
        {
            get
            {
                return isCancelEnabled;
            }
            set
            {
                isCancelEnabled = value;
                RaisePropertyChanged("IsCancelEnabled");
            }
        }

        public bool IsProgressIndeterminate
        {
            get
            {
                return isProgressIndeterminate;
            }
            set
            {
                isProgressIndeterminate = value;
                RaisePropertyChanged("IsProgressIndeterminate");
            }
        }

        public bool IsProgressBarEnabled
        {
            get
            {
                return isProgressBarEnabled;
            }
            set
            {
                isProgressBarEnabled = value;
                RaisePropertyChanged("IsProgressBarEnabled");
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

        #endregion

        #region Methods

        public MainViewModel()
        {
            Worker = new DuplicatesFinder(this);
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
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            IsCancelEnabled = true;
            IsProgressIndeterminate = true;
            if (!Worker.IsBusy)
                Worker.RunWorkerAsync(Path);
            else
                MessageBox.Show("Can't run the worker twice!");
        }

        /// <summary>
        /// Defines the method to call when the CancelCommand is invoked
        /// </summary>
        public void CancelExecute()
        {
            Worker.CancelAsync();
            IsCancelEnabled = false;
            IsProgressIndeterminate = false;
            Percents = 0;
        }

        public void OnWorkerProgressUpdate(int percents)
        {
            if (percents == 0)
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        public void OnWorkerComplete(IEnumerable<IEnumerable<CheckedFile>> Duplicates)
        {
            IsCancelEnabled = false;
            IsProgressIndeterminate = false;
            MessageBox.Show("Completed!");
        }

        public void OnWorkerThrownException(Exception e)
        {
        }

        #endregion
    }
}
