using UnityEngine;

namespace BarrierBlaster.Common
{
    public class MobileOnly : MonoBehaviour
    {
        private void Awake()
        {
           gameObject.SetActive(true);
           // Application.platform is RuntimePlatform.Android or RuntimePlatform.IPhonePlayer
        }
    }
}