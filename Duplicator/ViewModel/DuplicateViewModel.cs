using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Duplicator.ViewModel;

namespace Duplicator
{
    public class DuplicateViewModel: ViewModelBase
    {
        private string _duplicateName;
        private ObservableCollection<Duplicate> _duplicatesCollection;

        public string DuplicateName
        {
            get { return _duplicateName; }
            set
            {
                _duplicateName = value;
                RaisePropertyChanged("DuplicateName");

            }
        }

        public ObservableCollection<Duplicate> Duplicates
        {
            get { return _duplicatesCollection; }
            set
            {
                if (_duplicatesCollection != value)
                {
                    _duplicatesCollection = value;
                    RaisePropertyChanged("Duplicates");
                }
            }
        }       
    }
}
