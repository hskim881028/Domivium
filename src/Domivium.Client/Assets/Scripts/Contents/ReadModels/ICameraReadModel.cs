using UnityEngine;

namespace Domivium.Client.Contents.ReadModels
{
    public interface ICameraReadModel
    {
        public Camera MainCamera { get; }
        public Camera UICamera { get; }
    }
}