using Human;
using UnityEngine;

namespace Boosters
{
    public class Item : BaseScoreBooster
    {
        protected void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponentInParent<HumanItemSetter>()) return;
            base.OnTriggerEnter(other);
            Destroy(gameObject);
        }
    }
}