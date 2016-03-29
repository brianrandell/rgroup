using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rg.ApiTypes
{
    public class CreateTimelineEntry
    {
        public string Message { get; set; }
        public IList<int> MediaIds { get; set; }
    }
}
