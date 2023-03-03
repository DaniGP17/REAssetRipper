using REAssetRipper.Core.Constants;
using REAssetRipper.Core.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAssetRipper.Core
{
    public static class ErrorHandler
    {
        public static void NewError(Errors.Types errorType, string[] additionalInformation = null)
        {
            Console.WriteLine("New error finded of type " + errorType.ToString());
            Log.InsertNewLog("ERROR: " + errorType.ToString());

            if (additionalInformation != null)
            {
                foreach(var info in additionalInformation)
                {
                    Console.WriteLine("Additional info: " + info);
                    Log.InsertNewLog("ERROR ADDITIONAL INFO: " + info);
                }
            }
        }
    }
}
