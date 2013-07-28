using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duplicator
{
    /// <summary>
    /// Contains general worker callbacks
    /// </summary>
    interface IWorkerObserver
    {
        /// <summary>
        /// Progress has changed
        /// </summary>
        /// <param name="percents">New progress value</param>
        void OnWorkerProgressUpdate(int percents);

        /// <summary>
        /// Exception was thrown
        /// </summary>
        /// <param name="e">Throwed exception</param>
        void OnWorkerThrownException(Exception e);

        /// <summary>
        /// Worker finished its work
        /// </summary>
        void OnWorkerComplete(IEnumerable<IEnumerable<CheckedFile>> Duplicates);
    }
}
