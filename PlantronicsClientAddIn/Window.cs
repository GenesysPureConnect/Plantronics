using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ININ.InteractionClient.AddIn;

namespace PlantronicsClientAddIn
{
    public class Window : AddInWindow
    {

        protected override string CategoryDisplayName
        {
            get { return Resources.CategoryDisplayName; }
        }

        protected override string CategoryId
        {
            get { return "Plantronics Category"; }
        }

        public override object Content
        {
            get {
                return new Views.AddinPage();
            }
        }

        protected override string DisplayName
        {
            get { return Resources.DisplayName; }
        }

        protected override string Id
        {
            get { return "Plantronics AddIn"; }
        }
    }
}
