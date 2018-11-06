using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using AGS.API;

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

        public void Load(IEditorPlatform platform, IGame game, string basePath)
        {
            loadGuis(platform, game, getGuisFolder(basePath));
            loadRooms(platform, game, getRoomsFolder(basePath));
        }

        public void GenerateCode(string basePath, ICodeGenerator codeGenerator)
        {
            generateGuiCode(getGuisFolder(basePath), codeGenerator);
            generateRoomsCode(getRoomsFolder(basePath), codeGenerator);
        }

        public void Save(IEditorPlatform platform, IGame game, string basePath)
        {
            saveGuis(platform, game, getGuisFolder(basePath));
            saveRooms(platform, game, getRoomsFolder(basePath));
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

        private void saveEntity(IEditorPlatform platform, IGame game, string folder, string id)
        {
            if (id == null) return;
            if (!Entities.TryGetValue(id, out var entity))
            {
                Debug.WriteLine($"Missing entity {id}, cannot save it in {folder}.");
                return;
            }
            entity.Save(platform, game, folder);
        }

        private void saveGuis(IEditorPlatform platform, IGame game, string basePath)
        {
            Directory.CreateDirectory(basePath);

            foreach (var id in Guis)
            {
                saveEntity(platform, game, basePath, id);
            }
        }

        private void loadGuis(IEditorPlatform platform, IGame game, string basePath)
        {
            if (!Directory.Exists(basePath)) return;

            var guiFiles = Directory.GetFiles(basePath);
            foreach (var guiFile in guiFiles)
            {
                var gui = EntityModel.Load(platform, game, guiFile);
                if (gui == null)
                    continue;
                Entities.Add(gui.ID, gui);
                Guis.Add(gui.ID);
            }
        }

        private void saveRooms(IEditorPlatform platform, IGame game, string basePath)
        {
            Directory.CreateDirectory(basePath);

            foreach (var room in Rooms)
            {
                room.Save(platform, game, basePath);

                string roomFolder = Path.Combine(basePath, room.Folder);
                saveEntity(platform, game, roomFolder, room.BackgroundEntity);
                foreach (var id in room.Entities)
                {
                    saveEntity(platform, game, roomFolder, id);
                }
            }
        }

        private void loadRooms(IEditorPlatform platform, IGame game, string basePath)
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
                var room = RoomModel.Load(platform, game, roomJsonPath);
                Rooms.Add(room);
                var roomFiles = Directory.GetFiles(roomFolder);
                foreach (var roomFile in roomFiles)
                {
                    if (roomFile == roomJsonPath)
                        continue;

                    var entity = EntityModel.Load(platform, game, roomFile);
                    Entities.Add(entity.ID, entity);
                    if (entity.ID != room.BackgroundEntity)
                        room.Entities.Add(entity.ID);
                }
            }
        }
    }
}