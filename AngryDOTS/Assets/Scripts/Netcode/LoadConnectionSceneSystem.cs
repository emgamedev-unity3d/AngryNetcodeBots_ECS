#if UNITY_EDITOR

using Unity.Entities;
using UnityEngine.SceneManagement;

namespace Helpers
{
    /*
        The sole purpose of this class is to run on the editor,  so that it saves you time by first
        bringing you to the connection scene and then having you create a proper connection. This 
        way you don't have to close the moba scene, then open the connection scene so that you 
        can play test.

        The script takes you to the connection scene automatically from the moba scene
     */

    public partial class LoadConnectionSceneSystem : SystemBase
    {
        protected override void OnCreate()
        {
            Enabled = false;

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
                return;

            SceneManager.LoadScene(0);
        }

        protected override void OnUpdate()
        {
        }
    }
}

#endif