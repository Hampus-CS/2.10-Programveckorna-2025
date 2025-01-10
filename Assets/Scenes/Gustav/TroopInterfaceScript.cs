using UnityEngine;

public interface ITroopInterfaceScript
{
    public int health { get; set; }
    public int stress { get; set; }
    public string personality { get; set; }


    void Shoot(Vector3 target, GameObject weapon);
    void EnterCombat(Vector3 target, int stress);
    void Personality();
}
