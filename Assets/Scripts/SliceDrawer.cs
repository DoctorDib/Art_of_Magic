using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

public class SliceDrawer : MonoBehaviour {

	[SerializeField] private Camera m_Camera;

	public Material lineMaterial;
	public float lineWidth = .025f;
	public float depth = .5f; 

	private Vector3? m_InitialStartPoint = null;
	private Vector3? m_LineStartPoint = null;
	private Vector3? m_LineEndPoint = null;
	private bool m_CastingSpell = false;
	private bool m_CastSpell = false;
	private GameObject m_Spell;
	private int m_LineCount = 0;

	public float spellBoundaries = .1f;
	public float initialPointBoundaries = .02f;
	
	public List<Vector3> mouseCoordsSpell;
	public Hashtable knownSpells =new Hashtable(); // TODO - MAKE INTO A DICTIONARY MAYBE

	void Start() {
		Vector3[] squareSpell = {
			new Vector3(0.1f, .3f, 0),
			new Vector3(0.2f, 0, 3),
			new Vector3(0.2f, -.3f, 0),
			new Vector3(-0.3f, 0, -3),
		};
		
		knownSpells["SquareSpell"] = squareSpell;
	}

	private bool ABSPosition(Vector3 a, Vector3 b, float boundary) {
		return m_InitialStartPoint != null && 
		    (Mathf.Abs(a.x - b.x) < boundary && 
		     Mathf.Abs(a.y - b.y) < boundary && 
		     Mathf.Abs(a.z - b.z) < boundary );
	}
	
	void ResetSpell() {
		m_LineCount = 0;
		mouseCoordsSpell = new List<Vector3>();
		
		m_LineStartPoint = null;
		m_LineEndPoint = null;
		m_CastingSpell = false;
		m_CastSpell = false;
		Destroy(m_Spell);
	}
	
	private void CheckSpell(List<Vector3> mouseChoords) {

		// Looping through known spells
		foreach (object val in knownSpells) {
			Debug.Log(val);
			Debug.Log(knownSpells[val]); 
			
			//Debug.Log(knownSpells.Count);
			//Debug.Log(val);
			string currentSpell = "";
			
			ICollection key = knownSpells.Keys; 
			
			// Looping user input mouse coords
			for (var userIndex = 0; userIndex < mouseChoords.Count; userIndex++) {

				Vector3 nextPoint = userIndex+2 <= mouseChoords.Count ? mouseChoords[userIndex + 1] : mouseChoords[0];
				

				Vector3 tmp = new Vector3(
					mouseChoords[userIndex].x - nextPoint.x,
					mouseChoords[userIndex].y - nextPoint.y,
					mouseChoords[userIndex].z - nextPoint.z
				);
				
				
				
				/*if (!ABSPosition(tmp, knownSpells[val][userIndex], spellBoundaries)) {
					Debug.Log("FAILED SPELL");	
				} else {
					Debug.Log("Success");
				}*/
			}
		}
		
		ResetSpell();
	}
	
	/*private void CheckSpell(List<Vector3> mouseChoords) {

		// Looping through known spells
		for (var index = 0; index < knownSpells.Count; index++) {
			Debug.Log(knownSpells.Count);
			Debug.Log(index);
			string currentSpell = "";
			
			ICollection key = knownSpells.Keys; 
			
			// Looping user input mouse coords
			for (var userIndex = 0; userIndex < mouseChoords.Count; userIndex++) {

				Vector3 nextPoint = userIndex+2 <= mouseChoords.Count ? mouseChoords[userIndex + 1] : mouseChoords[0];
				

				Vector3 tmp = new Vector3(
					mouseChoords[userIndex].x - nextPoint.x,
					mouseChoords[userIndex].y - nextPoint.y,
					mouseChoords[userIndex].z - nextPoint.z
				);
				
				if (userIndex+1 <= knownSpells[index].Count && !ABSPosition(tmp, knownSpells[index][userIndex], spellBoundaries)) {
					Debug.Log("FAILED SPELL");	
				} else {
					Debug.Log("Success");
				}
			}
		}
		
		ResetSpell();
	}*/

    // Update is called once per frame
    void Update() {
	    
	    if (Input.GetMouseButtonDown(1)) {
		    // New spell detected
		    m_CastingSpell = true;
		    
		    m_Spell = new GameObject();
		    m_Spell.transform.parent = gameObject.transform;
		    m_Spell.name = "SpellParent";
	    }
	    
	    if (Input.GetMouseButtonUp(1)) {
		    ResetSpell();
	    }
	    
	    if (Input.GetMouseButtonDown(0) && !m_LineEndPoint.HasValue ) {
		    m_LineStartPoint = GetMouseCameraPoint();
		    m_InitialStartPoint = m_LineStartPoint;
	    }
	    
	    if (m_CastingSpell && Input.GetMouseButtonUp(0)) {
		    if (m_CastSpell) {
			    // CAST THE SPELL
			    Debug.Log("Casting spell");
			    CheckSpell(mouseCoordsSpell);
		    } else {
			    if (!m_LineStartPoint.HasValue) return;
		    
			    m_LineEndPoint = GetMouseCameraPoint(); // End point

			   
			    if (m_InitialStartPoint != null && ABSPosition(m_LineEndPoint.Value, m_InitialStartPoint.Value, initialPointBoundaries)) {
				    m_CastSpell = true;
			    }

			    GameObject spellSymbol = new GameObject();
		    
			    var lineRenderer = spellSymbol.AddComponent<LineRenderer>();
			    lineRenderer.material = lineMaterial;
			    lineRenderer.positionCount = 2;
			    if (m_LineStartPoint != null) {
				    lineRenderer.SetPositions(new Vector3[] {m_LineStartPoint.Value, m_LineEndPoint.Value});
				    lineRenderer.transform.parent = m_Spell.transform;

				    spellSymbol.name = "Spell Line";

				    lineRenderer.startWidth = lineWidth;
				    lineRenderer.endWidth = lineWidth;
			    }

			    m_LineStartPoint = m_LineEndPoint;
			    Debug.Log(m_LineStartPoint.Value);
			    mouseCoordsSpell.Add(m_LineStartPoint.Value); 
			    
		   
			    m_LineCount++;   
		    }
	    }
    }
	
	
	private Vector3 GetMouseCameraPoint() {
		var ray = m_Camera.ScreenPointToRay(Input.mousePosition);
		return ray.origin + ray.direction * depth;
	}
}
