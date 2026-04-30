using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gemserk
{
    [FilePath("Gemserk/Gemserk.Favorites.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class FavoritesAsset : ScriptableSingleton<FavoritesAsset>
    {
        [Serializable]
        public class Favorite
        {
            public LazyLoadReference<Object> reference;
            public string assetPath;
        }

        public event Action<FavoritesAsset> OnFavoritesUpdated;

        [SerializeField]
        public List<Favorite> favoritesList = new List<Favorite>();

        // Session-only pins for scene objects (instance IDs, not serialized).
        [NonSerialized]
        private readonly HashSet<int> _sessionPins = new HashSet<int>();

        public void AddFavorite(Favorite favorite)
        {
            favoritesList.Add(favorite);
            OnFavoritesUpdated?.Invoke(this);
            Save(true);
        }

        // Pin a scene hierarchy object for this session (not persisted).
        public void PinSceneObject(Object reference)
        {
            if (reference == null) return;
            _sessionPins.Add(reference.GetInstanceID());
            OnFavoritesUpdated?.Invoke(this);
        }

        public bool IsFavorite(Object reference)
        {
            if (reference == null) return false;
            var id = reference.GetInstanceID();
            return favoritesList.Any(f => f.reference.isSet && f.reference.instanceID == id)
                || _sessionPins.Contains(id);
        }

        public void RemoveFavorite(Object reference)
        {
            if (reference == null) return;
            var id = reference.GetInstanceID();
            favoritesList.RemoveAll(f => f.reference.isSet && f.reference.instanceID == id);
            _sessionPins.Remove(id);
            OnFavoritesUpdated?.Invoke(this);
            Save(true);
        }

        public void ClearAll()
        {
            favoritesList.Clear();
            _sessionPins.Clear();
            OnFavoritesUpdated?.Invoke(this);
            Save(true);
        }

        public void RemoveAtIndex(int index)
        {
            if (index < 0 || index >= favoritesList.Count)
                return;

            favoritesList.RemoveAt(index);
            OnFavoritesUpdated?.Invoke(this);
            Save(true);
        }

        public void OnFavoritesModified()
        {
            foreach (var favorite in favoritesList)
            {
                if (!favorite.reference.isBroken && favorite.reference.isSet)
                {
                    favorite.assetPath = AssetDatabase.GetAssetPath(favorite.reference.asset);
                }
            }
            Save(true);
        }
    }
}
