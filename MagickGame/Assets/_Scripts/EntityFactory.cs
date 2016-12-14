﻿using UnityEngine;
using System.Collections.Generic;
using System;

/* Current rules for modifying component lists:
 * - Can't have more than one of the same component
 * - Superclasses are replaced with subclasses
 * - Archetypes cannot be modified, instead new ones are generated with desired modifications
 * - Non-archetype modifications are not supported
 */

public static class EntityFactory
{
	public static GameObject generate(params ComponentWrapper[] components) {
		GameObject entity = new GameObject ();
		foreach(ComponentWrapper wrapper in removeDuplicates(components)) {
			Component comp = entity.AddComponent (wrapper.value);
			if(wrapper.action != null) wrapper.action (comp);
		}
		return entity;
	}

	public static Archetype buildArchetype(params ComponentWrapper[] components) {
		return new Archetype (components);
	}

	static ComponentWrapper[] removeDuplicates(ComponentWrapper[] componenents) {
		List<ComponentWrapper> keeps = new List<ComponentWrapper> ();
		foreach (ComponentWrapper wrapper in componenents) {
			if (keeps.FindAll (x => wrapper.value.IsAssignableFrom (x.value)).Count == 0) {
				if (keeps.Remove (keeps.Find (x => x.value.IsAssignableFrom (wrapper.value)))) {
					throw new Exceptions.ComponentListModifyException ("Superclass of type " + wrapper.value.ToString () +
						" already found. Replacing superclass with subclass.");
				}
				keeps.Add (wrapper);
			} else {
				throw new Exceptions.ComponentListModifyException ("Component of type " + wrapper.value.ToString () +
					" already found. Cannot add.");
			}
		}
		return keeps.ToArray ();
	}

	public class Archetype {
		List<ComponentWrapper> components;

		public Archetype(params ComponentWrapper[] components) {
			this.components = new List<ComponentWrapper>();
			this.components.AddRange(removeDuplicates(components));
		}

		/* Returns a new archetype that contains this archetype's components, 
		 * in addition to the ones in the given components as long as there aren't any 
		 * instances of them in the component list already.
		 */
		public Archetype add(params ComponentWrapper[] components) {
			List<ComponentWrapper> list = new List<ComponentWrapper> ();
			list.AddRange (this.components);
			foreach (ComponentWrapper wrapper in components) {
				if (list.FindAll (x => x.value.IsAssignableFrom (wrapper.value)).Count == 0) {
					list.Add (wrapper);
				} else 
					throw new Exceptions.ComponentListModifyException("Component of type " + wrapper.value.ToString () + 
						" already found. Cannot add.");
			}
			return new Archetype (list.ToArray ());
		}

		/* Returns a new archetype that contains this archetype's components, 
		 * except for the ones that are instances of the given components. Disregards actions.
		 */
		public Archetype remove(params ComponentWrapper[] components) {
			List<ComponentWrapper> list = new List<ComponentWrapper> ();
			list.AddRange (this.components);
			foreach(ComponentWrapper wrapper in components) {
				if (list.RemoveAll (x => x.value.IsAssignableFrom (wrapper.value)) == 0) {
					throw new Exceptions.ComponentListModifyException ("No components of type " + wrapper.value.ToString () + 
						" found. Cannot remove.");
				}
			}
			return new Archetype (list.ToArray ());
		}
		
		/* Returns a new archetype that contains this archetype's components,
		 * except with the given componenets replacing those that it is an instance of.
		 */
		public Archetype replace(params ComponentWrapper[] components) {
			List<ComponentWrapper> list = new List<ComponentWrapper> ();
			list.AddRange (this.components);
			foreach(ComponentWrapper wrapper in components) {
				List<ComponentWrapper> matches = list.FindAll (x => wrapper.value.IsAssignableFrom (x.value));
				if (matches.Count == 0)
					throw new Exceptions.ComponentListModifyException  ("No components of type " + wrapper.value.ToString () + 
						" found. Cannot replace.");
				else {
					foreach (ComponentWrapper comp in matches) {
						list.Remove (comp);
					}
					list.Add (wrapper);
				}
			}
			return new Archetype (list.ToArray ());
		}

		public static implicit operator ComponentWrapper[](Archetype a) {
			return a.components.ToArray ();
		}
	}
}

public struct ComponentWrapper {
	public Type value;
	public Action<Component> action;

	public ComponentWrapper(Type value, Action<Component> action) {
		this.value = value;
		this.action = action;
	}

	public static implicit operator ComponentWrapper(Type t) {
		return new ComponentWrapper (t, null);
	}
}
