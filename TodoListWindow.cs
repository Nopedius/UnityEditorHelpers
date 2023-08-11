/*
Script Name: TodoListWindow
Description: A custom Unity Editor script that provides a To-Do List window within the Unity Editor.
             This window allows users to add, delete, and check/uncheck to-do items.
             The state of the list is saved to a JSON file, and persists between Unity sessions.
             The To-Do List data is saved to a file named "todolist.json" in the Application.persistentDataPath directory.
Usage: Click on "Window/Todo List" in Unity's top menu to open the To-Do List window.
*/

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TodoListWindow : EditorWindow
{
    private List<string> todoList = new List<string>();
    private List<bool> todoCheckList = new List<bool>();
    private string newTodoItem = "";
    private string filePath;

    [MenuItem("Window/Todo List")]
    static void Init()
    {
        TodoListWindow window = (TodoListWindow)EditorWindow.GetWindow(typeof(TodoListWindow));
        window.Show();
    }

    private void OnEnable()
    {
        filePath = Path.Combine(Application.persistentDataPath, "todolist.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            TodoListData data = JsonUtility.FromJson<TodoListData>(json);
            todoList = data.Tasks;
            todoCheckList = data.TaskStates;
        }
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Todo List", EditorStyles.boldLabel);

        for (int i = 0; i < todoList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            bool oldCheckState = todoCheckList[i];
            todoCheckList[i] = EditorGUILayout.Toggle(todoCheckList[i], GUILayout.MaxWidth(20f));
            EditorGUILayout.LabelField(todoList[i]);

            if (oldCheckState != todoCheckList[i])
            {
                SaveTodoList();
            }

            if (GUILayout.Button("Delete", GUILayout.MaxWidth(60f)))
            {
                todoList.RemoveAt(i);
                todoCheckList.RemoveAt(i);
                SaveTodoList();
            }

            EditorGUILayout.EndHorizontal();
        }

        newTodoItem = EditorGUILayout.TextField("New Task", newTodoItem);

        if (GUILayout.Button("Add Task"))
        {
            if (!string.IsNullOrEmpty(newTodoItem))
            {
                todoList.Add(newTodoItem);
                todoCheckList.Add(false);
                newTodoItem = "";
                SaveTodoList();
            }
        }
    }

    private void SaveTodoList()
    {
        TodoListData data = new TodoListData()
        {
            Tasks = todoList,
            TaskStates = todoCheckList
        };

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(filePath, json);

        // Check if file exists
        if (File.Exists(filePath))
        {
            Debug.Log("File saved successfully to " + filePath);
        }
        else
        {
            Debug.Log("Failed to save file to " + filePath);
        }
    }
}

[System.Serializable]
public class TodoListData
{
    public List<string> Tasks = new List<string>();
    public List<bool> TaskStates = new List<bool>();
}
