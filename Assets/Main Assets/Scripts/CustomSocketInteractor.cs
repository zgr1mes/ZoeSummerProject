using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace CustomXR
{
    [AddComponentMenu("XR/Interactors/Custom Socket Interactor")]
    public class CustomSocketInteractor : XRSocketInteractor
    {
        [Header("Custom Allowed Objects")]
        [SerializeField]
        List<GameObject> allowedObjects = new();

        public List<GameObject> AllowedObjects => allowedObjects;

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            GameObject candidate = interactable.transform.gameObject;
            bool isAllowed = allowedObjects.Contains(candidate);

            return isAllowed &&
                   base.CanSelect(interactable) &&
                   ((!hasSelection && !interactable.isSelected) ||
                    (IsSelecting(interactable) && interactable.interactorsSelecting.Count == 1));
        }

        protected override void DrawHoveredInteractables()
        {
            if (!showInteractableHoverMeshes || interactableHoverScale <= 0f)
                return;

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                return;

            foreach (var interactable in interactablesHovered)
            {
                if (interactable == null || IsSelecting(interactable))
                    continue;

                GameObject go = interactable.transform.gameObject;
                if (!allowedObjects.Contains(go))
                    continue;

                var meshFilters = interactable.transform.GetComponentsInChildren<MeshFilter>(true);
                if (meshFilters == null || meshFilters.Length == 0)
                    continue;

                var attachTransform = GetAttachTransform(interactable);
                Material material = GetHoveredInteractableMaterial(interactable);
                if (material == null)
                    continue;

                foreach (var meshFilter in meshFilters)
                {
                    if (meshFilter == null || meshFilter.sharedMesh == null)
                        continue;

                    Renderer renderer = meshFilter.GetComponent<Renderer>();
                    if (renderer == null || !renderer.enabled)
                        continue;

                    if ((mainCamera.cullingMask & (1 << meshFilter.gameObject.layer)) == 0)
                        continue;

                    Vector3 offset = meshFilter.transform.position - interactable.transform.position;
                    Quaternion rotationOffset = Quaternion.Inverse(interactable.transform.rotation) * meshFilter.transform.rotation;

                    Matrix4x4 matrix = Matrix4x4.TRS(
                        attachTransform.position + attachTransform.rotation * offset,
                        attachTransform.rotation * rotationOffset,
                        meshFilter.transform.lossyScale * interactableHoverScale);

                    Mesh mesh = meshFilter.sharedMesh;
                    for (int i = 0; i < mesh.subMeshCount; ++i)
                    {
                        Graphics.DrawMesh(mesh, matrix, material, gameObject.layer, null, i);
                    }
                }
            }
        }
    }
}