using System.Collections.Generic;

namespace Rg.Web.ViewModels
{
    /// <summary>
    /// Base class of view models for views that host the text
    /// editor.
    /// </summary>
    public class ViewModelWithTextEditingBase : ViewModelBase
    {
        public IDictionary<string, User> AllUsers { get; set; }

        public class User
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string AvatarUrl { get; set; }
        }
    }
}
