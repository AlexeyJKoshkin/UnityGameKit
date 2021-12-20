using UnityEditor;

namespace GameKit.Editor
{
    public interface IOpenAdapter
    {
        string GetPath();
    }

    public class FolderOpenAdapter : IOpenAdapter
    {
        private readonly string _pathToSaves, NewName;

        public FolderOpenAdapter(string pathToSaves, string newName = "NewItem")
        {
            _pathToSaves = pathToSaves;
            NewName      = newName;
        }

        public string GetPath()
        {
            return EditorUtility.OpenFolderPanel("Куда сохранить ?", _pathToSaves, NewName);
        }
    }


    public class FileOpenAdapter : IOpenAdapter
    {
        private readonly string _pathToSaves;

        public FileOpenAdapter(string pathToSaves, string extension)
        {
            _pathToSaves = pathToSaves;
        }

        public string GetPath()
        {
            return EditorUtility.OpenFilePanel("Куда сохранить ?", _pathToSaves, "*.*");
        }
    }
}