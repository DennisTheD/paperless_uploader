using Foundation;
using LocalAuthentication;
using PaperlessClient.Mobile.Resources;
using PaperlessClient.Mobile.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PaperlessClient.Mobile.iOS.Services.IosAppAuthService))]
namespace PaperlessClient.Mobile.iOS.Services
{
    public class IosAppAuthService : IAppAuthService
    {
        private TaskCompletionSource<bool> authTcs;
        public Task<bool> AuthenticateUser()
        {
            var context = new LAContext();
            NSError AuthError;
            var localizedReason = new NSString(TextResources.AuthReasonText);

            // Because LocalAuthentication APIs have been extended over time,
            // you must check iOS version before setting some properties
            //context.LocalizedFallbackTitle = "Fallback";

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                context.LocalizedCancelTitle = TextResources.CancelText;
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                context.LocalizedReason = TextResources.AuthReasonText;
            }

            authTcs = new TaskCompletionSource<bool>();
            // Check if biometric authentication is possible
            if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out AuthError))
            {
                var replyHandler = new LAContextReplyHandler((success, error) =>
                {
                    authTcs.TrySetResult(success);

                });
                context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, localizedReason, replyHandler);
                return authTcs.Task;
            }

            // Fall back to PIN authentication
            else if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out AuthError))
            {
                var replyHandler = new LAContextReplyHandler((success, error) =>
                {
                    authTcs.TrySetResult(success);

                });
                context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, localizedReason, replyHandler);
                return authTcs.Task;
            }

            throw new InvalidOperationException("Auth not supportd");
        }

        public bool SupportsBiometricAuthentication()
        {
            var context = new LAContext();
            return
                context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var authError1)
                || context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out var authError2);
        }
    }
}