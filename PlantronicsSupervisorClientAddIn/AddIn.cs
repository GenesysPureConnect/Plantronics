using ININ.IceLib.Connection;
using ININ.InteractionClient.AddIn;
using System;
using System.Diagnostics;

namespace PlantronicsSupervisorClientAddIn
{
    public class AddIn :IAddIn
    {
        private static Session s_session = null;

        public void Load(System.IServiceProvider serviceProvider)
        {
            try
            {
                //must have the icelib sdk license to get the session as a service
                s_session = (Session)serviceProvider.GetService(typeof(Session));

            }
            catch (ArgumentNullException)
            {
                Debug.Fail("unable to get service.  Is the ICELIB SDK licence available?");
                throw;
            }
        }

        public static Session Session
        {
            get
            {
                return s_session;
            }
        }

        public void Unload()
        {
            
        }
    }
}
