﻿//Dispatch this event to cause the Actor to use the Object
namespace Systems.Events {
	class UseEvent {
		public readonly EgoComponent Actor, Object;

		public UseEvent(EgoComponent actor, EgoComponent obj) {
			Actor=actor;
			Object=obj;
		}
	}
}
