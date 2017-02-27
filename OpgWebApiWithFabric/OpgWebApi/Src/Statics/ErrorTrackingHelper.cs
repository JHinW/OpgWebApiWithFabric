using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpgWebApi.Src.Statics
{
    public class ErrorTrackingHelper
    {
        public static Action<Exception, string> TrackException { get; set; }
            = (exception, message) => { };

        // callbacks for blocking UI error message
        public static Func<Exception, string, Task> GenericApiCallExceptionHandler { get; set; }
            = (ex, errorTitle) => Task.FromResult(0);
    }
}
