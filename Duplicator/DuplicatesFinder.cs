using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Duplicator
{
    /// <summary>
    /// Background worker, responsible for duplicates locating.
    /// Call RunWorkerAsync(Path) to start the show!
    /// </summary>
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
        private Dictionary<long, List<CheckedFile>> PossibleDuplicates { get; set; }

        private IEnumerable<IEnumerable<CheckedFile>> Duplicates { get; set; }

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
            PossibleDuplicates = new Dictionary<long, List<CheckedFile>>();
        }

        /// <summary>
        /// Main execute method.
        /// 1. Orders possible duplicates by file size.
        /// 2. Checks similarity with checksums.
        /// </summary>
        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            Statistics.Reset();
            Statistics.StartTimer();
            // clear previous results
            PossibleDuplicates.Clear();
            // first, check received path
            string root = (string)e.Argument;
            if (root == null)
            {
                Observer.OnWorkerThrownException(new ArgumentNullException());
                return;
            }
            if (CancellationPending) return;
            // group all files by size (changes PossibleDuplicates)
            AnalizeFileSizes(new DirectoryInfo(root));
            if (CancellationPending) return;
            // remove files unique by size (changes PossibleDuplicates)
            CleanUpPossibleDuplicates();
            if (CancellationPending) return;
            // signal that preparations are finished and the heavy part is about to begin \m/_
            ReportProgress(0);
            if (CancellationPending) return;
            Statistics.Checkout();
            CalculateChecksums();
            Statistics.StopTimer();
            if (CancellationPending) return;
        }

        /// <summary>
        /// Redirects progress update to listener
        /// </summary>
        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Observer.OnWorkerProgressUpdate(e.ProgressPercentage);
        }

        /// <summary>
        /// Redirects complete call to listener
        /// </summary>
        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Observer.OnWorkerComplete(Duplicates);
        }

        /// <summary>
        /// Performs recursive traversing of the folder, collecting file sizes.
        /// File pathes are grouped by size and put into PossibleDuplicates.
        /// </summary>
        /// <param name="root">Folder to traverse</param>
        private void AnalizeFileSizes(DirectoryInfo root)
        {
            if (CancellationPending) return;
            try
            {
                // we can also use Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                // to get all file pathes and then find file sizes
                foreach (FileInfo file in root.GetFiles())
                {
                    Statistics.Increment("Files");
                    try
                    {
                        if (PossibleDuplicates.ContainsKey(file.Length))
                            PossibleDuplicates[file.Length].Add(new CheckedFile(file.FullName));
                        else PossibleDuplicates.Add(file.Length, new List<CheckedFile> { new CheckedFile(file.FullName) });
                    }
                    catch { }
                }
                // parallel execution notably increases performance
                Parallel.ForEach<DirectoryInfo>(root.GetDirectories(), directory => AnalizeFileSizes(directory));
            }
            catch { }
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

        private void CalculateChecksums()
        {
            long totalSize = CalculateTotalSize();
            long analyzedSize = 0;
            int percents = 0;
            List < IEnumerable < IEnumerable < CheckedFile >>> list = new List<IEnumerable<IEnumerable<CheckedFile>>>();
            foreach (KeyValuePair<long, List<CheckedFile>> item in PossibleDuplicates)
            {
                List<CheckedFile> filesWithTheSameSize = item.Value;
                foreach (CheckedFile file in filesWithTheSameSize)
                {
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(file.Path))
                        {
                            file.Hash = md5.ComputeHash(stream);
                        }
                    }
                    analyzedSize += item.Key;
                    percents = Convert.ToInt32(analyzedSize * 100 / totalSize);
                    ReportProgress(percents);
                    if (CancellationPending) return;
                }
                var duplicates = from file in filesWithTheSameSize
                             group file by file.Hash into grouped
                             select grouped;
                list.Add(duplicates);              
            }
            Duplicates = list.SelectMany(x => x).ToList();
        }

        /// <summary>
        /// Calculates size of all files for which md5 will be calculated
        /// </summary>
        /// <returns>total size</returns>
        private long CalculateTotalSize()
        {
            long totalSize = 0;
            foreach (KeyValuePair<long, List<CheckedFile>> item in PossibleDuplicates)
            {
                totalSize += item.Key * item.Value.Count;
            }    
            return totalSize;
        }
    }
}
