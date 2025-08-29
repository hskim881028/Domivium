using UnityEngine;

namespace Domivium.Client.Core.Message
{
    public readonly struct InputMessage
    {
        public InputMessageType Type { get; }

        public Vector2 Value { get; }

        private InputMessage(InputMessageType type)
        {
            Type = type;
            Value = Vector2.zero;
        }

        private InputMessage(InputMessageType type, Vector2 value)
        {
            Type = type;
            Value = value;
        }

        public static InputMessage Submit => new(InputMessageType.Submit);
        public static InputMessage Cancel => new(InputMessageType.Cancel);
        public static InputMessage Click(Vector2 value) => new(InputMessageType.Click, value);
        public static InputMessage Point(Vector2 value) => new(InputMessageType.Point, value);
    }
}