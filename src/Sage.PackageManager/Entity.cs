// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Sage.PackageManager
{
    /// <summary>
    /// An entity is any reachable object within the package graph.
    ///
    /// This interface is a high-level interface for interacting with the package graph and is not supposed to be a 1:1 representation of the underlying JSON.
    ///
    /// Types of entities are assets, automations, journeys, etc.
    /// </summary>
    public class Entity
    {
        protected static Regex MatchAssetId = new Regex(@"{{mcpm#/entities/assets/(?<ID>.*)/data/id}}");

        internal Entity(DataObjects.PackagedEntity packagedEntity)
        {
            this.Id = packagedEntity.originID ?? throw new InvalidDataException("Unable to identify the origin ID");
        }

        internal Entity(string id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Each entity contains an ID, such as "asset/1234" or "category/4567"
        /// </summary>
        public string Id
        {
            get;
        }

        /// <summary>
        /// These are all of the references this entity has on other entities, indexed by the entity ID.
        /// </summary>
        public IReadOnlyDictionary<string, Entity> OutgoingEdges
        {
            get
            {
                return _outgoingEdges;
            }
        }

        /// <summary>
        /// These are all of the references other entities have on this entity, indexed by the entity ID.
        /// </summary>
        public IReadOnlyDictionary<string, Entity> IncomingEdges
        {
            get
            {
                return _incomingEdges;
            }
        }

        private readonly ConcurrentDictionary<string, Entity> _outgoingEdges = new ConcurrentDictionary<string, Entity>();
        private readonly ConcurrentDictionary<string, Entity> _incomingEdges = new ConcurrentDictionary<string, Entity>();

        internal void AddOutgoing(Entity outgoing)
        {
            _outgoingEdges.AddOrUpdate(outgoing.Id, outgoing, (string id, Entity other) =>
            {
                if (other.Id != outgoing.Id)
                {
                    throw new ArgumentException(
                        $"Failure since IDs do not match. Existing: {other.Id} Adding: {outgoing.Id}");
                }

                return other;
            });

            outgoing.AddIncoming(this);
        }

        internal void AddIncoming(Entity incoming)
        {
            _incomingEdges.AddOrUpdate(incoming.Id, incoming, (string id, Entity other) =>
            {
                if (other.Id != incoming.Id)
                {
                    throw new ArgumentException(
                        $"Failure since IDs do not match. Existing: {other.Id} Adding: {incoming.Id}");
                }

                return other;
            });
        }

    }
}
