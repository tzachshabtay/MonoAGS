using System;
using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    /// <summary>
    /// This class is intended to add/remove tags for objects that are being followed,
    /// so one can then call the extension method WhoIsFollowingMe and get a list of
    /// all the entities that are following that target.
    /// </summary>
    public static class FollowTag
    {
        private const string FOLLOWER_PREFIX = "Followed by-->";

        /// <summary>
        /// Adds a tag to the target to mark that it's being followed by the follower
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="follower">Follower.</param>
        public static void AddTag(IObject target, IEntity follower)
        {
            target.Properties.Entities.SetValue(string.Format("{0}{1}", FOLLOWER_PREFIX, follower.ID), follower);   
        }


        /// <summary>
        /// Removes a tag from the target to mark that it's no longer being followed by the follower
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="follower">Follower.</param>
        public static void RemoveTag(IObject target, IEntity follower)
        {
            target.Properties.Entities.SetValue(string.Format("{0}{1}", FOLLOWER_PREFIX, follower.ID), null);
        }

        /// <summary>
        /// Returns a list of all the entities that are currently following the target.
        /// </summary>
        /// <param name="target">Target.</param>
        public static List<IEntity> WhoIsFollowingMe(this IObject target)
        {
            List<IEntity> entities = new List<IEntity>();
            foreach (var pair in target.Properties.Entities.AllProperties())
            {
                IEntity entity = pair.Value;
                if (entity == null || !pair.Key.StartsWith(FOLLOWER_PREFIX, StringComparison.CurrentCulture)) continue;
                entities.Add(entity);
            }
            return entities;
        }
    }
}
