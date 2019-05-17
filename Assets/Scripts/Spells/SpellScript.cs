using UnityEngine;

namespace Spells {
    public class SpellScript : MonoBehaviour {
        private float m_MoveSpeed = 10f;
        private float m_LifeSpan = 2; // seconds
    
        public bool move = false;

        // Update is called once per frame
        private void Update() {

            m_MoveSpeed = move ? 2f : 0;
            
            transform.Translate(transform.forward * m_MoveSpeed * Time.deltaTime);

            if (!move) return;
            
            transform.parent = null;
            m_LifeSpan = m_LifeSpan - Time.deltaTime;

            if (m_LifeSpan <= 0) {
                Destroy(gameObject);
            }

        }
    }
}
