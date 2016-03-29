using System.Collections.Generic;

namespace Rg.Web.ViewModels
{
    public class InvitationsViewModel : ViewModelBase
    {
        public IList<string> OutstandingInvitations { get; set; }
        public IList<string> AcceptedInvitations { get; set; }
    }
}
