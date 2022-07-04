using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Steamworks;
using Multiplayer.Game;

namespace Multiplayer.GUI
{
    public static class GUI
    {
        public static Button CreateButton(Sprite sprite, Vector3 position, Vector2 sizeDelta)
        {
            GameObject buttonObj = new GameObject("New Button");

            Canvas can = UnityEngine.Object.FindObjectOfType<Canvas>();
            
            buttonObj.transform.SetParent(can.transform);
            buttonObj.transform.position = position;
            RectTransform rectText = buttonObj.AddComponent<RectTransform>();
            rectText.sizeDelta = sizeDelta;
            buttonObj.AddComponent<Image>().sprite = sprite;
            Button button = buttonObj.AddComponent<Button>();
            return button;
        }

        public static string SetColor(string text, ConsoleColor color)
        {
            return $"<color=\"{color.ToString().ToLower()}\">{text}</color>";
        }

        public static void TextNormilizeByDialogBox(DialogBox dialogBox)
        {
            dialogBox.TextMesh.alignment = TMPro.TextAlignmentOptions.Left;
            dialogBox.TextMesh.margin = new Vector4(50, 0, 50, 0);
        }

        public static async Task<Sprite> GetSpriteByAvatarAsync(SteamId member)
        {
            Sprite avatar = Mod.PlayerIcon;
            Steamworks.Data.Image? iconTask = await new Friend(member).GetLargeAvatarAsync();
            Steamworks.Data.Image icon = iconTask.Value;
            var texture = new Texture2D((int)icon.Width, (int)icon.Height, TextureFormat.RGBA32, false, true);
            texture.LoadRawTextureData(icon.Data);
            UnityEngine.Color[] pixs = texture.GetPixels();
            Array.Reverse(pixs, 0, pixs.Length);
            texture.SetPixels(pixs);
            texture.Apply();
            avatar = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            return avatar;
        }

        public static void AddPlayerCursor(Friend owner)
        {
            MapManager.SpawnCursor(owner, Input.mousePosition);
        }
    }

    public class UIElements : MonoBehaviour
    {
        public static Canvas GetCanvas => FindObjectOfType<Canvas>();

        public static Vector3 CANVAS_SIZE => GetCanvas.pixelRect.size;//new Vector3(Screen.width, Screen.height);

        public static Vector3 CANVAS_CENTER => new Vector3(CANVAS_SIZE.x / 2, CANVAS_SIZE.y / 2);

        public static Vector3 CANVAS_TOP_RIGHT => new Vector3(CANVAS_SIZE.x, CANVAS_SIZE.y);

        public static Vector3 CANVAS_TOP_LEFT => new Vector3(0, CANVAS_SIZE.y);

        public static Vector3 CANVAS_TOP_CENTER => new Vector3(CANVAS_SIZE.x / 2, CANVAS_SIZE.y);

        public static Vector3 CANVAS_BOTTOM_RIGHT => new Vector3(CANVAS_SIZE.x, 0);

        public static Vector3 CANVAS_BOTTOM_LEFT => new Vector3(0, 0);

        public static Vector3 CANVAS_BOTTOM_CENTER => new Vector3(CANVAS_SIZE.x / 2, 0);

        public static float CANVAS_SIZE_X => CANVAS_SIZE.x;

        public static float CANVAS_SIZE_Y => CANVAS_SIZE.y;
    }
}
