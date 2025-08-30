using System.Text.Json;
using DungeonCodingAgent.Game.Core;
using DungeonCodingAgent.Game.Persistence;

namespace DungeonCodingAgent.Game.Systems;

/// <summary>
/// Save game system that handles persistent storage of game state to JSON files.
/// Implements atomic save operations and comprehensive error handling.
/// </summary>
public class SaveGameSystem
{
    private readonly string _saveDirectory;
    private readonly string _quickSaveFile;
    private readonly JsonSerializerOptions _jsonOptions;

    public SaveGameSystem(string? saveDirectory = null)
    {
        _saveDirectory = saveDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "DungeonCodingAgent",
            "saves");
        
        _quickSaveFile = Path.Combine(_saveDirectory, "quicksave.json");
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };
        
        // Subscribe to save/load events
        GameEvents.SaveRequested += HandleSaveRequested;
        GameEvents.LoadRequested += HandleLoadRequested;
        
        EnsureSaveDirectoryExists();
    }

    /// <summary>
    /// Save the current game state to the quicksave file
    /// </summary>
    public async Task<bool> SaveGameAsync(GameSaveData gameData)
    {
        try
        {
            if (!CanSaveGame())
            {
                GameEvents.RaiseSaveLoadError("Cannot save during combat or menu screens");
                return false;
            }

            // Update save metadata
            gameData.SaveDate = DateTime.UtcNow;
            
            // Use atomic save operation (temp file + rename)
            var tempFile = _quickSaveFile + ".tmp";
            
            var json = JsonSerializer.Serialize(gameData, _jsonOptions);
            await File.WriteAllTextAsync(tempFile, json);
            
            // Atomic rename operation
            if (File.Exists(_quickSaveFile))
            {
                File.Delete(_quickSaveFile);
            }
            File.Move(tempFile, _quickSaveFile);
            
            GameEvents.RaiseGameSaved(_quickSaveFile);
            GameEvents.RaiseMessageLogged("Game saved successfully");
            
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            var error = $"File permission error: {ex.Message}";
            GameEvents.RaiseSaveLoadError(error);
            GameEvents.RaiseMessageLogged(error);
            return false;
        }
        catch (DirectoryNotFoundException ex)
        {
            var error = $"Save directory not found: {ex.Message}";
            GameEvents.RaiseSaveLoadError(error);
            GameEvents.RaiseMessageLogged(error);
            return false;
        }
        catch (IOException ex)
        {
            var error = $"I/O error during save: {ex.Message}";
            GameEvents.RaiseSaveLoadError(error);
            GameEvents.RaiseMessageLogged(error);
            return false;
        }
        catch (Exception ex)
        {
            var error = $"Unexpected error during save: {ex.Message}";
            GameEvents.RaiseSaveLoadError(error);
            GameEvents.RaiseMessageLogged(error);
            return false;
        }
    }

    /// <summary>
    /// Load game state from the quicksave file
    /// </summary>
    public async Task<(bool Success, GameSaveData? GameData)> LoadGameAsync()
    {
        try
        {
            if (!File.Exists(_quickSaveFile))
            {
                GameEvents.RaiseSaveLoadError("No save file found");
                GameEvents.RaiseMessageLogged("No save file found");
                return (false, null);
            }

            var json = await File.ReadAllTextAsync(_quickSaveFile);
            
            if (string.IsNullOrWhiteSpace(json))
            {
                GameEvents.RaiseSaveLoadError("Save file is empty or corrupted");
                GameEvents.RaiseMessageLogged("Save file is empty or corrupted");
                return (false, null);
            }

            var gameData = JsonSerializer.Deserialize<GameSaveData>(json, _jsonOptions);
            
            if (gameData == null)
            {
                GameEvents.RaiseSaveLoadError("Failed to parse save file - corrupted data");
                GameEvents.RaiseMessageLogged("Failed to parse save file - corrupted data");
                return (false, null);
            }

            // Validate save file format
            if (!ValidateSaveData(gameData))
            {
                GameEvents.RaiseSaveLoadError("Save file format is invalid or corrupted");
                GameEvents.RaiseMessageLogged("Save file format is invalid or corrupted");
                return (false, null);
            }

            GameEvents.RaiseGameLoaded(_quickSaveFile);
            GameEvents.RaiseMessageLogged("Game loaded successfully");
            
            return (true, gameData);
        }
        catch (JsonException ex)
        {
            var error = $"Corrupted save file detected: {ex.Message}";
            GameEvents.RaiseSaveLoadError(error);
            GameEvents.RaiseMessageLogged(error);
            return (false, null);
        }
        catch (UnauthorizedAccessException ex)
        {
            var error = $"File permission error: {ex.Message}";
            GameEvents.RaiseSaveLoadError(error);
            GameEvents.RaiseMessageLogged(error);
            return (false, null);
        }
        catch (FileNotFoundException)
        {
            GameEvents.RaiseSaveLoadError("No save file found");
            GameEvents.RaiseMessageLogged("No save file found");
            return (false, null);
        }
        catch (IOException ex)
        {
            var error = $"I/O error during load: {ex.Message}";
            GameEvents.RaiseSaveLoadError(error);
            GameEvents.RaiseMessageLogged(error);
            return (false, null);
        }
        catch (Exception ex)
        {
            var error = $"Unexpected error during load: {ex.Message}";
            GameEvents.RaiseSaveLoadError(error);
            GameEvents.RaiseMessageLogged(error);
            return (false, null);
        }
    }

    /// <summary>
    /// Check if a save file exists
    /// </summary>
    public bool SaveFileExists() => File.Exists(_quickSaveFile);

    /// <summary>
    /// Get save file information
    /// </summary>
    public (DateTime SaveDate, long FileSize)? GetSaveInfo()
    {
        if (!File.Exists(_quickSaveFile))
            return null;

        var fileInfo = new FileInfo(_quickSaveFile);
        return (fileInfo.LastWriteTime, fileInfo.Length);
    }

    /// <summary>
    /// Delete the save file
    /// </summary>
    public bool DeleteSave()
    {
        try
        {
            if (File.Exists(_quickSaveFile))
            {
                File.Delete(_quickSaveFile);
                GameEvents.RaiseMessageLogged("Save file deleted");
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            GameEvents.RaiseSaveLoadError($"Error deleting save file: {ex.Message}");
            return false;
        }
    }

    private void HandleSaveRequested()
    {
        // This would be called by the game engine when Ctrl+S is pressed
        // For now, we'll just log that a save was requested
        GameEvents.RaiseMessageLogged("Save requested - implement in game engine");
    }

    private void HandleLoadRequested()
    {
        // This would be called by the game engine when Ctrl+L is pressed
        // For now, we'll just log that a load was requested
        GameEvents.RaiseMessageLogged("Load requested - implement in game engine");
    }

    private void EnsureSaveDirectoryExists()
    {
        try
        {
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }
        }
        catch (Exception ex)
        {
            GameEvents.RaiseSaveLoadError($"Failed to create save directory: {ex.Message}");
        }
    }

    private bool CanSaveGame()
    {
        // For now, allow saving in any state except during actual combat/menus
        // This will be expanded when the game state system is fully implemented
        return true;
    }

    private bool ValidateSaveData(GameSaveData gameData)
    {
        // Basic validation of save data structure
        return !string.IsNullOrEmpty(gameData.Version) &&
               gameData.Player != null &&
               gameData.Map != null &&
               gameData.Entities != null &&
               gameData.Inventory != null;
    }

    public void Dispose()
    {
        GameEvents.SaveRequested -= HandleSaveRequested;
        GameEvents.LoadRequested -= HandleLoadRequested;
    }
}