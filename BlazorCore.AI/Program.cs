using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Terminations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BlazorCore.AI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var selection = new GeneticSharp.Domain.Selections.RouletteWheelSelection();
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var fitness = new MyProblemFitness();
            var chromosome = new MyProblemChromosome();
            var population = new Population(50, 70, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = new GenerationNumberTermination(100);

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine("Best solution found has {0} fitness.", ga.BestChromosome.Fitness);
        }

        public class Chromosome : IChromosome
        {
            public double? Fitness { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public int Length => throw new NotImplementedException();

            public IChromosome Clone()
            {
                throw new NotImplementedException();
            }

            public int CompareTo([AllowNull] IChromosome other)
            {
                throw new NotImplementedException();
            }

            public IChromosome CreateNew()
            {
                throw new NotImplementedException();
            }

            public Gene GenerateGene(int geneIndex)
            {
                throw new NotImplementedException();
            }

            public Gene GetGene(int index)
            {
                throw new NotImplementedException();
            }

            public Gene[] GetGenes()
            {
                throw new NotImplementedException();
            }

            public void ReplaceGene(int index, Gene gene)
            {
                throw new NotImplementedException();
            }

            public void ReplaceGenes(int startIndex, Gene[] genes)
            {
                throw new NotImplementedException();
            }

            public void Resize(int newLength)
            {
                throw new NotImplementedException();
            }
        }

        public class AI
        {
            public class Environment
            {
                public int[] Observation()
                {
                    var state = new int[8];
                    state[0] = -1; // Action: -1 Left, 0 Forward, 1 Right
                    state[1] = 1; // Left Open: 0 False / 1 True
                    state[2] = 1; // Front Open
                    state[3] = 1; // Right Open
                    state[4] = 1; // Enemy Angle
                    state[5] = 1; // Enemy Direction
                    state[6] = 1; // Pickup Angle
                    state[7] = 0; // Pickup Spawn: 0 False / True

                    return state;
                }
            }
        }
    }
}
