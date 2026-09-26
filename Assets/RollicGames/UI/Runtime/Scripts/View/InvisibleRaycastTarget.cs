using UnityEngine;
using UnityEngine.UI;

namespace RollicGames.UI.Runtime.View
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class InvisibleRaycastTarget : Graphic
    {
        public override void SetMaterialDirty() {}
        public override void SetVerticesDirty() {}

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}