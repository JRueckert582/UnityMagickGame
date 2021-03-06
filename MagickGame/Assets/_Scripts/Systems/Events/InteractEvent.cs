﻿// Dispatch this event to cause the Actor to interact with the Object
namespace Systems.Events {
	class InteractEvent: EgoEvent {
		public readonly EgoComponent Actor, Object;

		public InteractEvent(EgoComponent actor, EgoComponent obj) {
			Actor = actor;
			Object = obj;
		}
	}
}