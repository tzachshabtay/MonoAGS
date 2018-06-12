using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AGS.Editor
{
    public class StateModel
    {
        public StateModel()
        {
            Entities = new Dictionary<string, EntityModel>(500);
            Rooms = new List<RoomModel>(20);
            Guis = new HashSet<string>();
        }

        public Dictionary<string, EntityModel> Entities { get; }

        public List<RoomModel> Rooms { get; }

        public HashSet<string> Guis { get; }

        public void Load(string basePath)
        {
            loadGuis(basePath);
            loadRooms(basePath);
        }

        private void loadGuis(string basePath)
        {
            string guisPath = Path.Combine(basePath, "Guis");

            if (!Directory.Exists(guisPath)) return;

            var guiFiles = Directory.GetFiles(guisPath);
            foreach (var guiFile in guiFiles)
            {
                var gui = EntityModel.Load(guiFile);
                if (gui == null)
                    continue;
                Entities.Add(gui.ID, gui);
                Guis.Add(gui.ID);
            }
        }

        private void loadRooms(string basePath)
        {
            string roomsParentPath = Path.Combine(basePath, "Rooms");

            if (!Directory.Exists(roomsParentPath)) return;

            var roomFolders = Directory.GetDirectories(roomsParentPath);
            const string roomFilename = "room.json";
            foreach (var roomFolder in roomFolders)
            {
                var roomJsonPath = Path.Combine(roomFolder, roomFilename);
                if (!File.Exists(roomJsonPath))
                {
                    Debug.WriteLine($"Missing room json at {roomFolder}");
                    continue;
                }
                var room = RoomModel.Load(roomJsonPath);
                Rooms.Add(room);
                var roomFiles = Directory.GetFiles(roomFolder);
                foreach (var roomFile in roomFiles)
                {
                    if (roomFile == roomJsonPath)
                        continue;

                    var entity = EntityModel.Load(roomFile);
                    Entities.Add(entity.ID, entity);
                    if (entity.ID != room.BackgroundEntity)
                        room.Entities.Add(entity.ID);
                }
            }
        }
    }
}