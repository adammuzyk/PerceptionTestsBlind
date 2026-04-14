using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PerceptionTests.Domain;

namespace PerceptionTests.Services
{
    public class ResultPersistenceService
    {
        private readonly RuntimeSettings _settings;

        public ResultPersistenceService(RuntimeSettings settings)
        {
            _settings = settings;
        }

        public PersistenceResult SaveCheckpoint(ExperimentRunState state)
        {
            string checkpointPath = null;
            try
            {
                var checkpointDirectory = Path.Combine(_settings.ResultPath, "checkpoints");
                Directory.CreateDirectory(checkpointDirectory);

                checkpointPath = Path.Combine(checkpointDirectory, "checkpoint_" + state.SessionId + ".json");
                var exportDocument = ExportMapper.Map(state, DateTime.UtcNow);
                WriteJsonFile(checkpointPath, exportDocument);

                return new PersistenceResult
                {
                    Success = true,
                    FilePath = checkpointPath
                };
            }
            catch (Exception ex)
            {
                return new PersistenceResult
                {
                    Success = false,
                    ErrorMessage = BuildSaveErrorMessage(checkpointPath, ex)
                };
            }
        }

        public PersistenceResult SaveFinal(ExperimentRunState state)
        {
            string finalPath = null;
            try
            {
                Directory.CreateDirectory(_settings.ResultPath);
                finalPath = GenerateFinalFileName();
                var exportDocument = ExportMapper.Map(state, DateTime.UtcNow);
                WriteJsonFile(finalPath, exportDocument);
                DeleteCheckpointIfExists(state.SessionId);

                return new PersistenceResult
                {
                    Success = true,
                    FilePath = finalPath
                };
            }
            catch (Exception ex)
            {
                return new PersistenceResult
                {
                    Success = false,
                    ErrorMessage = BuildSaveErrorMessage(finalPath, ex)
                };
            }
        }

        private string GenerateFinalFileName()
        {
            const string baseFileName = "testResult_";
            const string extension = ".json";
            int fileNumber = 1;
            var dirInfo = new DirectoryInfo(_settings.ResultPath);
            var existingFiles = dirInfo.EnumerateFiles("testResult_*.json").ToList();

            while (existingFiles.Any(f => f.Name.Equals(GetFileName(baseFileName, fileNumber, extension), StringComparison.OrdinalIgnoreCase)))
            {
                fileNumber++;
            }

            return Path.Combine(_settings.ResultPath, GetFileName(baseFileName, fileNumber, extension));
        }

        private static string GetFileName(string baseFileName, int fileNumber, string extension)
        {
            return string.Format("{0}{1:0000}{2}", baseFileName, fileNumber, extension);
        }

        private static void WriteJsonFile(string fullPath, object payload)
        {
            var serialized = JsonConvert.SerializeObject(payload, Formatting.Indented);
            File.WriteAllText(fullPath, serialized, Encoding.UTF8);
        }

        private void DeleteCheckpointIfExists(string sessionId)
        {
            var checkpointPath = Path.Combine(_settings.ResultPath, "checkpoints", "checkpoint_" + sessionId + ".json");
            if (File.Exists(checkpointPath))
            {
                File.Delete(checkpointPath);
            }
        }

        private static string BuildSaveErrorMessage(string filePath, Exception ex)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return ex.Message;
            }

            return "File: " + filePath + "\n" + ex.Message;
        }
    }
}
