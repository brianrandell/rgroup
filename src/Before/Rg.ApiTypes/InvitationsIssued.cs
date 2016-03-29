using System.Collections.Generic;

namespace Rg.ApiTypes
{
    public class InvitationsIssued
    {
        public IList<string> Outstanding { get; set; }
        public IList<string> Accepted { get; set; }
    }
}
