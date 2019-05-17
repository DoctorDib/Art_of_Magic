using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using Spells;
using UnityEngine;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

public class SpellCast : MonoBehaviour {

	[SerializeField] private Camera m_Camera;

	private Vector3? m_InitialStartPoint = null;
	private Vector3? m_LineStartPoint = null;
	private Vector3? m_LineEndPoint = null;
	private bool m_CastingSpell = false;
	private bool m_CastSpell = false;
	private GameObject m_Spell;
	private int m_LineCount = 0;
	private SpellScript m_SpellScript;

	public Material lineMaterial;
	public float lineWidth = .025f;
	public float depth = .5f; 
	public float spellBoundaries = 200f;
	public float initialPointBoundaries = .02f;
	public List<Vector3> mouseCoordsSpell;
	
	//public Hashtable knownSpells = new Hashtable(); // TODO - MAKE INTO A DICTIONARY MAYBE

	private Dictionary<string, Vector3[]> knownSpells = new Dictionary<string, Vector3[]>();

	private void Start() {
		Vector3[] squareSpell = {
			new Vector3(5, 375, 0),
			new Vector3(-198, -25, 0),
			new Vector3(0, -4350, 0),
			new Vector3(-470, 0, 0),
		};
		
		Vector3[] triangleSpell = {
			new Vector3(470, 0, 0),
			new Vector3(-210, -415, 0),
			new Vector3(260, 415, 0),
		};
		
		knownSpells.Add("Square", squareSpell);
		knownSpells.Add("Triangle", triangleSpell);
	}
	
	private static bool boundaryCheck(Vector3 a, Vector3 bound, float boundary) {
		return!(a.x > bound.x - boundary && a.x < bound.x + boundary) &&
		               !(a.y > bound.y - boundary && a.y < bound.y + boundary);
	}

	private static bool AbsPosition(Vector3 a, Vector3 b, float boundary) {
		
		Debug.Log("X: " + Mathf.Abs(a.x - b.x));
		Debug.Log("Y: " + Mathf.Abs(a.y - b.y));
		
		return (Mathf.Abs(a.x - b.x) < boundary && Mathf.Abs(a.y - b.y) < boundary);
	}

	private void ResetSpell() {
		m_LineCount = 0;
		m_LineStartPoint = null;
		m_LineEndPoint = null;
		m_CastingSpell = false;
		m_CastSpell = false;
		mouseCoordsSpell = new List<Vector3>();
	}
	
	private void CheckSpell(List<Vector3> mouseChoords) {

		string detectedSpell = "No spell detected...";
		
		// Looping through known spells
		foreach (KeyValuePair<string, Vector3[]> val in knownSpells) {
			bool detected = true;
			
			// Looping user input mouse coords
			for (int userIndex = 0; userIndex < mouseChoords.Count; userIndex++) {

				Vector3 nextPoint = (userIndex+2 < mouseChoords.Count) ? mouseChoords[userIndex + 1] : mouseChoords[0];
				
				Vector3 tmp = new Vector3(
					mouseChoords[userIndex].x - nextPoint.x,
					mouseChoords[userIndex].y - nextPoint.y,
					mouseChoords[userIndex].z - nextPoint.z
				);
				
				if (userIndex+1 <= val.Value.Length && !boundaryCheck(tmp, val.Value[userIndex], spellBoundaries)) {
					Debug.Log("FAILED SPELL");
					detected = false;
				} else {
					Debug.Log("Success");
				}
			}

			detectedSpell = detected ? val.Key : detectedSpell;
		}
		
		Debug.Log("DETECTED SPELL: " + detectedSpell);
		
		ResetSpell();
	}

    // Update is called once per frame
	private void Update() {
	    
	    if (Input.GetMouseButtonDown(1)) {
		    // New spell detected
		    m_CastingSpell = true;
		    
		    m_Spell = new GameObject();
		    m_Spell.transform.parent = gameObject.transform;
		    m_Spell.name = "SpellParent";
		    m_SpellScript = m_Spell.AddComponent<SpellScript>();
	    }
	    
	    if (Input.GetMouseButtonUp(1)) {
		    ResetSpell();
	    }
	    
	    if (Input.GetMouseButtonDown(0) && !m_LineEndPoint.HasValue ) {
		    m_LineStartPoint = GetMouseCameraPoint();
		    m_InitialStartPoint = m_LineStartPoint;
	    }

		if (!m_CastingSpell || !Input.GetMouseButtonUp(0)) return;
		
		if (m_CastSpell) {
			// CAST THE SPELL
			m_SpellScript.move = true;
			CheckSpell(mouseCoordsSpell);
		} else {
			if (!m_LineStartPoint.HasValue) return;
		    
			m_LineEndPoint = GetMouseCameraPoint(); // End point

			   
			if (m_InitialStartPoint != null && AbsPosition(m_LineEndPoint.Value, m_InitialStartPoint.Value, initialPointBoundaries)) {
				m_CastSpell = true;
			}

			GameObject spellSymbol = new GameObject();
		    
			LineRenderer lineRenderer = spellSymbol.AddComponent<LineRenderer>();
			lineRenderer.material = lineMaterial;
			lineRenderer.positionCount = 2;
			if (m_LineStartPoint != null) {
				lineRenderer.SetPositions(new Vector3[] {m_LineStartPoint.Value, m_LineEndPoint.Value});
				lineRenderer.transform.parent = m_Spell.transform;
				lineRenderer.useWorldSpace = false;
				    

				spellSymbol.name = "Spell Line";
				
				lineRenderer.startWidth = lineWidth;
				lineRenderer.endWidth = lineWidth;
			}

			m_LineStartPoint = m_LineEndPoint;
			    
			Debug.Log(Input.mousePosition);
			mouseCoordsSpell.Add(Input.mousePosition);
		   
			m_LineCount++;   
		}
	}
	
	
	private Vector3 GetMouseCameraPoint() {
		Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
		return ray.origin + ray.direction * depth;
	}
}
