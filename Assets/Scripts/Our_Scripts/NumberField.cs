using TMPro;
using UnityEngine;

/**
 * This component should be attached to a TextMeshPro object.
 * It allows to feed an integer number to the text field with a label.
 */
[RequireComponent(typeof(TextMeshPro))]
public class NumberField : MonoBehaviour
{
    private int number;

    [SerializeField]
    private string label = "Health:"; // Label for the text (e.g., "Health:")

    public int GetNumber()
    {
        return this.number;
    }

    public void SetNumber(int newNumber)
    {
        this.number = newNumber;
        GetComponent<TextMeshPro>().text = $"{label} {newNumber}";  // Combine the label and the number
    }

    public void AddNumber(int toAdd)
    {
        SetNumber(this.number + toAdd);
    }
}
