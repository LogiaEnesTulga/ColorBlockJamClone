using RollicGames.Math.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.Model;
using RollicGames.ColorBlockJamClone.Level.Runtime.View;
using UnityEngine;

public class LevelDoorView : MonoBehaviour
{
    [SerializeField] private MeshRenderer _doorRenderer;
    [SerializeField] private MeshRenderer _arrowRenderer;

    public void InitializeView(int2 gridPosition, LevelDirection direction, int length, Color color, Color arrowColor)
    {
        LevelViewHelper.ApplyColorProperty(_doorRenderer, color);
        LevelViewHelper.ApplyColorProperty(_arrowRenderer, arrowColor);

        _doorRenderer.transform.localScale = new Vector3(length, 0.5f, 1f);
        switch(direction)
        {
            case LevelDirection.Up:
                transform.localPosition = new Vector3(gridPosition.X * 2f + length -1f, -gridPosition.Y * 2f + 1.4f, 0f);
                transform.localRotation = Quaternion.Euler(0f, 0f , 180f);
                break;

            case LevelDirection.Down:
                transform.localPosition = new Vector3(gridPosition.X * 2f + length -1f, -gridPosition.Y * 2f - 1.4f, 0f);
                transform.localRotation = Quaternion.Euler(0f, 0f , 0f);
                break;
            
            case LevelDirection.Left:
                transform.localPosition = new Vector3(gridPosition.X * 2f - 1.4f, -gridPosition.Y * 2f - length + 1, 0f);
                transform.localRotation = Quaternion.Euler(0f, 0f , 270f);
                break;
            
            case LevelDirection.Right:
                transform.localPosition = new Vector3(gridPosition.X * 2f + 1.4f, -gridPosition.Y * 2f - length + 1, 0f);
                transform.localRotation = Quaternion.Euler(0f, 0f , 90f);
                break;
        }
    }
}
