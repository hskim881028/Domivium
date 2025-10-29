using UnityEngine;

namespace Domivium.Client.Contents.System.Model
{
    public interface ICameraSystemModel
    {
        public Camera MainCamera { get; }
        public Camera UICamera { get; }
    }
}