using UnityEngine;

[CreateAssetMenu(fileName = "QuestionDatabase", menuName = "Quiz/Question Database")]
public class QuestionDatabase : ScriptableObject
{
    public QuestionData[] questions;
}
