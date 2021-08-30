using BepInEx;
using CBKart.Utility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using USceneMgr = UnityEngine.SceneManagement.SceneManager;

namespace CustomKart
{
    [BepInPlugin("org.bepinex.plugins.customkart", "Custom Kart", "1.0.0.0")]
    public class CustomKart : BaseUnityPlugin
    {
        public const string KART_DIR = "CustomKart";
        public static string DATA_PATH = Path.Combine(Application.persistentDataPath, KART_DIR);
        private static Settings _settings;
        private static string _settingsPath = Path.Combine(DATA_PATH, "CustomKart.Settings.json");
        private Texture2D _tex;
        private Mesh _backElement;
        private Mesh _mainBody;
        private Mesh _pipes;
        private Mesh _seat;
        private Mesh _wheel;
        public static int TileSize;
        private static string _texName;
        private static List<Sprite> _sprites = new();

        private void Awake()
        {
            LoadSettings();

            if (!Directory.Exists(DATA_PATH))
            {
                Directory.CreateDirectory(DATA_PATH);
                return;
            }

            LoadTextures();
            LoadModels();

            USceneMgr.activeSceneChanged += OnSceneChange;
        }

        private void LoadSettings()
        {
            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);
                _settings = JsonUtility.FromJson<Settings>(json);
            }
            else
            {
                _settings = new Settings();
                string json = JsonUtility.ToJson(_settings, true);
                File.WriteAllText(_settingsPath, json);
            }
        }

        private void LoadModels()
        {
            var importer = new ObjImporter();
            if (File.Exists(Path.Combine(DATA_PATH, _settings.BackElement)))
            {
                _backElement = importer.ImportFile(Path.Combine(DATA_PATH, _settings.BackElement));
            }
            if (File.Exists(Path.Combine(DATA_PATH, _settings.MainBody)))
            {
                _mainBody = importer.ImportFile(Path.Combine(DATA_PATH, _settings.MainBody));
            }
            if (File.Exists(Path.Combine(DATA_PATH, _settings.Pipes)))
            {
                _pipes = importer.ImportFile(Path.Combine(DATA_PATH, _settings.Pipes));
            }
            if (File.Exists(Path.Combine(DATA_PATH, _settings.Seat)))
            {
                _seat = importer.ImportFile(Path.Combine(DATA_PATH, _settings.Seat));
            }
            if (File.Exists(Path.Combine(DATA_PATH, _settings.Wheel)))
            {
                _wheel = importer.ImportFile(Path.Combine(DATA_PATH, _settings.Wheel));
            }
        }

        private void LoadTextures()
        {
            string texPath = _settings.Sprites;
            if (File.Exists(Path.Combine(DATA_PATH, texPath)))
            {
                TileSize = _settings.TileSize;
                _texName = Path.GetFileNameWithoutExtension(texPath);
                byte[] texBytes = File.ReadAllBytes(Path.Combine(DATA_PATH, texPath));

                _tex = new Texture2D(2, 2);
                _tex.LoadImage(texBytes);
                _tex.name = _texName;

                for (int i = _tex.height - TileSize; i >= 0; i -= TileSize)
                {
                    for (int j = 0; j < _tex.width; j += TileSize)
                    {
                        var sprite = Sprite.Create(_tex, new Rect(j, i, TileSize, TileSize), new Vector2(0.5f, 0), TileSize / 2);
                        _sprites.Add(sprite);
                    }
                }
            }
        }

        private void OnSceneChange(Scene prevScene, Scene nextScene)
        {
            switch (nextScene.name)
            {
                case "The Forge":
                case "The Forge 2":
                case "The Forge 3":
                case "The Forge 4":
                case "The Forge 5":
                case "The Village":
                case "The Village 2":
                case "The Village 3":
                case "The Village 4":
                case "The Village 5":
                    StartCoroutine(ChangeKart());
                    break;
            }
        }

        private IEnumerator ChangeKart()
        {
            yield return new WaitWhile(() => GameObject.Find("Kart Drift RB Auto Online(Clone)") == null);

            var visualHolder = GameObject.Find("Kart Drift RB Auto Online(Clone)/Rotator/Visual Holder");
            Transform kart = visualHolder.transform.Find("Visual Shaker/cart_final");
            Transform character = kart.Find("PlayerRend");
            var spriteSel = character.GetComponent<DriveSpriteSelection>();
            for (int i = 0; i < spriteSel.SpritesRotation.Length; i++)
            {
                if (_sprites.Count > i)
                {
                    spriteSel.SpritesRotation[i] = _sprites[i];
                }
            }

            if (_backElement != null)
            {
                kart.Find("Back element").GetComponent<MeshFilter>().mesh = _backElement;
            }
            if (_mainBody != null)
            {
                kart.Find("Main Body").GetComponent<MeshFilter>().mesh = _mainBody;
            }
            if (_pipes != null)
            {
                kart.Find("pipes").GetComponent<MeshFilter>().mesh = _pipes;
                kart.Find("pipes (1)").GetComponent<MeshFilter>().mesh = _pipes;
            }
            if (_seat != null)
            {
                kart.Find("Seat").GetComponent<MeshFilter>().mesh = _seat;
            }
            if (_wheel != null)
            {
                visualHolder.transform.Find("Wheel FR/Visual Mesh").GetComponent<MeshFilter>().mesh = _wheel;
                visualHolder.transform.Find("Wheel BR/Visual Mesh").GetComponent<MeshFilter>().mesh = _wheel;
                visualHolder.transform.Find("Wheel FL/Visual Mesh").GetComponent<MeshFilter>().mesh = _wheel;
                visualHolder.transform.Find("Wheel BL/Visual Mesh").GetComponent<MeshFilter>().mesh = _wheel;
            }
        }
    }
}
