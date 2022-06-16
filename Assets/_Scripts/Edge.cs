using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public Tile thisTile;

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, transform.rotation * -(Vector3.forward));
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(ray);
    }

    public Edge EdgeParter()
    {
        Edge result = null;
        RaycastHit rch;
        if (Physics.Raycast(new Ray(transform.position, transform.rotation * -(Vector3.forward)), out rch, 1f))
        {
            if (rch.collider.gameObject != this.gameObject &&
                rch.collider.tag == "Edge")
            {
                result = rch.collider.GetComponent<Edge>();
                Debug.Log(this.thisTile.name + " " + this.name +":"+ result.name); // TODO: DELETE
            }
            else
            {
                Debug.Log(this.name +" NOPE : NOPE "+ result.name);// TODO: DELETE
            }
        }
        return result;
    }

    public Tile GetTile() => this.thisTile;
}
