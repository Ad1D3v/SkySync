using System.Collections.Generic;
using Unity.MLAgents.Policies;
using Unity.Barracuda;
using Onnx;
using System;
using System.Runtime.Serialization;

namespace ML.Link_0A
{
    /// <summary>
    /// Mapping between the output tensor names and the method that will use the
    /// output tensors and the Agents present in the batch to update their action, memories and
    /// value estimates.
    /// A TensorApplier implements a Dictionary of strings (node names) to an Action.
    /// This action takes as input the tensor and the Dictionary of Agent to AgentInfo for
    /// the current batch.
    /// </summary>
    internal class TensorApplier
    {
        /// <summary>
        /// A tensor Applier's Execute method takes a tensor and a Dictionary of Agent to AgentInfo.
        /// Uses the data contained inside the tensor to modify the state of the Agent. The Tensors
        /// are assumed to have the batch size on the first dimension and the agents to be ordered
        /// the same way in the dictionary and in the tensor.
        /// </summary>
        public interface IApplier
        {
            /// <summary>
            /// Applies the values in the Tensor to the Agents present in the agentInfos
            /// </summary>
            /// <param name="tensorProxy">
            /// The Tensor containing the data to be applied to the Agents
            /// </param>
            /// <param name="actionIds"> List of Agents Ids that will be updated using the tensor's data</param>
            /// <param name="lastActions"> Dictionary of AgentId to Actions to be updated</param>
            void Apply(TensorProto tensorProxy, IEnumerable<int> actionIds, Dictionary<int, float[]> lastActions);
            //void Apply(TensorProto tensor, IEnumerable<int> actionIds, Dictionary<int, float[]> lastActions);
        }

        readonly Dictionary<string, IApplier> m_Dict = new Dictionary<string, IApplier>();

        /// <summary>
        /// Returns a new TensorAppliers object.
        /// </summary>
        /// <param name="bp"> The BrainParameters used to determine what Appliers will be
        /// used</param>
        /// <param name="seed"> The seed the Appliers will be initialized with.</param>
        /// <param name="allocator"> Tensor allocator</param>
        /// <param name="memories">Dictionary of AgentInfo.id to memory used to pass to the inference model.</param>
        /// <param name="barracudaModel"></param>
        public TensorApplier(
            BrainParameters bp,
            int seed,
            ITensorAllocator allocator,
            Dictionary<int, List<float>> memories,
            object barracudaModel = null)
        {
            if (bp.VectorActionSpaceType == SpaceType.Continuous)
            {
                m_Dict[TensorNames.ActionOutput] = new ContinuousActionOutputApplier();
            }
            else
            {
                m_Dict[TensorNames.ActionOutput] =
                    new DiscreteActionOutputApplier(bp.VectorActionSize, seed, allocator);
            }
            m_Dict[TensorNames.RecurrentOutput] = new MemoryOutputApplier(memories);

            if (barracudaModel != null)
            {
                var model = (Model)barracudaModel;

                for (var i = 0; i < model?.memories.Count; i++)
                {
                    m_Dict[model.memories[i].output] =
                        new BarracudaMemoryOutputApplier(model.memories.Count, i, memories);
                }
            }
        }

        /// <summary>
        /// Updates the state of the agents based on the data present in the tensor.
        /// </summary>
        /// <param name="tensors"> Enumerable of tensors containing the data.</param>
        /// <param name="actionIds"> List of Agents Ids that will be updated using the tensor's data</param>
        /// <param name="lastActions"> Dictionary of AgentId to Actions to be updated</param>
        /// <exception cref="UnityAgentsException"> One of the tensor does not have an
        /// associated applier.</exception>
        public void ApplyTensors(
            IEnumerable<TensorProto> tensors, IEnumerable<int> actionIds, Dictionary<int, float[]> lastActions)
        {
            foreach (var tensor in tensors)
            {
                if (!m_Dict.ContainsKey(tensor.Name))
                {
                    throw new UnityAgentsException(
                        $"Unknown tensorProxy expected as output : {tensor.Name}");
                }
                m_Dict[tensor.Name].Apply(tensor, actionIds, lastActions);
            }
        }

        internal void ApplyTensors(IReadOnlyList<TensorProxy> m_InferenceOutputs, List<int> m_OrderedAgentsRequestingDecisions, Dictionary<int, float[]> m_LastActionsReceived)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    internal class UnityAgentsException : Exception
    {
        public UnityAgentsException()
        {
        }

        public UnityAgentsException(string message) : base(message)
        {
        }

        public UnityAgentsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnityAgentsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    internal class DiscreteActionOutputApplier : TensorApplier.IApplier
    {
        private int[] vectorActionSize;
        private int seed;
        private ITensorAllocator allocator;

        public DiscreteActionOutputApplier(int[] vectorActionSize, int seed, ITensorAllocator allocator)
        {
            this.vectorActionSize = vectorActionSize;
            this.seed = seed;
            this.allocator = allocator;
        }

        public void Apply(TensorProto tensorProxy, IEnumerable<int> actionIds, Dictionary<int, float[]> lastActions)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class ContinuousActionOutputApplier : TensorApplier.IApplier
    {
        void TensorApplier.IApplier.Apply(TensorProto tensorProxy, IEnumerable<int> actionIds, Dictionary<int, float[]> lastActions)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class TensorNames
    {
        public static string ActionOutput { get; internal set; }
        public static string RecurrentOutput { get; internal set; }
    }

    internal class MemoryOutputApplier : TensorApplier.IApplier
    {
        private Dictionary<int, List<float>> memories;

        public MemoryOutputApplier(Dictionary<int, List<float>> memories)
        {
            this.memories = memories;
        }

        void TensorApplier.IApplier.Apply(TensorProto tensorProxy, IEnumerable<int> actionIds, Dictionary<int, float[]> lastActions)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class BarracudaMemoryOutputApplier : TensorApplier.IApplier
    {
        private int count;
        private int i;
        private Dictionary<int, List<float>> memories;

        public BarracudaMemoryOutputApplier(int count, int i, Dictionary<int, List<float>> memories)
        {
            this.count = count;
            this.i = i;
            this.memories = memories;
        }

        void TensorApplier.IApplier.Apply(TensorProto tensorProxy, IEnumerable<int> actionIds, Dictionary<int, float[]> lastActions)
        {
            throw new System.NotImplementedException();
        }
    }
}
