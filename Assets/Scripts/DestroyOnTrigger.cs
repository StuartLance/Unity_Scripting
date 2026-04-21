using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public string tagFilter;

    private void OnTriggerEnter(Collider other) // 1
    {
        if (other.CompareTag(tagFilter)) // 2
        {
            Destroy(gameObject); // 3
        }
    }

}
