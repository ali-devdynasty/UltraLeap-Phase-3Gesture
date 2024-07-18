using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SaveManager
{
    private string _dataPath;
    
    public SaveManager()
    {
        _dataPath = Path.Combine(Application.persistentDataPath, "session_data.json");
    }

    public void SaveSessionData(SessionData data)
    {
        _dataPath = Path.Combine(Application.persistentDataPath,data.SessionId.ToString()+data.ParticipantId.ToString() + ".json");
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(_dataPath, jsonData);
    }

    public SessionData LoadSessionData(string sessionId, string participantId)
    {
        _dataPath = Path.Combine(Application.persistentDataPath, sessionId + participantId +".json");
        SessionData data = null;

        if (File.Exists(_dataPath))
        {
            string jsonData = File.ReadAllText(_dataPath);
            data = JsonUtility.FromJson<SessionData>(jsonData);

            if (data.SessionId != sessionId || data.ParticipantId != participantId)
            {
                data = null;
            }
        }

        return data;
    }

    public string GetFormattedDataString(SessionData data)
    {
        StringBuilder builder = new StringBuilder();

        if (!string.IsNullOrEmpty(data.SessionId))
            builder.AppendLine($"Session ID: {data.SessionId}");

        if (!string.IsNullOrEmpty(data.ParticipantId))
            builder.AppendLine($"Participant ID: {data.ParticipantId}");

        builder.AppendLine(); // Empty line
        if (!string.IsNullOrEmpty(data.TotalSessionTime))
            builder.AppendLine($"Total Session Time: {data.TotalSessionTime}");

        if (!string.IsNullOrEmpty(data.sessionState.ToString()))
            builder.AppendLine($"Session State: {data.sessionState}");

        builder.AppendLine(); // Empty line
        builder.AppendLine("Groups:");

        for (int i = 0; i < data.groupData.Count; i++)
        {
            if (data.groupData[i].State == State.NotPlayed)
                continue; // Skip this group if the state is NotPlayed

            builder.AppendLine($"- Group ID: {data.groupData[i].GroupId}");

            if (data.groupData[i].State != State.NotPlayed)
                builder.AppendLine($"- Session State: {data.groupData[i].State}");

            if (!string.IsNullOrEmpty(data.groupData[i].TotalTime))
                builder.AppendLine($"- Total Time: {data.groupData[i].TotalTime}");

            builder.AppendLine(); // Empty line
            if (data.groupData[i].tasks != null && data.groupData[i].tasks.Count > 0)
            {
                builder.AppendLine($"- Tasks:");

                for (int j = 0; j < data.groupData[i].tasks.Count; j++)
                {
                    if (!string.IsNullOrEmpty(data.groupData[i].tasks[j].TaskNo))
                        builder.AppendLine($"-- Task No: {data.groupData[i].GroupId + "."+data.groupData[i].tasks[j].TaskNo}");

                    if (!string.IsNullOrEmpty(data.groupData[i].tasks[j].CompletedTimes))
                        builder.AppendLine($"-- Completed Times: {data.groupData[i].tasks[j].CompletedTimes}");

                    builder.AppendLine(); // Empty line
                    if (data.groupData[i].tasks[j].subTasks != null && data.groupData[i].tasks[j].subTasks.Count > 0)
                    {
                        builder.AppendLine($"-- Subtasks:");

                        for (int k = 0; k < data.groupData[i].tasks[j].subTasks.Count; k++)
                        {
                            builder.AppendLine($"--- Subtask No : {data.groupData[i].GroupId +"."+ data.groupData[i].tasks[j].TaskNo +"-"+ data.groupData[i].tasks[j].subTasks[k].SubTaskNo}");
                            if (!string.IsNullOrEmpty(data.groupData[i].tasks[j].subTasks[k].handDetectedForFirstTime))
                                builder.AppendLine($"--- Hand Detected For First Time: {data.groupData[i].tasks[j].subTasks[k].handDetectedForFirstTime}");

                            // Handle Left and Right Hand and Finger Movements here
                            if (data.groupData[i].tasks[j].subTasks[k].LefthandAndFingureMovement != null)
                            {
                                builder.AppendLine("--- Left Hand and Finger Movements:");
                                foreach (var leftHandMovement in data.groupData[i].tasks[j].subTasks[k].LefthandAndFingureMovement)
                                {
                                    builder.AppendLine();
                                    builder.AppendLine($"---- Time: {leftHandMovement.time}");
                                    builder.AppendLine($"---- Hand Position: {leftHandMovement.handPos}");
                                    builder.AppendLine($"---- Pinch Strength: {leftHandMovement.pinchStrenght}");
                                    builder.AppendLine($"---- Grab Strength: {leftHandMovement.grabStrenght}");
                                    builder.AppendLine($"---- Hand Velocity: {leftHandMovement.handVeclocity}");
                                    builder.AppendLine($"---- Hand Rotation: {leftHandMovement.handRotation}");

                                    builder.AppendLine("---- Finger Positions:");
                                    foreach (var finger in leftHandMovement.finguePos)
                                    {
                                        builder.AppendLine();
                                        builder.AppendLine($"------ Finger No: {leftHandMovement.finguePos.IndexOf(finger) + 1}");
                                        builder.AppendLine($"------ Finger Position: {finger.fingurePos}");
                                        builder.AppendLine($"------ Is Extended: {finger.isExtended}");
                                        builder.AppendLine($"------ Direction: {finger.direction}");
                                    }

                                    builder.AppendLine(); // Empty line
                                }
                            }

                            if (data.groupData[i].tasks[j].subTasks[k].RighthandAndFingureMovement != null)
                            {
                                builder.AppendLine("--- Right Hand and Finger Movements:");
                                foreach (var rightHandMovement in data.groupData[i].tasks[j].subTasks[k].RighthandAndFingureMovement)
                                {
                                    builder.AppendLine();
                                    builder.AppendLine($"---- Time: {rightHandMovement.time}");
                                    builder.AppendLine($"---- Hand Position: {rightHandMovement.handPos}");
                                    builder.AppendLine($"---- Pinch Strength: {rightHandMovement.pinchStrenght}");
                                    builder.AppendLine($"---- Grab Strength: {rightHandMovement.grabStrenght}");
                                    builder.AppendLine($"---- Hand Velocity: {rightHandMovement.handVeclocity}");
                                    builder.AppendLine($"---- Hand Rotation: {rightHandMovement.handRotation}");

                                    builder.AppendLine("---- Finger Positions:");
                                    foreach (var finger in rightHandMovement.finguePos)
                                    {
                                        builder.AppendLine();
                                        builder.AppendLine($"------ Finger No: {rightHandMovement.finguePos.IndexOf(finger)+1}");
                                        builder.AppendLine($"------ Finger Position: {finger.fingurePos}");
                                        builder.AppendLine($"------ Is Extended: {finger.isExtended}");
                                        builder.AppendLine($"------ Direction: {finger.direction}");
                                    }

                                    builder.AppendLine(); // Empty line
                                }
                            }


                            if (!string.IsNullOrEmpty(data.groupData[i].tasks[j].subTasks[k].timeWhenTask_COmpleted))
                                builder.AppendLine($"--- Time When Task Completed: {data.groupData[i].tasks[j].subTasks[k].timeWhenTask_COmpleted}");

                            if (!string.IsNullOrEmpty(data.groupData[i].tasks[j].subTasks[k].totalTime))
                                builder.AppendLine($"--- Total Time: {data.groupData[i].tasks[j].subTasks[k].totalTime}");

                            if (data.groupData[i].tasks[j].subTasks[k].state != null)
                                builder.AppendLine($"--- State: {data.groupData[i].tasks[j].subTasks[k].state}");

                            builder.AppendLine(); // Empty line
                        }
                    }
                }
            }
        }

        return builder.ToString();
    }






}