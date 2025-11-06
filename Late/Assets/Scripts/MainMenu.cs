using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
  public void OnPlayButton()
  {
    SceneManager.LoadScene("Prologue");
  }

   public void OnPlayButton2()
  {
    SceneManager.LoadScene("Hospital");
  }


}
