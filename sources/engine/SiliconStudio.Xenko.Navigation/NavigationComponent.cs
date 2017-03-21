﻿// Copyright (c) 2016-2017 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Design;
using SiliconStudio.Xenko.Navigation.Processors;

namespace SiliconStudio.Xenko.Navigation
{
    /// <summary>
    /// This is used to interface with the navigation mesh. Supports TryFindPath and Raycast
    /// </summary>
    [DataContract("NavigationComponent")]
    [Display("Navigation", Expand = ExpandRule.Once)]
    [ComponentOrder(20000)]
    [DefaultEntityComponentProcessor(typeof(NavigationProcessor), ExecutionMode = ExecutionMode.Runtime)]
    public class NavigationComponent : EntityComponent
    {
        [DataMemberIgnore] internal RecastNavigationMesh RecastNavigationMesh;
        [DataMemberIgnore] internal Vector3 SceneOffset;
        
        private NavigationMesh navigationMesh;
        private NavigationMeshGroup group;

        /// <summary>
        /// The navigation mesh which is being used. Setting this to <c>null</c> will load the dynamic navigation mesh for the current game (if enabled)
        /// </summary>
        [DataMember(10)]
        public NavigationMesh NavigationMesh
        {
            get { return navigationMesh; }
            set
            {
                navigationMesh = value; 
                NavigationMeshChanged?.Invoke(this, null);
            }
        }

        /// <summary>
        /// The navigation mesh group to use
        /// </summary>
        [DataMember(20)]
        public NavigationMeshGroup Group
        {
            get { return group; }
            set
            {
                group = value;
                NavigationMeshChanged?.Invoke(this, null);
            }
        }

        /// <summary>
        /// Raised when the navigation mesh changed on this component
        /// </summary>
        internal event EventHandler NavigationMeshChanged;

        /// <summary>
        /// Finds a path from the entity's current location to <see cref="end"/>
        /// </summary>
        /// <param name="end">The ending location of the pathfinding query</param>
        /// <param name="path">The resulting collection of waypoints for the found path, if any (at least 2 if a path was found)</param>
        /// <returns>The found path points or null</returns>
        public bool TryFindPath(Vector3 end, IList<Vector3> path)
        {
            return TryFindPath(Entity.Transform.WorldMatrix.TranslationVector, end, path, NavigationQuerySettings.Default);
        }

        /// <summary>
        /// Finds a path from the entity's current location to <see cref="end"/>
        /// </summary>
        /// <param name="end">The ending location of the pathfinding query</param>
        /// <param name="querySettings">Advanced settings to be provided to the navigation mesh query</param>
        /// <param name="path">The resulting collection of waypoints for the found path, if any (at least 2 if a path was found)</param>
        /// <returns>True if a valid path was found</returns>
        public bool TryFindPath(Vector3 end, IList<Vector3> path, NavigationQuerySettings querySettings)
        {
            return TryFindPath(Entity.Transform.WorldMatrix.TranslationVector, end, path, querySettings);
        }

        /// <summary>
        /// Finds a path from point <see cref="start"/> to <see cref="end"/>
        /// </summary>
        /// <param name="start">The starting location of the pathfinding query</param>
        /// <param name="end">The ending location of the pathfinding query</param>
        /// <param name="path">The resulting collection of waypoints for the found path, if any (at least 2 if a path was found)</param>
        /// <returns>True if a valid path was found</returns>
        public bool TryFindPath(Vector3 start, Vector3 end, IList<Vector3> path)
        {
            return TryFindPath(start, end, path, NavigationQuerySettings.Default);
        }

        /// <summary>
        /// Finds a path from point <see cref="start"/> to <see cref="end"/>
        /// </summary>
        /// <param name="start">The starting location of the pathfinding query</param>
        /// <param name="end">The ending location of the pathfinding query</param>
        /// <param name="querySettings">Advanced settings to be provided to the navigation mesh query</param>
        /// <param name="path">The resulting collection of waypoints for the found path, if any (at least 2 if a path was found)</param>
        /// <returns>True if a valid path was found</returns>
        public bool TryFindPath(Vector3 start, Vector3 end, IList<Vector3> path, NavigationQuerySettings querySettings)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (RecastNavigationMesh == null)
                return false;

            start -= SceneOffset;
            end -= SceneOffset;
            if (!RecastNavigationMesh.TryFindPath(start, end, path, querySettings))
                return false;
            
            for (int i = 0; i < path.Count; i++)
            {
                path[i] += SceneOffset;
            }
            return true;
        }

        /// <summary>
        /// Performs a raycast on the navigation mesh to perform line of sight or similar checks. Starts from the entity's current world position
        /// </summary>
        /// <param name="end">Ending point</param>
        /// <returns>The found raycast hit if <see cref="NavigationRaycastResult.Hit"/> is true</returns>
        public NavigationRaycastResult Raycast(Vector3 end)
        {
            return Raycast(Entity.Transform.WorldMatrix.TranslationVector, end, NavigationQuerySettings.Default);
        }

        /// <summary>
        /// Performs a raycast on the navigation mesh to perform line of sight or similar checks.  Starts from the entity's current world position
        /// </summary>
        /// <param name="end">Ending point</param>
        /// <param name="querySettings">Advanced settings to be provided to the navigation mesh query</param>
        /// <returns>The found raycast hit if <see cref="NavigationRaycastResult.Hit"/> is true</returns>
        public NavigationRaycastResult Raycast(Vector3 end, NavigationQuerySettings querySettings)
        {
            return Raycast(Entity.Transform.WorldMatrix.TranslationVector, end, querySettings);
        }

        /// <summary>
        /// Performs a raycast on the navigation mesh to perform line of sight or similar checks.
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">Ending point</param>
        /// <returns>The found raycast hit if <see cref="NavigationRaycastResult.Hit"/> is true</returns>
        public NavigationRaycastResult Raycast(Vector3 start, Vector3 end)
        {
            return Raycast(start, end, NavigationQuerySettings.Default);
        }

        /// <summary>
        /// Performs a raycast on the navigation mesh to perform line of sight or similar checks
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">Ending point</param>
        /// <param name="querySettings">Advanced settings to be provided to the navigation mesh query</param>
        /// <returns>The found raycast hit if <see cref="NavigationRaycastResult.Hit"/> is true</returns>
        public NavigationRaycastResult Raycast(Vector3 start, Vector3 end, NavigationQuerySettings querySettings)
        {
            NavigationRaycastResult result = new NavigationRaycastResult { Hit = false };

            if (RecastNavigationMesh == null)
                return result;
            
            start -= SceneOffset;
            end -= SceneOffset;
            result = RecastNavigationMesh.Raycast(start, end, querySettings);
            result.Position += SceneOffset;
            return result;
        }
    }
}