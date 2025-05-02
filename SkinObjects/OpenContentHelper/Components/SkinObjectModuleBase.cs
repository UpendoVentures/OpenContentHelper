using DotNetNuke.UI.Skins; 

namespace UpendoVentures.SkinObjects.OpenContentHelper.Components
{
    public class OpenContentHelperModuleBase : SkinObjectBase 
	{
        public string ControlPath 
		{
            get 
			{
                return string.Concat(TemplateSourceDirectory, "/"); 
            }
        }
    }
}