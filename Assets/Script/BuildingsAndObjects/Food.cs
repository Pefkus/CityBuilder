using UnityEngine;

// Dziedziczenie po MonoBehaviour pozwala przypisaæ ten skrypt jako komponent 
// bezpoœrednio do prefabu na scenie (np. paczki karmy, ryby czy krzaka).
public class Food : MonoBehaviour
{
    // Zmienna publiczna, co pozwala na jej ³atw¹ edycjê w Inspektorze Unity 
    // oraz odczyt przez inne skrypty (np. skrypt pracownika).
    // Dziêki temu ró¿nym obiektom mo¿esz nadaæ ró¿n¹ wagê 
    // (np. ma³y owoc = 1.5, wielki worek = 50.0) przy u¿yciu tego samego skryptu.
    public float KgPerUnit; // Iloœæ kilogramów jedzenia, któr¹ reprezentuje ten obiekt
}