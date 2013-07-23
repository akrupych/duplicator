using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Duplicator
{
    class DuplicatesFinder : BackgroundWorker
    {
        /// <summary>
        /// Listens to the worker events
        /// </summary>
        private IWorkerObserver Observer { get; set; }
        /// <summary>
        /// Contains possible duplicates during size checking.
        /// long key is for size, list value contains file pathes with that size
        /// </summary>
        private Dictionary<long, List<string>> PossibleDuplicates { get; set; }

        /// <summary>
        /// Initial event handlers setup
        /// </summary>
        /// <param name="observer">Event listener</param>
        public DuplicatesFinder(IWorkerObserver observer)
        {
            Observer = observer;
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            DoWork += new DoWorkEventHandler(OnDoWork);
            ProgressChanged += new ProgressChangedEventHandler(OnProgressChanged);
            RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnRunWorkerCompleted);
            PossibleDuplicates = new Dictionary<long, List<string>>();
        }

        /// <summary>
        /// Main execute method.
        /// 1. Orders possible duplicates by file size.
        /// 2. Checks similarity with checksums.
        /// </summary>
        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            // clear previous results
            PossibleDuplicates.Clear();
            // first, check received path
            string root = (string)e.Argument;
            if (root == null)
            {
                Observer.OnWorkerThrownException(new ArgumentNullException());
                return;
            }
            // group all files by size (changes PossibleDuplicates)
            AnalizeFileSizes(new DirectoryInfo(root));
            // remove files unique by size (changes PossibleDuplicates)
            CleanUpPossibleDuplicates();
            // signal that preparations are finished and the heavy part is about to begin \m/_
            ReportProgress(0);
        }

        /// <summary>
        /// Redirects progress update to listener
        /// </summary>
        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Observer.OnWorkerProgressUpdate(0);
        }

        /// <summary>
        /// Redirects complete call to listener
        /// </summary>
        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Observer.OnWorkerComplete();
        }

        /// <summary>
        /// Performs recursive traversing of the folder, collecting file sizes.
        /// File pathes are grouped by size and put into PossibleDuplicates.
        /// </summary>
        /// <param name="root">Folder to traverse</param>
        private void AnalizeFileSizes(DirectoryInfo root)
        {
            // we can also use Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
            // to get all file pathes and then find file sizes
            foreach (FileInfo file in root.GetFiles())
            {
                try
                {
                    if (PossibleDuplicates.ContainsKey(file.Length))
                        PossibleDuplicates[file.Length].Add(file.FullName);
                    else PossibleDuplicates.Add(file.Length, new List<string> { file.FullName });
                }
                catch { }
            }
            // we can also use Parallel.For(0, root.GetDirectories().Length, i => { AnalizeFileSizes(root.GetDirectories()[i]); });
            // for adding some performance; though it must be checked
            foreach (DirectoryInfo directory in root.GetDirectories())
                AnalizeFileSizes(directory);
        }

        /// <summary>
        /// Removes records that have only one file from PossibleDuplicates.
        /// Remaining files become really possible duplicates.
        /// </summary>
        private void CleanUpPossibleDuplicates()
        {
            long[] keys = new long[PossibleDuplicates.Count];
            PossibleDuplicates.Keys.CopyTo(keys, 0);
            foreach (long key in keys)
                if (PossibleDuplicates[key].Count == 1)
                    PossibleDuplicates.Remove(key);
        }
    }
}
