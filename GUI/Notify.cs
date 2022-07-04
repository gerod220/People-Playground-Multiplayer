using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.GUI
{
    internal static class Notify
    {
        private static DialogBox _curDialogBox = null;
        private static NotifyController _controller = null;

        public static void Show(string text, float time)
        {
            time *= 2;
            NewNotify(text, time);   
        }

        private static void NewNotify(string text, float time)
        {
            if(_controller == null)
            {
                _controller = new GameObject("MP:NotifyController").AddComponent<NotifyController>();
            }

            var task = new DialogTask();
            task.text = text;
            task.time = time;

            _controller.Tasks.Add(task);
        }

        public static void Remove()
        {
            if (_curDialogBox == null) return;

            _curDialogBox?.Close();
            _curDialogBox = null;
        }

        private class DialogTask
        {
            public DialogBox Dialog { get; private set; }
            public string text;
            public float time;

            public void Show()
            {
                UISoundBehaviour.Main.AudioSource.PlayOneShot(Mod.NotifySound);

                Dialog = DialogBoxManager.Dialog("[UI:Loading..]", new DialogButton[0]);
                Dialog.SetTitle(text);
                Dialog.transform.position = UIElements.CANVAS_TOP_RIGHT - new Vector3(220, 60);
            }
        }

        private class NotifyController : MonoBehaviour
        {
            public List<DialogTask> Tasks = new List<DialogTask>();
            private void Start()
            {
                DontDestroyOnLoad(this);
                DontDestroyOnLoad(gameObject);
            }

            private void Update()
            {
                if (Tasks.Count < 1) return;
                if (_curDialogBox == null)
                {
                    Tasks[0].Show();
                    _curDialogBox = Tasks[0].Dialog;
                }

                Tasks[0].time -= Time.fixedDeltaTime;

                if (Tasks[0].time < 1)
                {
                    Tasks.RemoveAt(0);
                    Remove();
                }

                if (Tasks.Count < 1) _curDialogBox = null;
            }
        }
    }
}
