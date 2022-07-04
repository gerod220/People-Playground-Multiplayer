using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Multiplayer.Game
{
    internal static class MapManager
    {
        public static Map GetMapByName(string name)
        {
            foreach (var map in MapRegistry.GetAllMaps())
            {
                if (map.name == name) return map;
            }
            return null;
        }

        public static bool Load(Map map)
        {
            if (map == null) return false;

            MapLoaderBehaviour.CurrentMap = map;
            foreach (GameObject gameObject in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (gameObject.ToString() == "Play button (UnityEngine.GameObject)")
                {
                    gameObject.GetComponent<SceneSwitchBehaviour>().Switch();
                    return true;
                }
            }
            return false;
        }

        public static bool SpawnObject(string name, Vector3 pos)
        {
            if (CatalogBehaviour.Main?.Catalog != null) return false;

            SpawnableAsset sa = null;
            foreach (SpawnableAsset spawnableAsset in CatalogBehaviour.Main?.Catalog.Items)
            {
                if (name == spawnableAsset.name)
                {
                    sa = spawnableAsset;
                    break;
                }
            }

            if (sa != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(sa.Prefab, pos, Quaternion.identity);
                //gameObject.AddComponent<Network.NetworkObjectController>();
                gameObject.AddComponent<TexturePackApplier>();
                gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
                gameObject.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = sa;
                gameObject.name = sa.name;
                return true;
            }
            return false;
            /*
            if (flipped)
            {
                Vector3 localScale = gameObject.transform.localScale;
                localScale.x *= -1f;
                gameObject.transform.localScale = localScale;
            }
            */
        }

        public static bool SpawnCursor(Friend client, Vector3 startPos)
        {
            //if (!CatalogBehaviour.Main) return false;

            SpawnableAsset sa = CatalogBehaviour.Main.GetSpawnable("Brick");

            if (sa)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(sa.Prefab, startPos, Quaternion.identity);
                gameObject.name = "Cursor";
                gameObject.AddComponent<GUI.Cursor>();
                gameObject.AddComponent<TexturePackApplier>();
                gameObject.AddComponent<AudioSourceTimeScaleBehaviour>();
                gameObject.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = sa;
                gameObject.name = sa.name;

                SpriteRenderer spriteRenderer;
                gameObject.TryGetComponent<SpriteRenderer>(out spriteRenderer);
                if(spriteRenderer)
                {
                    spriteRenderer.sprite = Mod.CursorIcon;
                }

                PhysicalBehaviour physicalBehaviour;
                if (gameObject.TryGetComponent<PhysicalBehaviour>(out physicalBehaviour))
                {
                    UnityEngine.Object.Destroy(physicalBehaviour);
                }

                Rigidbody2D rigidbody2D;
                if (gameObject.TryGetComponent<Rigidbody2D>(out rigidbody2D))
                {
                    UnityEngine.Object.Destroy(rigidbody2D);
                }

                Collider2D collider2D;
                if (gameObject.TryGetComponent<Collider2D>(out collider2D))
                {
                    UnityEngine.Object.Destroy(collider2D);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async void AddToMap(Friend player)
        {
            //if (player.Id == Client.ClientManager.CurrentLobby.Owner.Id) return;
            for (int i = 1; i < Client.ClientManager.CurrentLobby.MaxMembers+1; i++)
            {
                var prefab = CatalogBehaviour.Main.GetSpawnable("PlayerPrefabAsset" + i);
                if(!prefab.VisibleInCatalog)
                {
                    prefab.name = player.Name;
                    prefab.ViewSprite = await GUI.GUI.GetSpriteByAvatarAsync(player.Id);
                    prefab.VisibleInCatalog = true;
                    CatalogBehaviour.Main.CreateItemButtons();
                    break;
                }
            }
            
            GUI.GUI.AddPlayerCursor(player);
        }

        public static void DeleteFromMap(Friend player)
        {
            for (int i = 1; i < Client.ClientManager.CurrentLobby.MaxMembers+1; i++)
            {
                var prefab = CatalogBehaviour.Main.GetSpawnable("PlayerPrefabAsset" + i);
                if(prefab.VisibleInCatalog && prefab.name == player.Name)
                {
                    prefab.ViewSprite = Mod.PlayerIcon;
                    prefab.VisibleInCatalog = false;
                    CatalogBehaviour.Main.CreateItemButtons();
                    break;
                }
            }

            //GUI.GUI.DeletePlayerCursor(player);
        }

        public static Category PlayersCategory;
        public static SpawnableAsset PrewCurSelectItem;
        public static void CreateCategory(string name, string description, Sprite icon)
        {
            CatalogBehaviour manager = UnityEngine.Object.FindObjectOfType<CatalogBehaviour>();
            if (manager.Catalog.Categories.FirstOrDefault((Category c) => c.name == name) == null)
            {
                Category category = ScriptableObject.CreateInstance<Category>();
                category.name = name;
                category.Description = description;
                category.Icon = icon;

                var list = manager.Catalog.Categories.ToList(); 
                list.Insert(0, category);
                manager.Catalog.Categories = list.ToArray();
                PlayersCategory = category;
            }
        }

        public static void DeleteCategory(Category targetCategory)
        {
            CatalogBehaviour manager = UnityEngine.Object.FindObjectOfType<CatalogBehaviour>();
            var list = manager.Catalog.Categories.ToList();
            list.Remove(targetCategory);
            manager.Catalog.Categories = list.ToArray();
        }

        public static void RegisterPlayersListPrefabs()
        {
            for (int i = 1; i < Client.ClientManager.CurrentLobby.MaxMembers+1; i++)
            {
                var PlayerListPrefabAsset = ScriptableObject.CreateInstance<SpawnableAsset>();
                PlayerListPrefabAsset.name = "PlayerPrefabAsset" + i;
                PlayerListPrefabAsset.Category = PlayersCategory;
                PlayerListPrefabAsset.Description = "PlayerPrefabAsset";
                PlayerListPrefabAsset.VisibleInCatalog = false;
                PlayerListPrefabAsset.ViewSprite = Mod.CursorIcon;
                PlayerListPrefabAsset.Prefab = new GameObject("PlayerPrefabAsset"+i);
                
                var PlayerListPrefab = new Modification()
                {
                    NameOverride = "PlayerPrefabAsset" + i,
                    CategoryOverride = PlayersCategory,
                    OriginalItem = PlayerListPrefabAsset,
                    DescriptionOverride = "Client",
                    AfterSpawn = (x) => { },
                };
                
                ModAPI.Register(PlayerListPrefab);
            }
        }
    }
}