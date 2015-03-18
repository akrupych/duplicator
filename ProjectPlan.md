## Check knowledge’s in areas: ##
  * .NET base class library
  * WPF + MVVM design pattern.
  * Application logic decomposition
  * Defensive development and code robustness.
  * Data structures and algorithms.
  * Multithreading.

## Test application 1: ##

**Application destination:**
tool to find file duplications (files with same data).

Application has following screens:
  1. Start info screen with folder selection dialog.
  1. Search progress which may be canceled.
  1. Results screen.

On first screen user selects some target folder on disc. Target folder may contain any files with any size. Target folder may contain subfolders and recursive search is required. Application collects all nested file names and shows all files which are duplicated on disc in target folder.

  * Application should correctly handle situations when  files are busy or not accessible and ignore them.
  * Files may have any size. That’s why application should cache all files content. Memory usage should not be too high (not more than 500MB).
  * Application should show progress bar with progress percent if it is possible.
  * Application should be highly responsive and provide progress cancelation. Application should not be frizzed in any case.
  * Application may use multi CPU cores for performance optimization if it is possible.