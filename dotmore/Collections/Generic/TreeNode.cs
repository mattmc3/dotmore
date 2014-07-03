using System;
using System.Collections.Generic;
using mattmc3.dotmore.Extensions;
//using mattmc3.dotmore.Diagnostics;

namespace mattmc3.dotmore.Collections.Generic {

	public enum TreeTraversal {
		DepthFirstPreOrder,
		DepthFirstPostOrder,
		BreadthFirst
	}

	/// <summary>
	/// Represents a tree structure.
	/// </summary>
	/// <typeparam name="T">
	/// The type of values stored in the tree.
	/// </typeparam>
	public class TreeNode<T> {

		#region Fields/Properties

		private List<TreeNode<T>> _children;

		/// <summary>
		/// Gets or sets the value contained in the TreeNode.
		/// </summary>
		public T Value {
			get;
			private set;
		}

		/// <summary>
		/// The parent of the current node.  Will be null if the current node
		/// is the topmost node (aka: 'root') of the tree.
		/// </summary>
		public TreeNode<T> Parent {
			get;
			private set;
		}

		/// <summary>
		/// The first child node if one exists.  Null otherwise.
		/// </summary>
		public TreeNode<T> FirstChild {
			get {
				return this.HasChild ? _children[0] : null;
			}
		}

		/// <summary>
		/// The last child node if one exists.  Null otherwise.
		/// </summary>
		public TreeNode<T> LastChild {
			get {
				return this.HasChild ? _children[_children.Count - 1] : null;
			}
		}

		/// <summary>
		/// In the parent's list of children (aka: siblings), this gets the
		/// sibling immediately before the current node.  If the node is the
		/// root of the tree, it can still have siblings.  If it is the first
		/// sibling, this returns null.
		/// </summary>
		public TreeNode<T> PreviousSibling {
			get;
			private set;
		}

		/// <summary>
		/// In the parent's list of children (aka: siblings), this gets the
		/// sibling immediately after the current node.  If the node is the
		/// root of the tree, it can still have siblings.  If it is the last
		/// sibling, this returns null.
		/// </summary>
		public TreeNode<T> FollowingSibling {
			get;
			private set;
		}

		/// <summary>
		/// Gets the root node of the current tree.
		/// </summary>
		public TreeNode<T> Root {
			get {
				TreeNode<T> curNode = this;
				while (!curNode.IsRoot) {
					curNode = curNode.Parent;
				}
				return curNode;
			}
		}

		/// <summary>
		/// Gets the depth of the current node in the tree.  Depth starts at a
		/// value of 0 for the root node of a tree.
		/// </summary>
		public int Depth {
			get {
				int i = 0;
				TreeNode<T> curNode = this;
				while (!curNode.IsRoot) {
					curNode = curNode.Parent;
					i += 1;
				}
				return i;
			}
		}

		/// <summary>
		/// Gets the number of children for the current node.
		/// </summary>
		public int ChildCount {
			get { return _children.Count; }
		}

		/// <summary>
		/// Indicates whether or not the current node has children
		/// </summary>
		public bool HasChild {
			get { return this.ChildCount > 0; }
		}

		/// <summary>
		/// Indicates whether the current node is a leaf node.  A leaf node
		/// has no children.
		/// </summary>
		public bool IsLeaf {
			get { return this.ChildCount == 0; }
		}

		/// <summary>
		/// Indicates whether the current node is the top level node.
		/// </summary>
		public bool IsRoot {
			get { return this.Parent == null; }
		}

		/// <summary>
		/// Indicates whether this node is the first child of its parent node.
		/// </summary>
		public bool IsFirstSibling {
			get { return this.PreviousSibling == null; }
		}

		/// <summary>
		/// Indicates whether this node is the last child of its parent node.
		/// </summary>
		public bool IsLastSibling {
			get { return this.FollowingSibling == null; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default public constructor.
		/// </summary>
		public TreeNode() {
			this.Parent = null;
			this.FollowingSibling = null;
			this.PreviousSibling = null;
			_children = new List<TreeNode<T>>();
		}

		/// <summary>
		/// Constructs a new tree containing the specified value.
		/// </summary>
		/// <param name="value">The value to store in the tree.</param>
		public TreeNode(T value)
			: this() {
			this.Value = value;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Does a deep search of the tree with current node as the root to find
		/// a node with a value that matches the specified value.
		/// </summary>
		/// <param name="childValue">The value to compare to.</param>
		/// <returns>
		/// True if value exists in the tree.  False otherwise.
		/// </returns>
		public bool Contains(T childValue) {
			if (this.Value.Equals(childValue)) {
				return true;
			}
			foreach (TreeNode<T> descendant in GetDescendants()) {
				if (descendant.Value.Equals(childValue)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Does a deep search of the tree with current node as the root to find
		/// a node that matches the one specified.
		/// </summary>
		/// <param name="child">The node being searched for.</param>
		/// <returns>
		/// True if the node exists in the tree.  False otherwise.
		/// </returns>
		public bool Contains(TreeNode<T> child) {
			if (this.Equals(child)) {
				return true;
			}
			foreach (TreeNode<T> descendant in GetDescendants()) {
				if (descendant.Equals(child)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Prepends a new child to the beginning of the current node's list of
		/// children.
		/// </summary>
		/// <param name="childValue">The value represented by the new child node.</param>
		/// <returns>The newly created node containing childValue.</returns>
		public TreeNode<T> PrependChild(T childValue) {
			return AttachChild(0, childValue);
		}

		/// <summary>
		/// Prepends an existing tree node to the beginning of the current
		/// node's list of children.  If the TreeNode was already part of
		/// another tree structure, its membership is changed.
		/// </summary>
		/// <param name="child">
		/// The child node to attach.  The child node is removed from any tree
		/// structure it previously belonged to prior to attaching.
		/// </param>
		public void PrependChild(TreeNode<T> child) {
			AttachChild(0, child);
		}

		/// <summary>
		/// Appends a new child to the end of the current node's list of children.
		/// </summary>
		/// <param name="childValue">The value represented by the new child node.</param>
		/// <returns>The newly created node containing childValue.</returns>
		public TreeNode<T> AppendChild(T childValue) {
			return AttachChild(this.ChildCount, childValue);
		}

		/// <summary>
		/// Appends an existing tree node to the end of the current node's list
		/// of children.  If the TreeNode was already part of another tree
		/// structure, its membership is changed.
		/// </summary>
		/// <param name="child">
		/// The child node to attach.  The child node is removed from any tree
		/// structure it previously belonged to prior to attaching.
		/// </param>
		public void AppendChild(TreeNode<T> child) {
			AttachChild(this.ChildCount, child);
		}

		/// <summary>
		/// Attaches a new child node containing the specified value at the
		/// specified index.
		/// </summary>
		/// <param name="index">The zero-based index where the new child should be attached.</param>
		/// <param name="childValue"></param>
		/// <returns>The newly created node containing childValue.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when the child node provided is null.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown when the index specified is not a valid insertion point for the child node.
		/// Valid values are: 0 thru this.ChildCount
		/// </exception>
		public TreeNode<T> AttachChild(int index, T childValue) {
			TreeNode<T> newChild = new TreeNode<T>(childValue);
			AttachChild(index, newChild);
			return newChild;
		}

		/// <summary>
		/// Attaches the specified child node at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index where the new child should be attached.</param>
		/// <param name="child">
		/// The child node to attach.  The child node is detached from any tree
		/// structure it previously belonged to prior to attaching.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when the child node provided is null.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown when the index specified is not a valid insertion point for
		/// the child node.  Valid values are: 0 &lt;= index &lt;=
		/// this.ChildCount
		/// </exception>
		public void AttachChild(int index, TreeNode<T> child) {
			// check arguments
			if (child == null) {
				throw new ArgumentNullException("child");
			}
			if (!(0 <= index && index <= this.ChildCount)) {
				throw new IndexOutOfRangeException("The index specified is not valid: " + index.ToString());
			}

			// Detach the child from any existing tree structure.
			child.Detach();

			// get the desired previous and next siblings
			TreeNode<T> prevSibling = null;
			TreeNode<T> nextSibling = null;
			if (index != 0) {
				prevSibling = _children[index - 1];
			}
			if (index != this.ChildCount) {
				nextSibling = _children[index];
			}

			// set references
			child.PreviousSibling = prevSibling;
			child.FollowingSibling = nextSibling;
			if (prevSibling != null) {
				prevSibling.FollowingSibling = child;
			}
			if (nextSibling != null) {
				nextSibling.PreviousSibling = child;
			}

			// attach child to the parent
			child.Parent = this;
			_children.Insert(index, child);
		}

		/// <summary>
		/// Reparents and attaches a node as the following sibling to the
		/// current node.  If the current node is a root node (ie: no parent),
		/// it is still possible to attach a sibling.
		/// </summary>
		/// <param name="node">The node to attach as a following sibling to the current node.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when the node provided is null.
		/// </exception>
		public void AttachFollowingSibling(TreeNode<T> node) {
			if (node == null) {
				throw new ArgumentNullException("node");
			}

			// special case for root nodes (no parent)
			if (this.IsRoot) {
				node.Detach();
				node.FollowingSibling = this.FollowingSibling; // could be null and that's okay
				node.PreviousSibling = this;

				// repoint the existing nodes
				if (this.FollowingSibling != null) {
					this.FollowingSibling.PreviousSibling = node;
				}
				this.FollowingSibling = node;
			}
			else {
				// if this node has a parent, it's easier to attach via the parent
				int thisIndex = this.Parent._children.IndexOf(this);
				if (thisIndex < 0 || thisIndex >= this.Parent.ChildCount) throw new Exception("Unexpected error");
				this.Parent.AttachChild(thisIndex + 1, node);
			}
		}

		/// <summary>
		/// Reparents and attaches a node as the previous sibling to the current
		/// node.  If the current node is a root node (ie: no parent), it is
		/// still possible to attach a sibling.
		/// </summary>
		/// <param name="node">The node to attach as a previous sibling to the current node.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when the node provided is null.
		/// </exception>
		public void AttachPreviousSibling(TreeNode<T> node) {
			if (node == null) {
				throw new ArgumentNullException("node");
			}


			// special case for root nodes (no parent)
			if (this.IsRoot) {
				node.Detach();
				node.PreviousSibling = this.PreviousSibling; // could be null and that's okay
				node.FollowingSibling = this;

				// repoint the existing nodes
				if (this.PreviousSibling != null) {
					this.PreviousSibling.FollowingSibling = node;
				}
				this.PreviousSibling = node;
			}
			else {
				// if this node has a parent, it's easier to attach via the parent
				int thisIndex = this.Parent._children.IndexOf(this);
				if (thisIndex < 0 || thisIndex >= this.Parent.ChildCount) throw new Exception("Unexpected error");
				this.Parent.AttachChild(thisIndex, node);
			}
		}

		/// <summary>
		/// Detaches the current node from its siblings and parent so that it
		/// becomes a root node.  It retains all children.
		/// </summary>
		public void Detach() {
			// remove this node from its parent
			if (this.Parent != null) {
				bool success = this.Parent._children.Remove(this);
				if (!success) {
					throw new Exception("Unexpected error detaching node.");
				}
				this.Parent = null;
			}

			// repoint sibling references
			if (this.PreviousSibling != null) {
				this.PreviousSibling.FollowingSibling = this.FollowingSibling;
			}
			if (this.FollowingSibling != null) {
				this.FollowingSibling.PreviousSibling = this.PreviousSibling;
			}

			// remove sibling references
			this.PreviousSibling = null;
			this.FollowingSibling = null;
		}

		/// <summary>
		/// Removes all child nodes from this TreeNode.
		/// </summary>
		public void RemoveAllChildren() {
			while (_children.Count > 0) {
				RemoveChildAt(0);
			}
		}

		/// <summary>
		/// Removes the specified child node from this TreeNode's list of children.
		/// </summary>
		/// <param name="childNode">The child node to remove.</param>
		/// <returns>True if the child node was found and removed.  False otherwise.</returns>
		public bool RemoveChild(TreeNode<T> childNode) {
			int index = _children.IndexOf(childNode);
			if (index >= 0) {
				RemoveChildAt(index);
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// Removes the child node located at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the child to remove.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown when the index specified does not exist in the list of children
		/// for this TreeNode.
		/// </exception>
		public void RemoveChildAt(int index) {
			if (!(0 <= index && index < this.ChildCount)) {
				throw new IndexOutOfRangeException("The index provided is out of range: " + index.ToString());
			}

			TreeNode<T> node = _children[index];
			node.Detach();
		}

		#endregion

		#region Traversals

		public IEnumerable<TreeNode<T>> GetAncestors() {
			TreeNode<T> curNode = this;
			while (!curNode.IsRoot) {
				curNode = curNode.Parent;
				yield return curNode;
			}
		}

		public IEnumerable<TreeNode<T>> GetChildren() {
			return _children;
		}

		public IEnumerable<TreeNode<T>> GetFollowingSiblings() {
			TreeNode<T> curNode = this;
			while (!curNode.IsLastSibling) {
				curNode = curNode.FollowingSibling;
				yield return curNode;
			}
		}

		public IEnumerable<TreeNode<T>> GetPreviousSiblings() {
			TreeNode<T> curNode = this;
			while (!curNode.IsFirstSibling) {
				curNode = curNode.PreviousSibling;
				yield return curNode;
			}
		}

		public IEnumerable<TreeNode<T>> GetDescendants() {
			foreach (TreeNode<T> child in _children) {
				yield return child;
				foreach (TreeNode<T> descendant in child.GetDescendants()) {
					yield return descendant;
				}
			}
		}

		public IEnumerable<TreeNode<T>> GetSelfAndDescendants(TreeTraversal traversal) {
			switch (traversal) {
				case TreeTraversal.BreadthFirst:
					yield return this;
					foreach (var descendant in GetDescendantsBreadthFirst(new TreeNode<T>[] { this })) {
						yield return descendant;
					}
					break;

				case TreeTraversal.DepthFirstPreOrder:
					yield return this;
					foreach (TreeNode<T> descendant in GetDescendants()) {
						yield return descendant;
					}
					break;

				case TreeTraversal.DepthFirstPostOrder:
					foreach (var descendant in GetDescendantsDepthFirstPostOrder(this)) {
						yield return descendant;
					}
					break;

				default:
					throw new ArgumentException("Traversal method unhandled [{0}]".FormatWith(traversal), "traversal");
			}
		}

		private IEnumerable<TreeNode<T>> GetDescendantsBreadthFirst(IEnumerable<TreeNode<T>> nodes) {
			var children = new List<TreeNode<T>>();
			foreach (TreeNode<T> parent in nodes) {
				foreach (var child in parent.GetChildren()) {
					yield return child;
					children.Add(child);
				}
			}
			if (children.Count != 0) {
				foreach (TreeNode<T> descendant in GetDescendantsBreadthFirst(children)) {
					yield return descendant;
				}
			}
		}

		private IEnumerable<TreeNode<T>> GetDescendantsDepthFirstPostOrder(TreeNode<T> node) {
			foreach (var child in node.GetChildren()) {
				foreach (var descendant in GetDescendantsDepthFirstPostOrder(child)) {
					yield return descendant;
				}
			}
			yield return node;
		}

		#endregion
	}
}
