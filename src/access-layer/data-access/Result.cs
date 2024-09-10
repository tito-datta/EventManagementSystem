using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Result
    {
        public string Error { get; set; }
        public object Content { get; set; }

        internal bool IsError => !string.IsNullOrEmpty(Error);
    }
}
