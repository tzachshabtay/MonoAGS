using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

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
            loadGuis(getGuisFolder(basePath));
            loadRooms(getRoomsFolder(basePath));
        }

        public void GenerateCode(string basePath, ICodeGenerator codeGenerator)
        {
            generateGuiCode(getGuisFolder(basePath), codeGenerator);
            generateRoomsCode(getRoomsFolder(basePath), codeGenerator);
        }

        public void Save(string basePath)
        {
            saveGuis(getGuisFolder(basePath));
            saveRooms(getRoomsFolder(basePath));
        }

        private string getGuisFolder(string basePath) => Path.Combine(basePath, "GUIs");
        private string getRoomsFolder(string basePath) => Path.Combine(basePath, "Rooms");

        private void generateGuiCode(string basePath, ICodeGenerator codeGenerator)
        {
            foreach (var guiId in Guis)
            {
                generateEntityCode(guiId, basePath, codeGenerator);
            }
        }

        private void generateRoomsCode(string basePath, ICodeGenerator codeGenerator)
        {
            foreach (var room in Rooms)
            {
                string path = Path.Combine(basePath, room.ID);
                generateEntityCode(room.BackgroundEntity, path, codeGenerator);
                foreach (var id in room.Entities)
                {
                    generateEntityCode(id, path, codeGenerator);
                }
            }
        }

        private void generateEntityCode(string id, string path, ICodeGenerator codeGenerator)
        {
            if (id == null) return;
            path = Path.Combine(path, id);
            if (!Entities.TryGetValue(id, out var entity))
            {
                Debug.WriteLine($"Didn't find entity id {id}, can't generate code for it (class at {path}).");
                return;
            }
            StringBuilder code = new StringBuilder();
            codeGenerator.GenerateCode("MyGameNamespace", entity, code); //todo: namespace
            Debug.WriteLine($"Code for {id}:");
            Debug.WriteLine(code.ToString());
            Debug.WriteLine("----------");
        }

        private void saveEntity(string folder, string id)
        {
            if (id == null) return;
            if (!Entities.TryGetValue(id, out var entity))
            {
                Debug.WriteLine($"Missing entity {id}, cannot save it in {folder}.");
                return;
            }
            entity.Save(folder);
        }

        private void saveGuis(string basePath)
        {
            Directory.CreateDirectory(basePath);

            foreach (var id in Guis)
            {
                saveEntity(basePath, id);
            }
        }

        private void loadGuis(string basePath)
        {
            if (!Directory.Exists(basePath)) return;

            var guiFiles = Directory.GetFiles(basePath);
            foreach (var guiFile in guiFiles)
            {
                var gui = EntityModel.Load(guiFile);
                if (gui == null)
                    continue;
                Entities.Add(gui.ID, gui);
                Guis.Add(gui.ID);
            }
        }

        private void saveRooms(string basePath)
        {
            Directory.CreateDirectory(basePath);

            foreach (var room in Rooms)
            {
                room.Save(basePath);

                string roomFolder = Path.Combine(basePath, room.Folder);
                saveEntity(roomFolder, room.BackgroundEntity);
                foreach (var id in room.Entities)
                {
                    saveEntity(roomFolder, id);
                }
            }
        }

        private void loadRooms(string basePath)
        {
            if (!Directory.Exists(basePath)) return;

            var roomFolders = Directory.GetDirectories(basePath);
            const string roomFilename = RoomModel.Filename;
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