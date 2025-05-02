
using DotNetNuke.Services.Exceptions;
using System;
using UpendoVentures.SkinObjects.OpenContentHelper.Components;

namespace UpendoVentures.SkinObjects.OpenContentHelper
{
    public partial class View : OpenContentHelperModuleBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               // do something
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

    }
}