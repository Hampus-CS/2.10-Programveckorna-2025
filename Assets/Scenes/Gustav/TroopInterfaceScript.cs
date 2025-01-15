using UnityEngine;

public interface ITroopInterfaceScript
{
    public int health { get; set; }
    public int stress { get; set; }
    public int range { get; set; }
    public float accuracy { get; set; }
    public float suppresion { get; set; }
    public string personality { get; set; }

    void Personality();
}
