﻿using Components.Attachment;
using Systems.Events;
using UnityEngine;

// A Leaf Node that causes the actor to drop what it is holding
namespace Components.AI.Tree.Leaf {
	[DisallowMultipleComponent]
	class Drop: TreeNode {
		public override CompletionState Act(EgoComponent root) {
			Hardpoint hardpoint;
			if (root.TryGetComponents(out hardpoint) && hardpoint.Attached != null) {
				EgoEvents<DetachEvent>.AddEvent(new DetachEvent(hardpoint.Attached));
				return CompletionState.SUCCESS;
			}
			return CompletionState.FAIL;
		}
	}
}
