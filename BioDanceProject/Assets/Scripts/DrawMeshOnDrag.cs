using System.Collections.Generic;
using UnityEngine;
public class DrawMeshOnDrag : MonoBehaviour
// {
//     public Mesh mesh;
//     public Material material;

//     private bool isDragging = false;

//     void Update()
//     {
//         // マウスボタンが押されたとき
//         if (Input.GetMouseButtonDown(0))
//         {
//             isDragging = true;
//         }

//         // マウスボタンが離されたとき
//         if (Input.GetMouseButtonUp(0))
//         {
//             isDragging = false;
//         }

//         // ドラッグ中にメッシュを描画
//         if (isDragging)
//         {
//             Vector3 mousePos = Input.mousePosition;
//             mousePos.z = 10.0f; // 適切なZ値に設定
//             Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

//             Graphics.DrawMesh(mesh, Matrix4x4.TRS(worldPos, Quaternion.identity, Vector3.one), material, 0);
//         }
//     }
// }
{
    public Mesh instanceMesh;
    public Material instanceMaterial;

    private List<Matrix4x4> matrices = new List<Matrix4x4>();
    private bool isDragging = false;

    void Update()
    {
        // マウスボタンが押されたとき
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }

        // マウスボタンが離されたとき
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // ドラッグ中にインスタンスの位置を更新
        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10.0f; // 適切なZ値に設定
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // メッシュのインスタンスの変換行列を追加
            matrices.Add(Matrix4x4.TRS(worldPos, Quaternion.identity, Vector3.one));
        }

        // インスタンスを描画
        if (matrices.Count > 0)
        {
            Graphics.DrawMeshInstanced(instanceMesh, 0, instanceMaterial, matrices);
        }
    }
}