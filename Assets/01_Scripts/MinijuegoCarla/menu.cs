using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Juego");
    }
}