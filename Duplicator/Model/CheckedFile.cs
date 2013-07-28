using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duplicator
{
    public class CheckedFile
    {
        public string Path { get; set; }

        public byte[] Hash { get; set; }

        public CheckedFile(string path)
        {
            Path = path;
        }
    }
}
