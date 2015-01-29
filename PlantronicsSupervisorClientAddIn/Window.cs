using ININ.IceLib.Connection;
using ININ.IceLib.Connection.Extensions;
using ININ.InteractionClient.AddIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlantronicsSupervisorClientAddIn
{
    public class Window :AddInWindow
    {

        public Window()
        {

        }
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
            get
            {
                if (AddIn.Session != null)
                {
                    var serverParams = new ServerParameters(AddIn.Session);
                    var serverParamList = serverParams.GetServerParameters(new[] { "PlantronicsStatusWebServerUrl" });

                    if (serverParamList.Count > 0)
                    {
                        var value = serverParamList[0].Value;
                        if (String.IsNullOrEmpty(value))
                        {
                            return null;
                        }

                        WebBrowser web = new WebBrowser();
                        web.Navigate(value);
                        return web;
                    }
                }

                return null;

            }
        }

        protected override string DisplayName
        {
            get { return Resources.DisplayName; }
        }

        protected override string Id
        {
            get { return "Plantronics Supervisors AddIn"; }
        }
    }
}
