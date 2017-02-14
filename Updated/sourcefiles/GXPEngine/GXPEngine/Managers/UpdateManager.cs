using System;
using System.Reflection;
using System.Collections.Generic;

namespace GXPEngine.Managers
{
	public class UpdateManager
	{
		private delegate void UpdateDelegate();
		private delegate void LateUpdateDelegate();
		
		private UpdateDelegate _updateDelegates;
		private LateUpdateDelegate _lateUpdateDelegates;

		private Dictionary<GameObject, UpdateDelegate> _updateReferences = new Dictionary<GameObject, UpdateDelegate>();
		private Dictionary<GameObject, LateUpdateDelegate> _lateUpdateReferences = new Dictionary<GameObject, LateUpdateDelegate>();
		
		//------------------------------------------------------------------------------------------------------------------------
		//														UpdateManager()
		//------------------------------------------------------------------------------------------------------------------------
		public UpdateManager ()
		{
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Step()
		//------------------------------------------------------------------------------------------------------------------------
		public void Step ()
		{
			if (_updateDelegates != null)
				_updateDelegates ();
			if (_lateUpdateDelegates != null)
				_lateUpdateDelegates ();
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Add()
		//------------------------------------------------------------------------------------------------------------------------
		public void Add(GameObject gameObject) {
			MethodInfo info = gameObject.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (info != null) {
				UpdateDelegate onUpdate = (UpdateDelegate)Delegate.CreateDelegate(typeof(UpdateDelegate), gameObject, info, false);
				if (onUpdate != null && !_updateReferences.ContainsKey(gameObject)) {
					_updateReferences[gameObject] = onUpdate;
					_updateDelegates += onUpdate;
				}
			} else {
				validateCase(gameObject, "Update");
			}
			info = gameObject.GetType().GetMethod("LateUpdate", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (info != null) {
				LateUpdateDelegate onLateUpdate = (LateUpdateDelegate)Delegate.CreateDelegate(typeof(LateUpdateDelegate), gameObject, info, false);
				if (onLateUpdate != null && !_lateUpdateReferences.ContainsKey(gameObject)) {
					_lateUpdateReferences[gameObject] = onLateUpdate;
					_lateUpdateDelegates += onLateUpdate;
				}
			} else {
				validateCase(gameObject, "LateUpdate");
			}
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														validateCase()
		//------------------------------------------------------------------------------------------------------------------------
		private void validateCase(GameObject gameObject, string name) {
			MethodInfo info = gameObject.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (info != null) {
				throw new Exception("'" + name + "' function was not binded for '" + gameObject + "'. Please check it's case. (Capital?)");
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Contains()
		//------------------------------------------------------------------------------------------------------------------------
		public Boolean Contains (GameObject gameObject)
		{
			return _updateReferences.ContainsKey (gameObject);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														Remove()
		//------------------------------------------------------------------------------------------------------------------------
		public void Remove(GameObject gameObject) {
			if (_updateReferences.ContainsKey(gameObject)) {
				UpdateDelegate onUpdate = _updateReferences[gameObject];
				if (onUpdate != null) _updateDelegates -= onUpdate;			
				_updateReferences.Remove(gameObject);
			}
		}
	}
}

