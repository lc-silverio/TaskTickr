using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTickr.Services.Settings
{
    public  interface ISettingsService
    {
        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <returns>Returns the settings</returns>
        public APISettings GetSettings();
    }
}
