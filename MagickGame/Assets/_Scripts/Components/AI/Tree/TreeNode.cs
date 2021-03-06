﻿// Base node type of a behavior tree.
namespace Components.AI.Tree {
	public abstract class TreeNode {
		//Called by parent node or BehaviorTreeSystem to perform some function.
		public abstract CompletionState Act(EgoComponent root);
	}
	// Denotes the completion state of some Node, so that its parent can react to it.
	public enum CompletionState {
		SUCCESS, FAIL, RUNNING
	}
}
