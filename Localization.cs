using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Reflection;
using System.Globalization;



namespace Words
{
    class Localization
    {
        private static CultureInfo сhoiseLanguage;
        private static ResourceManager m_resMgr = new ResourceManager("Words.Properties.Resources", Assembly.GetExecutingAssembly());
        public static void SetLanguage(string language)
        {
            сhoiseLanguage = new CultureInfo(language);
        }
        public static ResourceManager AppResourcesManager
        {
            get { return m_resMgr; }
        }
        public static String GetLocalizedString(String sResourceName)
        {
            return AppResourcesManager.GetString(sResourceName, сhoiseLanguage);
        }
    }
}
