using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float Health;
    public Scrollbar HealthScroll;

    public void TakeDamage(int damage) {
        Health -= damage;

        HealthScroll.size = Health/100;

        if (Health <= 0){

            Debug.Log("Quit Game");

            // If running in the editor
            #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
            #else
                    // If running in a build
                    Application.Quit();
            #endif
        }

    }
}
