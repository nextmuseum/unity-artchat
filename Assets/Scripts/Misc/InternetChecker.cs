using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ARtChat
{
    public class InternetChecker 
    {
        private const bool allowCarrierDataNetwork = true;
        private const string pingAddress = "8.8.8.8"; // Google Public DNS server
        private const float waitingTime = 2.0f;

        private Ping ping;

        public async UniTask<bool> IsInternetAvailable(CancellationToken cancellationToken)
        {
            /*
            bool internetPossiblyAvailable;
            switch (Application.internetReachability)
            {
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    internetPossiblyAvailable = true;
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    internetPossiblyAvailable = allowCarrierDataNetwork;
                    break;
                default:
                    internetPossiblyAvailable = false;
                    break;
            }
            if (!internetPossiblyAvailable)
            {
                InternetIsNotAvailable();
                return false;
            }
            */
            bool pingSuccess = await CheckPingResult(cancellationToken);
            return pingSuccess;
        }

        private async UniTask<bool> CheckPingResult(CancellationToken token)
        {
            ping = new Ping(pingAddress);

            try
            {
                await UniTask.WaitUntil(() => ping.isDone, cancellationToken: token);
            }
            catch (OperationCanceledException e)
            {
                Debug.Log($"Ping exceede Time {e}");
                return false;
            }


            if (ping.time >= 0)
            {
                Debug.Log("Ping Timeout");
                return true;
            }
            
            return false;
        }

        private void InternetIsNotAvailable()
        {
            Debug.Log("No Internet :(");
        }

        private void InternetAvailable()
        {
            Debug.Log("Internet is available! ;)");
        }
    }
}
